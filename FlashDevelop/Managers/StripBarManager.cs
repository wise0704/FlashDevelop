using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDevelop.Managers
{
    internal static class StripBarManager
    {
        internal static readonly List<ToolStripItem> Items = new List<ToolStripItem>();

        /// <summary>
        /// Finds the tool or menu strip item by name or shortcut command id.
        /// </summary>
        internal static ToolStripItem FindMenuItem(string name)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Name == name)
                {
                    return Items[i];
                }
            }

            return Globals.MainForm.GetShortcutItem(name);
        }

        /// <summary>
        /// Finds the tool or menu strip items by name or shortcut command id.
        /// </summary>
        internal static List<ToolStripItem> FindMenuItems(string name)
        {
            var found = new List<ToolStripItem>();
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Name == name)
                {
                    found.Add(Items[i]);
                }
            }

            var item = ShortcutManager.GetRegisteredItem(name);
            if (item != null)
            {
                found.AddRange(item.Items);
            }

            return found;
        }

        /// <summary>
        /// Gets a menu strip from the specified XML file.
        /// </summary>
        internal static MenuStrip GetMenuStrip(string file)
        {
            var menuStrip = new MenuStrip();
            menuStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            FillMenuItems(menuStrip.Items, XmlHelper.LoadXmlDocument(file));
            return menuStrip;
        }

        /// <summary>
        /// Gets a tool strip from the specified XML file.
        /// </summary>
        internal static ToolStrip GetToolStrip(string file)
        {
            var toolStrip = new ToolStripEx();
            toolStrip.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            FillToolItems(toolStrip.Items, XmlHelper.LoadXmlDocument(file));
            return toolStrip;
        }

        /// <summary>
        /// Gets a context menu strip from the specified XML file.
        /// </summary>
        internal static ContextMenuStrip GetContextMenu(string file)
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            FillMenuItems(contextMenu.Items, XmlHelper.LoadXmlDocument(file));
            return contextMenu;
        }

        /// <summary>
        /// Adds tool strip menu items to the specified tool strip.
        /// </summary>
        private static void FillMenuItems(ToolStripItemCollection items, XmlNode xmlNode)
        {
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "menu":
                        GenerateRuntimeGeneratedXmlData(node);
                        var menu = GetMenu(node);
                        items.Add(menu); // Add item first to get the correct id
                        CreateKeyId(menu); // Create id first to get the correct children id's
                        FillMenuItems(menu.DropDown.Items, node);
                        break;

                    case "separator":
                        var separator = GetSeparator(node);
                        items.Add(separator);
                        break;

                    case "button":
                        var button = GetMenuItem(node);
                        items.Add(button); // Add item first to get the correct id
                        CreateKeyId(button);
                        Globals.MainForm.RegisterShortcut(((ItemData) button.Tag).KeyId, button);
                        break;
                }
            }
        }

        /// <summary>
        /// Adds a tool strip item to the specified tool strip item collection.
        /// </summary>
        private static void FillToolItems(ToolStripItemCollection items, XmlNode xmlNode)
        {
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "separator":
                        var separator = GetSeparator(node);
                        items.Add(separator);
                        break;

                    case "button":
                        var button = GetButtonItem(node);
                        items.Add(button); // Add item first to get the correct id
                        CreateKeyId(button);
                        Globals.MainForm.RegisterShortcut(((ItemData) button.Tag).KeyId, button);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a menu from the specified XML node.
        /// </summary>
        public static ToolStripMenuItem GetMenu(XmlNode node)
        {
            var item = new ToolStripMenuItem();
            string name = XmlHelper.GetAttribute(node, "name");
            string label = XmlHelper.GetAttribute(node, "label");
            string image = XmlHelper.GetAttribute(node, "image");
            string click = XmlHelper.GetAttribute(node, "click");
            string flags = XmlHelper.GetAttribute(node, "flags");
            string enabled = XmlHelper.GetAttribute(node, "enabled");
            string tag = XmlHelper.GetAttribute(node, "tag");
            item.Tag = new ItemData(label, null, tag, flags);
            item.Text = GetLocalizedString(label);
            if (name != null) item.Name = GetValidName(name); // Use the given name
            else item.Name = GetNameFromLabel(label); // Assign from id
            if (enabled != null) item.Enabled = Convert.ToBoolean(enabled);
            if (image != null) item.Image = Globals.MainForm.FindImage(image);
            if (click != null) item.Click += GetEventHandler(click);
            Items.Add(item);
            return item;
        }

        /// <summary>
        /// Gets a button item from the specified xml node
        /// </summary>
        private static ToolStripButton GetButtonItem(XmlNode node)
        {
            var item = new ToolStripButton();
            string name = XmlHelper.GetAttribute(node, "name");
            string label = XmlHelper.GetAttribute(node, "label");
            string image = XmlHelper.GetAttribute(node, "image");
            string click = XmlHelper.GetAttribute(node, "click");
            string flags = XmlHelper.GetAttribute(node, "flags");
            string enabled = XmlHelper.GetAttribute(node, "enabled");
            string keyId = XmlHelper.GetAttribute(node, "keyid");
            string tag = XmlHelper.GetAttribute(node, "tag");
            item.Tag = new ItemData(label, keyId, tag, flags);
            if (image != null) item.ToolTipText = TextHelper.RemoveMnemonicsAndEllipsis(GetLocalizedString(label));
            else item.Text = TextHelper.RemoveMnemonicsAndEllipsis(GetLocalizedString(label)); // Use text instead...
            if (name != null) item.Name = GetValidName(name); // Use the given name
            else item.Name = GetNameFromLabel(label); // Assign from id
            if (enabled != null) item.Enabled = Convert.ToBoolean(enabled);
            if (image != null) item.Image = Globals.MainForm.FindImage(image);
            if (click != null) item.Click += GetEventHandler(click);
            Items.Add(item);
            return item;
        }

        /// <summary>
        /// Gets a menu item from the specified XML node.
        /// </summary>
        private static ToolStripMenuItemEx GetMenuItem(XmlNode node)
        {
            var item = new ToolStripMenuItemEx();
            string name = XmlHelper.GetAttribute(node, "name");
            string label = XmlHelper.GetAttribute(node, "label");
            string image = XmlHelper.GetAttribute(node, "image");
            string click = XmlHelper.GetAttribute(node, "click");
            string flags = XmlHelper.GetAttribute(node, "flags");
            string enabled = XmlHelper.GetAttribute(node, "enabled");
            string shortcut = XmlHelper.GetAttribute(node, "shortcut");
            string keytext = XmlHelper.GetAttribute(node, "keytext");
            string keyId = XmlHelper.GetAttribute(node, "keyid");
            string tag = XmlHelper.GetAttribute(node, "tag");
            item.Tag = new ItemData(label, keyId, tag, flags);
            item.Text = GetLocalizedString(label);
            if (name != null) item.Name = GetValidName(name); // Use the given name
            else item.Name = GetNameFromLabel(label); // Assign from id
            if (enabled != null) item.Enabled = Convert.ToBoolean(enabled);
            if (image != null) item.Image = Globals.MainForm.FindImage(image);
            if (shortcut != null) item.ShortcutKeys = GetKeys(shortcut);
            if (keytext != null) item.ShortcutKeyDisplayString = GetKeyText(keytext);
            if (click != null) item.Click += GetEventHandler(click);
            Items.Add(item);
            return item;
        }

        /// <summary>
        /// Gets a separator item from the specified XML node.
        /// </summary>
        private static ToolStripSeparator GetSeparator(XmlNode node)
        {
            return new ToolStripSeparator();
        }

        /// <summary>
        /// Generates XML data at runtime.
        /// </summary>
        private static void GenerateRuntimeGeneratedXmlData(XmlNode node)
        {
            switch (XmlHelper.GetAttribute(node, "name"))
            {
                case "SyntaxMenu":
                    node.InnerXml = GetSyntaxMenuXml();
                    break;
            }
        }

        /// <summary>
        /// Gets the dynamic syntax menu XML (easy integration :)
        /// </summary>
        private static string GetSyntaxMenuXml()
        {
            string syntaxXml = "";
            string[] syntaxFiles = Directory.GetFiles(Path.Combine(PathHelper.SettingDir, "Languages"), "*.xml");
            for (int i = 0; i < syntaxFiles.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(syntaxFiles[i]);
                syntaxXml += $"<button label=\"{fileName}\" click=\"{nameof(MainForm.ChangeSyntax)}\" tag=\"{fileName.ToLower()}\" image=\"559\" flags=\"Enable:IsEditable+Check:IsEditable|IsActiveSyntax\" />";
            }
            return syntaxXml;
        }

        /// <summary>
        /// Creates a key id from the item name if not defined.
        /// </summary>
        private static void CreateKeyId(ToolStripItem item)
        {
            var data = (ItemData) item.Tag;
            if (string.IsNullOrEmpty(data.KeyId))
            {
                if (item.OwnerItem == null)
                {
                    data.KeyId = GetKeyIdFromName(item.Name);
                }
                else
                {
                    data.KeyId = ((ItemData) item.OwnerItem.Tag).KeyId + "." + GetKeyIdFromName(item.Name);
                }
            }
        }

        /// <summary>
        /// Removes "Menu" from the end of the specified text.
        /// </summary>
        private static string GetKeyIdFromName(string text)
        {
            if (text.EndsWithOrdinal("Menu"))
            {
                return text.Remove(text.Length - 4);
            }
            return text;
        }

        /// <summary>
        /// Removes "Label." from the beginning of the text.
        /// </summary>
        private static string GetNameFromLabel(string text)
        {
            if (text.StartsWithOrdinal("Label."))
            {
                return text.Substring(6);
            }
            return GetValidName(text);
        }

        /// <summary>
        /// Removes mnemonics, ellipses and spaces from the string.
        /// </summary>
        private static string GetValidName(string text)
        {
            return TextHelper.RemoveMnemonicsAndEllipsis(text).Replace('.', '_').Replace(' ', '_');
        }

        /// <summary>
        /// Gets a localized string if available
        /// </summary>
        private static string GetLocalizedString(string key)
        {
            try
            {
                return key.StartsWithOrdinal("Label.") ? TextHelper.GetString(key) : key;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a shortcut keys from a string
        /// </summary>
        private static ShortcutKeys GetKeys(string data)
        {
            try
            {
                return ShortcutKeys.Parse(GetKeyText(data));
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return ShortcutKeys.None;
            }
        }

        /// <summary>
        /// Gets a shortcut key string from a string
        /// </summary>
        private static string GetKeyText(string data)
        {
            return data.Replace('|', '+').Replace("Control", "Ctrl");
        }

        /// <summary>
        /// Gets a click handler from method name.
        /// </summary>
        private static EventHandler GetEventHandler(string method)
        {
            try
            {
                return (EventHandler) Delegate.CreateDelegate(typeof(EventHandler), Globals.MainForm, method);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }
    }
}
