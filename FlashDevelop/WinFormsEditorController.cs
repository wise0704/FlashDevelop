using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FlashDevelop.Dialogs;
using FlashDevelop.Managers;
using PluginCore;
using PluginCore.Managers;

namespace FlashDevelop
{
    class WinFormsEditorController : IEditorController
    {
        //private FRInFilesDialog frInFilesDialog;
        //private FRInDocDialog frInDocDialog;
        //private GoToDialog gotoDialog;

        private Form owner;
        public object Owner
        {
            get { return owner; }
        }

        public WinFormsEditorController(Form owner)
        {
            this.owner = owner;
            //this.gotoDialog = new GoToDialog();
            //this.frInFilesDialog = new FRInFilesDialog();
            //this.frInDocDialog = new FRInDocDialog();
        }

        public bool? ProcessCmdKey(Keys keyData)
        {
            /**
            * Notify plugins. Don't notify ControlKey or ShiftKey as it polls a lot
            */
            KeyEvent ke = new KeyEvent(EventType.Keys, keyData);
            Keys keyCode = keyData & Keys.KeyCode;
            if ((keyCode != Keys.ControlKey) && (keyCode != Keys.ShiftKey))
            {
                EventManager.DispatchEvent(this, ke);
            }
            if (!ke.Handled)
            {
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
                return null;
            }
            return true;
        }
    }
}
