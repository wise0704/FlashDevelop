using System.ComponentModel;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;

namespace ProjectManager.Helpers
{
    /// <summary>
    /// A simple form where a user can enter a text string.
    /// </summary>
    public class LineEntryDialog : Form, IShortcutHandlerForm
    {
        private string line;
        
        private System.Windows.Forms.TextBox lineBox;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label titleLabel;
        
        public LineEntryDialog(string captionText, string labelText, string defaultLine)
        {
            InitializeComponent();
            InititalizeLocalization(captionText, labelText);
            this.Font = PluginBase.Settings.DefaultFont;
            this.lineBox.Text = (defaultLine != null) ? defaultLine : string.Empty;
            this.lineBox.SelectAll();
            this.lineBox.Focus();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.lineBox = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.Location = new System.Drawing.Point(8, 8);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(266, 16);
            this.titleLabel.TabIndex = 3;
            this.titleLabel.Text = "Enter text:";
            // 
            // lineBox
            // 
            this.lineBox.Location = new System.Drawing.Point(10, 24);
            this.lineBox.Name = "lineBox";
            this.lineBox.Size = new System.Drawing.Size(260, 20);
            this.lineBox.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(68, 50);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 21);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(149, 50);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 21);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // LineEntryDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(282, 81);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lineBox);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LineEntryDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter Text";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        /// <summary>
        /// Gets the line entered by the user.
        /// </summary>
        public string Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Selects the specified range of text.
        /// </summary>
        public void SelectRange(int start, int length)
        {
            this.lineBox.Select(start, length);
        }

        private void InititalizeLocalization(string captionText, string labelText)
        {
            this.btnOK.Text = TextHelper.GetString("Label.OK");
            this.btnCancel.Text = TextHelper.GetString("Label.Cancel");
            this.titleLabel.Text = string.IsNullOrEmpty(labelText) ? TextHelper.GetString("Info.EnterText") : labelText;
            this.Text = " " + (string.IsNullOrEmpty(captionText) ? TextHelper.GetString("Title.EnterText") : captionText);
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            var cancelArgs = new CancelEventArgs(false);
            this.OnValidating(cancelArgs);
            if (!cancelArgs.Cancel)
            {
                this.line = this.lineBox.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        void IShortcutHandlerForm.HandleEvent(object sender, NotifyEvent e)
        {
            if (e.Type != EventType.ShortcutKey || !this.lineBox.Focused)
            {
                return;
            }
            var ske = (ShortcutKeyEvent) e;
            switch (ske.Command)
            {
                case "Edit.Copy":
                    this.lineBox.Copy();
                    e.Handled = true;
                    break;
                case "Edit.CopyLine":
                    {
                        int start = this.lineBox.SelectionStart;
                        int length = this.lineBox.SelectionLength;
                        this.lineBox.SelectAll();
                        this.lineBox.Copy();
                        this.lineBox.Select(start, length);
                    }
                    e.Handled = true;
                    break;
                case "Edit.Cut":
                    this.lineBox.Cut();
                    e.Handled = true;
                    break;
                case "Edit.CutLine":
                    this.lineBox.SelectAll();
                    this.lineBox.Cut();
                    e.Handled = true;
                    break;
                case "Edit.DeleteLine":
                    this.lineBox.Clear();
                    e.Handled = true;
                    break;
                case "Edit.Paste":
                    this.lineBox.Paste();
                    e.Handled = true;
                    break;
                case "Edit.SelectAll":
                    this.lineBox.SelectAll();
                    e.Handled = true;
                    break;
                case "Edit.ToLowercase":
                case "Edit.ToUppercase":
                    {
                        int start = this.lineBox.SelectionStart;
                        int length = this.lineBox.SelectionLength;
                        this.lineBox.SelectedText = ske.Command == "Edit.ToLowercase" ? this.lineBox.SelectedText.ToLower() : this.lineBox.SelectedText.ToUpper();
                        this.lineBox.Select(start, length);
                    }
                    e.Handled = true;
                    break;
                case "Edit.Undo":
                    this.lineBox.Undo();
                    e.Handled = true;
                    break;
            }
        }
    }

}
