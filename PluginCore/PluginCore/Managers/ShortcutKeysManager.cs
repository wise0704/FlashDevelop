using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PluginCore.Controls;

namespace PluginCore.Managers
{
    /// <summary>
    /// A static manager class for advanced shortcut keys.
    /// </summary>
    public static class ShortcutKeysManager
    {
        private static PropertyInfo p_IsAssignedToDropDownItem;
        private static PropertyInfo p_Properties;
        private static PropertyInfo p_Shortcuts;
        private static MethodInfo m_GetToplevelOwnerToolStrip;
        private static MethodInfo m_SetInteger;

        private static IList toolStrips;

        #region Properties

        internal static IList ToolStrips
        {
            get
            {
                if (toolStrips == null)
                {
                    toolStrips = ToolStripManager_ToolStrips();
                }
                return toolStrips;
            }
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Retrieves a value indicating whether a defined shortcut key is valid.
        /// </summary>
        /// <param name="shortcut">The shortcut key to test for validity.</param>
        public static bool IsValidShortcut(ShortcutKeys shortcut)
        {
            if (shortcut.IsExtended)
            {
                return IsValidExtendedShortcutFirst(shortcut.First) && IsValidExtendedShortcutSecond(shortcut.Second);
            }
            return IsValidSimpleShortcut(shortcut.First);
        }

        /// <summary>
        /// Retrieves a value indicating whether a defined shortcut key is a valid simple shortcut.
        /// </summary>
        /// <param name="keys">The shortcut key to test for validity.</param>
        public static bool IsValidSimpleShortcut(Keys keys)
        {
            if (keys == Keys.None)
            {
                return false;
            }
            var keyCode = keys & Keys.KeyCode;
            switch (keyCode)
            {
                case Keys.None:
                case Keys.ShiftKey:
                case Keys.ControlKey:
                case Keys.Menu:
                    return false;
                case Keys.Insert:
                case Keys.Delete:
                    return true;
            }
            switch (keys & Keys.Modifiers)
            {
                case Keys.None:
                case Keys.Shift:
                    return Keys.F1 <= keyCode && keyCode <= Keys.F24;
            }
            return true;
        }

        /// <summary>
        /// Retrieves a value indicating whether a defined shortcut key is a valid simple shortcut, excluding <see cref="Keys.Insert"/> and <see cref="Keys.Delete"/>.
        /// </summary>
        /// <param name="keys">The shortcut key to test for validity.</param>
        public static bool IsValidSimpleShortcutExclInsertDelete(Keys keys)
        {
            if (keys == Keys.None)
            {
                return false;
            }
            var keyCode = keys & Keys.KeyCode;
            switch (keyCode)
            {
                case Keys.None:
                case Keys.ShiftKey:
                case Keys.ControlKey:
                case Keys.Menu:
                    return false;
            }
            switch (keys & Keys.Modifiers)
            {
                case Keys.None:
                case Keys.Shift:
                    return Keys.F1 <= keyCode && keyCode <= Keys.F24;
            }
            return true;
        }

        /// <summary>
        /// Retrieves a value indicating whether a defined shortcut key is valid for the first part of an extended shortcut.
        /// </summary>
        /// <param name="first">The shortcut key to test for validity.</param>
        public static bool IsValidExtendedShortcutFirst(Keys first)
        {
            if (first == Keys.None)
            {
                return false;
            }
            switch (first & Keys.KeyCode)
            {
                case Keys.None:
                case Keys.ShiftKey:
                case Keys.ControlKey:
                case Keys.Menu:
                    return false;
            }
            switch (first & Keys.Modifiers)
            {
                case Keys.None:
                case Keys.Shift:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Retrieves a value indicating whether a defined shortcut key is valid for the second part of an extended shortcut.
        /// </summary>
        /// <param name="second">The shortcut key to test for validity.</param>
        public static bool IsValidExtendedShortcutSecond(Keys second)
        {
            if (second == Keys.None)
            {
                return false;
            }
            switch (second & Keys.KeyCode)
            {
                case Keys.None:
                case Keys.ShiftKey:
                case Keys.ControlKey:
                case Keys.Menu:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Processes a command key.
        /// </summary>
        /// <param name="m">A <see cref="Message"/>, passed by reference, that represents the window message to process.</param>
        /// <param name="keyData">A <see cref="ShortcutKeys"/> value that represents the key to process.</param>
        private static bool ProcessCmdKey(ref Message m, ShortcutKeys keyData)
        {
            if (IsValidShortcut(keyData))
            {
                return ProcessShortcut(ref m, keyData);
            }
            return false;
        }

        private static bool ProcessShortcut(ref Message m, ShortcutKeys shortcut)
        {
            if (!IsThreadUsingToolStrips())
            {
                return false;
            }

            var activeControl = Control.FromChildHandle(m.HWnd);
            var activeControlInChain = activeControl;

            if (activeControlInChain != null)
            {
                do
                {
                    if (activeControlInChain.ContextMenuStrip != null)
                    {
                        var activeControlInChain_ContextMenuStrip_Shortcuts = activeControlInChain.ContextMenuStrip.Shortcuts();
                        if (activeControlInChain_ContextMenuStrip_Shortcuts.ContainsKey(shortcut))
                        {
                            var item = activeControlInChain_ContextMenuStrip_Shortcuts[shortcut] as ToolStripMenuItemEx;
                            if (item != null && item.ProcessCmdKeyInternal(ref m, shortcut))
                            {
                                return true;
                            }
                        }
                    }
                    activeControlInChain = activeControlInChain.Parent;
                }
                while (activeControlInChain != null);

                if (activeControlInChain != null)
                {
                    activeControl = activeControlInChain;
                }

                bool handled = false;
                bool needsPrune = false;

                for (int i = 0, count = toolStrips.Count; i < count; i++)
                {
                    var toolStrip = toolStrips[i] as ToolStrip;
                    bool isAssociatedContextMenu = false;
                    bool isDoublyAssignedContextMenuStrip = false;

                    if (toolStrip == null)
                    {
                        needsPrune = true;
                        continue;
                    }
                    if (toolStrip == activeControl.ContextMenuStrip)
                    {
                        continue;
                    }
                    var toolStrip_Shortcuts = toolStrip.Shortcuts();
                    if (toolStrip_Shortcuts.ContainsKey(shortcut))
                    {
                        if (toolStrip.IsDropDown)
                        {
                            var dropDown = toolStrip as ToolStripDropDown;
                            var toplevelContextMenu = dropDown.GetFirstDropDown() as ContextMenuStrip;

                            if (toplevelContextMenu != null)
                            {
                                isDoublyAssignedContextMenuStrip = toplevelContextMenu.IsAssignedToDropDownItem();
                                if (!isDoublyAssignedContextMenuStrip)
                                {
                                    if (toplevelContextMenu != activeControl.ContextMenuStrip)
                                    {
                                        continue;
                                    }
                                    isAssociatedContextMenu = true;
                                }
                            }
                        }

                        bool rootWindowsMatch = false;

                        if (!isAssociatedContextMenu)
                        {
                            var topMostToolStrip = toolStrip.GetToplevelOwnerToolStrip();
                            if (topMostToolStrip != null)
                            {
                                var rootWindowOfToolStrip = WindowsFormsUtils_GetRootHWnd(topMostToolStrip);
                                var rootWindowOfControl = WindowsFormsUtils_GetRootHWnd(activeControl);
                                rootWindowsMatch = rootWindowOfToolStrip.Handle == rootWindowOfControl.Handle;

                                if (rootWindowsMatch)
                                {
                                    var mainForm = Control.FromHandle(rootWindowOfControl.Handle) as Form;
                                    if (mainForm != null && mainForm.IsMdiContainer)
                                    {
                                        var toolStripForm = topMostToolStrip.FindForm();
                                        if (toolStripForm != mainForm && toolStripForm != null)
                                        {
                                            rootWindowsMatch = toolStripForm == mainForm.ActiveMdiChild;
                                        }
                                    }
                                }
                            }
                        }

                        if (isAssociatedContextMenu || rootWindowsMatch || isDoublyAssignedContextMenuStrip)
                        {
                            var item = toolStrip_Shortcuts[shortcut] as ToolStripMenuItemEx;
                            if (item != null && item.ProcessCmdKeyInternal(ref m, shortcut))
                            {
                                handled = true;
                                break;
                            }
                        }
                    }
                }
                if (needsPrune)
                {
                    PruneToolStripList();
                }
                return handled;
            }

            return false;
        }

        private static bool IsThreadUsingToolStrips()
        {
            return ToolStrips != null && toolStrips.Count > 0;
        }

        private static void PruneToolStripList()
        {
            if (IsThreadUsingToolStrips())
            {
                for (int i = toolStrips.Count - 1; i >= 0; i--)
                {
                    if (toolStrips[i] == null)
                    {
                        toolStrips.RemoveAt(i);
                    }
                }
            }
        }

        #endregion

        #region Reflections

        // Reflection: System.Windows.Forms.ToolStripDropDown.IsAssignedToDropDownItem
        // Cache: p_IsAssignedToDropDownItem
        internal static bool IsAssignedToDropDownItem(this ToolStripDropDown @this)
        {
            if (p_IsAssignedToDropDownItem == null)
            {
                p_IsAssignedToDropDownItem = typeof(ToolStripDropDown).GetProperty("IsAssignedToDropDownItem", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return (bool) p_IsAssignedToDropDownItem.GetValue(@this, null);
        }

        // Reflection: System.Windows.Forms.ToolStripDropDown.OwnerToolStrip
        // Cache: N/A
        internal static ToolStrip OwnerToolStrip(this ToolStripDropDown @this)
        {
            var ownerItem = @this.OwnerItem; //=>ownerItem
            if (ownerItem != null)
            {
                var owner = ownerItem.GetCurrentParent(); //=>Parent.get=>ParentInternal.get
                if (owner != null)
                {
                    return owner;
                }
                if (ownerItem.Placement == ToolStripItemPlacement.Overflow && ownerItem.Owner != null)
                {
                    return ownerItem.Owner.OverflowButton.DropDown;
                }
                if (owner == null)
                {
                    return ownerItem.Owner;
                }
            }
            return null;
        }

        // Reflection: System.Windows.Forms.ToolStripItem.Properties : System.Windows.Forms.PropertyStore
        // Cache: p_Properties
        internal static object Properties(this ToolStripItem @this)
        {
            if (p_Properties == null)
            {
                p_Properties = typeof(ToolStripItem).GetProperty("Properties", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return (object) p_Properties.GetValue(@this, null);
        }

        // Reflection: System.Windows.Forms.ToolStrip.Shortcuts
        // Cache: p_Shortcuts
        internal static Hashtable Shortcuts(this ToolStrip @this)
        {
            if (p_Shortcuts == null)
            {
                p_Shortcuts = typeof(ToolStrip).GetProperty("Shortcuts", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return (Hashtable) p_Shortcuts.GetValue(@this, null);
        }

        // Reflection: System.Windows.Forms.ToolStripManager.ToolStrips : System.Windows.Forms.ClientUtils+WeakRefCollection
        // Cache: --
        internal static IList ToolStripManager_ToolStrips()
        {
            return (IList) typeof(ToolStripManager).InvokeMember("ToolStrips", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty, null, null, null);
        }

        // Reflection: System.Windows.Forms.ToolStripMenuItem.PropShortcutKeys
        // Cache: --
        internal static int ToolStripMenuItem_PropShortcutKeys()
        {
            return (int) typeof(ToolStripMenuItem).InvokeMember("PropShortcutKeys", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField, null, null, null);
        }

        // Reflection: System.Windows.Forms.ToolStripDropDown.GetFirstDropDown()
        // Cache: N/A
        internal static ToolStripDropDown GetFirstDropDown(this ToolStripDropDown @this)
        {
            var topmost = @this;
            var ownerDropDown = topmost.OwnerToolStrip() as ToolStripDropDown;
            while (ownerDropDown != null)
            {
                topmost = ownerDropDown;
                ownerDropDown = topmost.OwnerToolStrip() as ToolStripDropDown;
            }
            return topmost;
        }

        // Reflection: System.Windows.Forms.ToolStrip.GetToplevelOwnerToolStrip()
        // Cache: m_GetToplevelOwnerToolStrip
        internal static ToolStrip GetToplevelOwnerToolStrip(this ToolStrip @this)
        {
            if (m_GetToplevelOwnerToolStrip == null)
            {
                m_GetToplevelOwnerToolStrip = typeof(ToolStrip).GetMethod("GetToplevelOwnerToolStrip", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return (ToolStrip) m_GetToplevelOwnerToolStrip.Invoke(@this, null);
        }

        // Reflection: System.Windows.Forms.PropertyStore.SetInteger(Int32, Int32)
        // Cache: m_SetInteger
        internal static void Properties_SetInteger(this ToolStripMenuItemEx @this, int key, int value)
        {
            if (m_SetInteger == null)
            {
                m_SetInteger = @this.Properties.GetType().GetMethod("SetInteger", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            }
            m_SetInteger.Invoke(@this.Properties, new object[] { key, value });
        }

        // Reflection: System.Windows.Forms.WindowsFormsUtils.GetRootHWnd(Control)
        // Cache: N/A
        internal static HandleRef WindowsFormsUtils_GetRootHWnd(Control control)
        {
            return WindowsFormsUtils_GetRootHWnd(new HandleRef(control, control.Handle));
        }

        // Reflection: System.Windows.Forms.WindowsFormsUtils.GetRootHWnd(HandleRef)
        // Cache: N/A
        internal static HandleRef WindowsFormsUtils_GetRootHWnd(HandleRef hwnd)
        {
            var rootHwnd = Win32.GetAncestor(new HandleRef(hwnd, hwnd.Handle), Win32.GA_ROOT);
            return new HandleRef(hwnd.Wrapper, rootHwnd);
        }

        #endregion
    }
}
