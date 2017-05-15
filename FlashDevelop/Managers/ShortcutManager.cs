using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using FlashDevelop.Helpers;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace FlashDevelop.Managers
{
    /// <summary>
    /// A manager class for shortcuts.
    /// </summary>
    internal static class ShortcutManager
    {
        private const string VersionKey = "version";
        private const string ShortcutKeyDelimiter = "; ";
        private static readonly int FileVersionCurrent = 1;
        private static readonly int FileVersionExtendedShortcut = 1;

        private static HashSet<ShortcutKeys> allShortcuts;
        private static HashSet<Keys> altFirstKeys;
        private static List<ShortcutKeys> ignoredKeys;
        private static Dictionary<string, ShortcutItem> registeredItems;
        private static Dictionary<ShortcutKeys, ShortcutItem> cachedItems;

        static ShortcutManager()
        {
            allShortcuts = new HashSet<ShortcutKeys>();
            altFirstKeys = new HashSet<Keys>();
            ignoredKeys = new List<ShortcutKeys>();
            registeredItems = new Dictionary<string, ShortcutItem>();
            cachedItems = new Dictionary<ShortcutKeys, ShortcutItem>();
        }

        /// <summary>
        /// Gets a collection of all shortcut keys.
        /// </summary>
        internal static HashSet<ShortcutKeys> AllShortcuts
        {
            get { return allShortcuts; }
        }

        /// <summary>
        /// Gets a collection of the first parts of all extended shortcuts with only <see cref="Keys.Alt"/> as their modifiers in the first parts.
        /// </summary>
        internal static HashSet<Keys> AltFirstKeys
        {
            get { return altFirstKeys; }
        }

        /// <summary>
        /// Gets a list of ignored keys.
        /// </summary>
        internal static List<ShortcutKeys> IgnoredKeys
        {
            get { return ignoredKeys; }
        }

        /// <summary>
        /// Gets a collection of all registered shortcut items.
        /// </summary>
        internal static ICollection<ShortcutItem> RegisteredItems
        {
            get { return registeredItems.Values; }
        }

        /// <summary>
        /// Registers a shortcut.
        /// </summary>
        internal static void RegisterShortcut(string id, ShortcutKeys[] defaultShortcuts, ToolStripItem[] toolStripItems)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            ShortcutItem item;
            if (!registeredItems.TryGetValue(id, out item))
            {
                item = new ShortcutItem(id, new ShortcutKeys[0], new ToolStripItem[0]);
                registeredItems.Add(id, item);
            }

            var shortcutsUnion = new HashSet<ShortcutKeys>(item.Default);
            if (defaultShortcuts != null && defaultShortcuts.Length > 0)
            {
                shortcutsUnion.UnionWith(defaultShortcuts);
            }

            if (toolStripItems != null && toolStripItems.Length > 0)
            {
                var itemsUnion = new HashSet<ToolStripItem>(item.Items);
                foreach (var toolStripItem in toolStripItems)
                {
                    if (toolStripItem != null)
                    {
                        itemsUnion.Add(toolStripItem);
                        if (toolStripItem is ToolStripMenuItemEx)
                        {
                            shortcutsUnion.Add(((ToolStripMenuItemEx) toolStripItem).ShortcutKeys);
                            ((ToolStripMenuItemEx) toolStripItem).ShortcutKeys = ShortcutKeys.None;
                        }
                        else if (toolStripItem is ToolStripMenuItem)
                        {
                            shortcutsUnion.Add((ShortcutKeys) ((ToolStripMenuItem) toolStripItem).ShortcutKeys);
                            ((ToolStripMenuItem) toolStripItem).ShortcutKeys = Keys.None;
                        }
                        else if (toolStripItem.Tag is ItemData)
                        {
                            if (((ItemData) toolStripItem.Tag).KeyId != id)
                            {
                                throw new ArgumentException($"The shortcut ID of the specified {nameof(ToolStripItem)} object is different from the specified shortcut ID '{id}': {((ItemData) toolStripItem.Tag).KeyId}");
                            }
                        }
                        else
                        {
                            toolStripItem.Tag = new ItemData(null, id, null, null);
                        }
                    }
                }

                item.Items = new ToolStripItem[itemsUnion.Count];
                itemsUnion.CopyTo(item.Items);
            }

            shortcutsUnion.Remove(ShortcutKeys.None);
            item.Default = new ShortcutKeys[shortcutsUnion.Count];
            shortcutsUnion.CopyTo(item.Default);
            item.Custom = item.Default;
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        internal static ShortcutItem GetRegisteredItem(string id)
        {
            ShortcutItem item;
            return registeredItems.TryGetValue(id, out item) ? item : null;
        }

        /// <summary>
        /// Gets the specified registered shortcut item.
        /// </summary>
        internal static ShortcutItem GetRegisteredItem(ShortcutKeys keys)
        {
            ShortcutItem item;
            return cachedItems.TryGetValue(keys, out item) ? item : null;
        }

        /// <summary>
        /// Applies all shortcuts to the items.
        /// </summary>
        internal static void ApplyAllShortcuts()
        {
            allShortcuts.Clear();
            altFirstKeys.Clear();
            cachedItems.Clear();

            foreach (var item in RegisteredItems)
            {
                for (int i = 0; i < item.Custom.Length; i++)
                {
                    var keys = item.Custom[i];
                    if (allShortcuts.Add(keys))
                    {
                        cachedItems.Add(keys, item);
                        if (keys.IsExtended && (keys.First & Keys.Modifiers) == Keys.Alt)
                        {
                            altFirstKeys.Add(keys.First);
                        }
                    }
                    else
                    {
                        TraceManager.Add($"Duplicate shortcut definitions with '{keys}': '{cachedItems[keys].Id}', '{item.Id}'");
                    }
                }
            }

            for (int i = 0; i < ignoredKeys.Count; i++)
            {
                allShortcuts.Add(ignoredKeys[i]);
            }

            foreach (var item in RegisteredItems)
            {
                var keys = item.Custom.Length > 0 ? item.Custom[0] : ShortcutKeys.None;
                for (int i = 0; i < item.Items.Length; i++)
                {
                    UpdateShortcutKeyDisplayString(item.Items[i], keys);
                }

                var e = new ShortcutUpdateEvent(EventType.ShortcutUpdate, item.Id, item.Custom);
                EventManager.DispatchEvent(Globals.MainForm, e);
            }
        }

        /// <summary>
        /// Updates the shortcut display string of the specified item, by using its ItemData stored in the Tag property.
        /// </summary>
        internal static void UpdateShortcutKeyDisplayString(ToolStripItem item)
        {
            if (item?.Tag is ItemData)
            {
                var keys = Globals.MainForm.GetShortcutKeys(((ItemData) item.Tag).KeyId);
                UpdateShortcutKeyDisplayString(item, keys);
            }
        }

        /// <summary>
        /// Updates the shortcut display string of the specified item to the specified shortcut keys.
        /// </summary>
        private static void UpdateShortcutKeyDisplayString(ToolStripItem item, ShortcutKeys keys)
        {
            bool showShortcutKeys = !keys.IsNone && Globals.Settings.ViewShortcuts;

            if (item is ToolStripMenuItem)
            {
                ((ToolStripMenuItem) item).ShortcutKeyDisplayString = showShortcutKeys ? keys.ToString() : null;
            }
            else
            {
                string displayString = showShortcutKeys ? " (" + keys + ")" : null;
                int index = item.ToolTipText.LastIndexOfOrdinal(" (");
                if (index >= 0)
                {
                    item.ToolTipText = item.ToolTipText.Remove(index) + displayString;
                }
                else
                {
                    item.ToolTipText += displayString;
                }
            }
        }

        /// <summary>
        /// Loads the custom shortcuts from a file.
        /// </summary>
        internal static void LoadCustomShortcuts()
        {
            try
            {
                string file = FileNameHelper.ShortcutData;
                if (!File.Exists(file))
                {
                    if (File.Exists(FileNameHelper.ShortcutDataOld))
                    {
                        File.Move(FileNameHelper.ShortcutDataOld, file);
                    }
                    else
                    {
                        return;
                    }
                }

                var shortcuts = new List<Argument>();
                shortcuts = (List<Argument>) ObjectSerializer.Deserialize(file, shortcuts, false);

                int version = 0;
                int i = 0;
                int count = shortcuts.Count;

                if (count > 0 && shortcuts[0].Key == VersionKey)
                {
                    version = int.Parse(shortcuts[0].Value);
                    i = 1;
                }

                for (; i < count; i++)
                {
                    var arg = shortcuts[i];
                    var item = GetRegisteredItem(arg.Key);
                    if (item != null)
                    {
                        try
                        {
                            if (version >= FileVersionExtendedShortcut)
                            {
                                item.Custom = DeserializeKeys(arg.Value);
                            }
                            else // for backward compatibility
                            {
                                item.Custom = new[] { (ShortcutKeys) (Keys) Enum.Parse(typeof(Keys), arg.Value) };
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorManager.ShowError($"{{ \"{arg.Key}\" = \"{arg.Value}\" }}\n{ex.Message}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves the custom shortcuts to a file.
        /// </summary>
        internal static void SaveCustomShortcuts()
        {
            try
            {
                var shortcuts = new List<Argument>()
                {
                    new Argument(VersionKey, FileVersionCurrent.ToString())
                };
                foreach (var item in RegisteredItems)
                {
                    if (item.IsModified)
                    {
                        shortcuts.Add(new Argument(item.Id, SerializeKeys(item.Custom)));
                    }
                }
                ObjectSerializer.Serialize(FileNameHelper.ShortcutData, shortcuts);
            }
            catch (Exception e)
            {
                ErrorManager.ShowError(e);
            }
        }

        /// <summary>
        /// Loads the custom shortcuts from a file to a list.
        /// </summary>
        internal static void LoadCustomShortcuts(string file, IEnumerable<IShortcutItem> items)
        {
            try
            {
                if (!File.Exists(file))
                {
                    return;
                }

                var customShortcuts = new List<Argument>();
                customShortcuts = (List<Argument>) ObjectSerializer.Deserialize(file, customShortcuts, false);

                int version = 0;
                int start = 0;
                int count = customShortcuts.Count;

                if (count > 0 && customShortcuts[0].Key == VersionKey)
                {
                    version = int.Parse(customShortcuts[0].Value);
                    start = 1;
                }

                foreach (var item in items)
                {
                    var newShortcut = item.Default;
                    for (int i = start; i < count; i++)
                    {
                        var arg = customShortcuts[i];
                        if (arg.Key == item.Id)
                        {
                            try
                            {
                                if (version >= FileVersionExtendedShortcut)
                                {
                                    newShortcut = DeserializeKeys(arg.Value);
                                }
                                else // for backward compatibility
                                {
                                    newShortcut = new[] { (ShortcutKeys) (Keys) Enum.Parse(typeof(Keys), arg.Value) };
                                }
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError($"{{ \"{arg.Key}\" = \"{arg.Value}\" }}\n{ex.Message}", ex);
                            }
                            customShortcuts.RemoveAt(i);
                            count--;
                            break;
                        }
                    }
                    item.Custom = new List<ShortcutKeys>(newShortcut);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves the list of custom shortcuts to a file.
        /// </summary>
        internal static void SaveCustomShortcuts(string file, IEnumerable<IShortcutItem> items)
        {
            try
            {
                var shortcuts = new List<Argument>()
                {
                    new Argument(VersionKey, FileVersionCurrent.ToString())
                };
                foreach (var item in items)
                {
                    if (item.IsModified)
                    {
                        shortcuts.Add(new Argument(item.Id, SerializeKeys(item.Custom.ToArray())));
                    }
                }
                ObjectSerializer.Serialize(file, shortcuts);
            }
            catch (Exception e)
            {
                ErrorManager.ShowError(e);
            }
        }

        /// <summary>
        /// Converts an array of shortcut keys into a string.
        /// </summary>
        internal static string SerializeKeys(ShortcutKeys[] keys)
        {
            if (keys.Length > 0)
            {
                string buffer = keys[0].ToString();
                for (int i = 1; i < keys.Length; i++)
                {
                    buffer += ShortcutKeyDelimiter + keys[i];
                }
                return buffer;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts a string into an array of shortcut keys.
        /// </summary>
        internal static ShortcutKeys[] DeserializeKeys(string data)
        {
            string[] strings = data.Split(new[] { ShortcutKeyDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            var keys = new ShortcutKeys[strings.Length];
            for (int i = 0; i < strings.Length; i++)
            {
                keys[i] = ShortcutKeys.Parse(strings[i]);
            }
            return keys;
        }
    }

    internal class ShortcutItem
    {
        internal readonly string Id;
        internal ShortcutKeys[] Default;
        internal ShortcutKeys[] Custom;
        internal ToolStripItem[] Items;

        internal ShortcutItem(string id, ShortcutKeys[] defaultShortcut, ToolStripItem[] items)
        {
            Id = id;
            Default = defaultShortcut;
            Custom = defaultShortcut;
            Items = items;
        }

        internal bool IsModified
        {
            get
            {
                if (Default.Length != Custom.Length)
                {
                    return true;
                }
                // This does a sequential equality check, meaning if the order is different, it's considered modified
                for (int i = 0; i < Default.Length; i++)
                {
                    if (Default[i] != Custom[i])
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    internal interface IShortcutItem
    {
        string Id { get; }
        ShortcutKeys[] Default { get; }
        List<ShortcutKeys> Custom { get; set; }
        bool IsModified { get; }
    }
}
