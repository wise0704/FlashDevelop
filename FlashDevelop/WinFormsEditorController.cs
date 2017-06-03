using System;
using System.Windows.Forms;
using FlashDevelop.Controls;
using FlashDevelop.Dialogs;
using FlashDevelop.Managers;
using PluginCore;
using PluginCore.Managers;

namespace FlashDevelop
{
    // TODO: Share dialogs?
    // TODO: Proper shortcuts working without hacks, at least for some main ones like Ctrl+Shift+1, or Save
    class WinFormsEditorController : IEditorController
    {
        private QuickFind quickFind;
        private FRInFilesDialog frInFilesDialog;
        private FRInDocDialog frInDocDialog;
        private GoToDialog gotoDialog;

        private Form owner;
        public object Owner
        {
            get { return owner; }
        }

        public object QuickFindControl
        {
            get { return quickFind; }
        }

        public bool CanSearch
        {
            get { return this.quickFind.CanSearch; }
            set { this.quickFind.CanSearch = value; }
        }

        public WinFormsEditorController(Form owner)
        {
            this.owner = owner;
            this.quickFind = new QuickFind(this);
            this.gotoDialog = new GoToDialog(this);
            this.frInFilesDialog = new FRInFilesDialog(this);
            this.frInDocDialog = new FRInDocDialog(this);
        }

        public bool? ProcessCmdKey(Keys keyData)
        {
            /**
             * Notify plugins. Don't notify ControlKey or ShiftKey as it polls a lot
             */
            switch (keyData & Keys.KeyCode)
            {
                case Keys.ControlKey:
                case Keys.ShiftKey:
                case Keys.Menu:
                    return null;
            }
            KeyEvent ke = new KeyEvent(EventType.Keys, keyData);
            EventManager.DispatchEvent(this, ke);
            if (ke.Handled)
            {
                return true;
            }
            /**
             * Ignore basic control keys if sci doesn't have focus.
             */
            if (Globals.SciControl == null || !Globals.SciControl.IsFocus)
            {
                if (keyData == (Keys.Control | Keys.C)) return false;
                else if (keyData == (Keys.Control | Keys.V)) return false;
                else if (keyData == (Keys.Control | Keys.X)) return false;
                else if (keyData == (Keys.Control | Keys.A)) return false;
                else if (keyData == (Keys.Control | Keys.Z)) return false;
                else if (keyData == (Keys.Control | Keys.Y)) return false;
            }
            /**
             * Process special key combinations and allow "chaining" of 
             * Ctrl-Tab commands if you keep holding control down.
             */
            if ((keyData & Keys.Control) != 0)
            {
                Boolean sequentialTabbing = Globals.MainForm.AppSettings.SequentialTabbing;
                if ((keyData == (Keys.Control | Keys.Next)) || (keyData == (Keys.Control | Keys.Tab)))
                {
                    TabbingManager.TabTimer.Enabled = true;
                    if (keyData == (Keys.Control | Keys.Next) || sequentialTabbing)
                    {
                        TabbingManager.NavigateTabsSequentially(1);
                    }
                    else TabbingManager.NavigateTabHistory(1);
                    return true;
                }
                if ((keyData == (Keys.Control | Keys.Prior)) || (keyData == (Keys.Control | Keys.Shift | Keys.Tab)))
                {
                    TabbingManager.TabTimer.Enabled = true;
                    if (keyData == (Keys.Control | Keys.Prior) || sequentialTabbing)
                    {
                        TabbingManager.NavigateTabsSequentially(-1);
                    }
                    else TabbingManager.NavigateTabHistory(-1);
                    return true;
                }
            }
            string shortcutId = Globals.MainForm.GetShortcutItemId(keyData);
            switch (shortcutId)
            {
                case "SearchMenu.FindAndReplace":
                    this.ShowFindAndReplace();
                    return true;
                case "SearchMenu.FindAndReplaceInFiles":
                    this.ShowFindAndReplaceInFiles();
                    return true;
                case "SearchMenu.QuickFind":
                    this.ShowQuickFind();
                    return true;
                case "SearchMenu.QuickFindNext":
                    this.FindNext();
                    return true;
                case "SearchMenu.QuickFindPrevious":
                    this.FindPrevious();
                    return true;
                case "SearchMenu.GotoPositionOrLine":
                    this.ShowGoTo();
                    return true;
                default:
                    return null;
            }
        }

        public void ApplyAllSettings()
        {
            this.frInFilesDialog.UpdateSettings();
        }

        /// <summary>
        /// Sets the text to find globally
        /// </summary>
        public void SetFindText(object sender, string text)
        {
            if (sender != this.quickFind) this.quickFind.SetFindText(text);
            if (sender != this.frInDocDialog) this.frInDocDialog.SetFindText(text);
        }

        /// <summary>
        /// Sets the case setting to find globally
        /// </summary>
        public void SetMatchCase(object sender, bool matchCase)
        {
            if (sender != this.quickFind) this.quickFind.SetMatchCase(matchCase);
            if (sender != this.frInDocDialog) this.frInDocDialog.SetMatchCase(matchCase);
        }

        /// <summary>
        /// Sets the whole word setting to find globally
        /// </summary>
        public void SetWholeWord(object sender, bool wholeWord)
        {
            if (sender != this.quickFind) this.quickFind.SetWholeWord(wholeWord);
            if (sender != this.frInDocDialog) this.frInDocDialog.SetWholeWord(wholeWord);
        }

        /// <summary>
        /// Opens a goto dialog
        /// </summary>
        public void ShowGoTo()
        {
            if (!this.gotoDialog.Visible) this.gotoDialog.Show();
            else this.gotoDialog.Activate();
        }

        /// <summary>
        /// Displays the next result
        /// </summary>
        public void FindNext()
        {
            bool update = !Globals.Settings.DisableFindTextUpdating;
            bool simple = !Globals.Settings.DisableSimpleQuickFind && !this.quickFind.Visible;
            this.frInDocDialog.FindNext(true, update, simple);
        }

        /// <summary>
        /// Displays the previous result
        /// </summary>
        public void FindPrevious()
        {
            bool update = !Globals.Settings.DisableFindTextUpdating;
            bool simple = !Globals.Settings.DisableSimpleQuickFind && !this.quickFind.Visible;
            this.frInDocDialog.FindNext(false, update, simple);
        }

        /// <summary>
        /// Opens a find and replace dialog
        /// </summary>
        public void ShowFindAndReplace()
        {
            if (!this.frInDocDialog.Visible) this.frInDocDialog.Show();
            else
            {
                this.frInDocDialog.InitializeFindText();
                this.frInDocDialog.Activate();
            }
        }

        /// <summary>
        /// Opens a find and replace in files dialog
        /// </summary>
        public void ShowFindAndReplaceInFiles()
        {
            if (!this.frInFilesDialog.Visible) this.frInFilesDialog.Show();
            else
            {
                this.frInFilesDialog.UpdateFindText();
                this.frInFilesDialog.Activate();
            }
        }

        /// <summary>
        /// Opens a find and replace in files dialog with a location
        /// </summary>
        public void ShowFindAndReplaceInFilesFrom(string path)
        {
            if (!this.frInFilesDialog.Visible) this.frInFilesDialog.Show(); // Show first..
            else this.frInFilesDialog.Activate();
            this.frInFilesDialog.SetFindPath(path);
        }

        /// <summary>
        /// Shows the quick find control
        /// </summary>
        public void ShowQuickFind()
        {
            this.quickFind.ShowControl();
        }
    }
}
