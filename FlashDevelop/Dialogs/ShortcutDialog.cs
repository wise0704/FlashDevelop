using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using FlashDevelop.Managers;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDevelop.Dialogs
{
    public class ShortcutDialog : SmartForm
    {
        private const char ViewConflictsKey = '?';
        private const char ViewCustomKey = '*';

        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.ColumnHeader idHeader;
        private System.Windows.Forms.ColumnHeader keyHeader;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Label pressNewLabel;
        private System.Windows.Forms.TextBox shortcutTextBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripMenuItem revertToDefault;
        private System.Windows.Forms.ToolStripMenuItem revertAllToDefault;

        private ConflictsManager conflictsManager;
        private ShortcutListItem[] shortcutListItems;
        private ShortcutKeys inputKeys;

        private ShortcutDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "d7837615-77ac-425e-80cd-65515d130913";
            this.InitializeComponent();
            this.InitializeContextMenu();
            this.InitializeLocalization();
            this.InitializeGraphics();
            this.InitializeShortcutListItems();
            this.ApplyScaling();
            this.PopulateListView();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.searchLabel = new System.Windows.Forms.Label();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.idHeader = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.keyHeader = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.listView = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.pressNewLabel = new System.Windows.Forms.Label();
            this.shortcutTextBox = new System.Windows.Forms.TextBox();
            this.addButton = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.importButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(13, 9);
            this.searchLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(0, 20);
            this.searchLabel.TabIndex = 13;
            // 
            // filterTextBox
            // 
            this.filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterTextBox.Location = new System.Drawing.Point(12, 32);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(756, 27);
            this.filterTextBox.TabIndex = 0;
            this.filterTextBox.TextChanged += new System.EventHandler(this.FilterTextBox_TextChanged);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.Location = new System.Drawing.Point(774, 28);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(35, 35);
            this.clearButton.TabIndex = 1;
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // idHeader
            // 
            this.idHeader.Width = 350;
            // 
            // keyHeader
            // 
            this.keyHeader.Width = 239;
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.idHeader,
            this.keyHeader});
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(12, 69);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(797, 425);
            this.listView.SmallImageList = this.imageList;
            this.listView.TabIndex = 2;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListView_ItemSelectionChanged);
            this.listView.ClientSizeChanged += new System.EventHandler(this.ListView_ClientSizeChanged);
            this.listView.DoubleClick += new System.EventHandler(this.ListView_DoubleClick);
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListView_KeyDown);
            this.listView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ListView_KeyPress);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // comboBox
            // 
            this.comboBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox.IntegralHeight = false;
            this.comboBox.Location = new System.Drawing.Point(12, 500);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(691, 28);
            this.comboBox.TabIndex = 3;
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(709, 500);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(100, 28);
            this.removeButton.TabIndex = 4;
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // pressNewLabel
            // 
            this.pressNewLabel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pressNewLabel.AutoSize = true;
            this.pressNewLabel.Location = new System.Drawing.Point(8, 531);
            this.pressNewLabel.Name = "pressNewLabel";
            this.pressNewLabel.Size = new System.Drawing.Size(0, 20);
            this.pressNewLabel.TabIndex = 12;
            // 
            // shortcutTextBox
            // 
            this.shortcutTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shortcutTextBox.Enabled = false;
            this.shortcutTextBox.Location = new System.Drawing.Point(12, 554);
            this.shortcutTextBox.Name = "shortcutTextBox";
            this.shortcutTextBox.Size = new System.Drawing.Size(691, 27);
            this.shortcutTextBox.TabIndex = 5;
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(709, 554);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(100, 28);
            this.addButton.TabIndex = 6;
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox.Location = new System.Drawing.Point(12, 595);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(21, 25);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 11;
            this.pictureBox.TabStop = false;
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabel.Location = new System.Drawing.Point(40, 584);
            this.infoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(560, 49);
            this.infoLabel.TabIndex = 10;
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // importButton
            // 
            this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.importButton.Location = new System.Drawing.Point(607, 595);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(35, 35);
            this.importButton.TabIndex = 7;
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exportButton.Location = new System.Drawing.Point(648, 595);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(35, 35);
            this.exportButton.TabIndex = 8;
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(689, 595);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(120, 35);
            this.closeButton.TabIndex = 9;
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // updateTimer
            // 
            this.updateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // ShortcutDialog
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(821, 642);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.shortcutTextBox);
            this.Controls.Add(this.pressNewLabel);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.filterTextBox);
            this.Controls.Add(this.searchLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(527, 359);
            this.Name = "ShortcutDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShortcutDialog_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShortcutDialog_FormClosed);
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the <see cref="ListView"/> context menu.
        /// </summary>
        private void InitializeContextMenu()
        {
            this.revertToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertToDefault"), null, this.RevertToDefault_Click);
            this.revertAllToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertAllToDefault"), null, this.RevertAllToDefault_Click);

            this.listView.ContextMenuStrip = new ContextMenuStrip()
            {
                Font = Globals.Settings.DefaultFont,
                ImageScalingSize = ScaleHelper.Scale(new Size(16, 16)),
                Renderer = new DockPanelStripRenderer(false, false)
            };
            this.listView.ContextMenuStrip.Items.AddRange(new[]
            {
                this.revertToDefault,
                this.revertAllToDefault,
            });
            this.listView.ContextMenuStrip.Opening += this.ContextMenuStrip_Opening;
        }

        /// <summary>
        /// Applies the localized texts to the form.
        /// </summary>
        private void InitializeLocalization()
        {
            var tooltip = new ToolTip(this.components);
            tooltip.SetToolTip(this.importButton, TextHelper.GetStringWithoutMnemonics("Label.Import"));
            tooltip.SetToolTip(this.exportButton, TextHelper.GetStringWithoutMnemonics("Label.Export"));
            this.idHeader.Text = TextHelper.GetString("Label.Command");
            this.keyHeader.Text = TextHelper.GetString("Label.Shortcut");
            this.infoLabel.Text = TextHelper.GetString("Info.ShortcutEditInfo");
            this.closeButton.Text = TextHelper.GetString("Label.Close");
            this.removeButton.Text = TextHelper.GetString("Label.Remove");
            this.pressNewLabel.Text = TextHelper.GetString("Info.PressNewShortcut");
            this.addButton.Text = TextHelper.GetString("Label.Add");
            this.searchLabel.Text = TextHelper.GetString("Label.ShortcutSearch");
            this.Text = " " + TextHelper.GetString("Title.Shortcuts");
        }

        /// <summary>
        /// Initializes the graphics.
        /// </summary>
        private void InitializeGraphics()
        {
            this.listView.GetType().InvokeMember(nameof(this.DoubleBuffered), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, this.listView, new object[] { true });
            this.imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.imageList.Images.Add(Globals.MainForm.FindImage("545", false));
            this.imageList.Images.Add(Globals.MainForm.FindImage("545|6|3|3", false));
            this.clearButton.Image = Globals.MainForm.FindImage16("153", false);
            this.pictureBox.Image = Globals.MainForm.FindImage16("229", false);
            this.importButton.Image = Globals.MainForm.FindImage16("55|1|3|3", false);
            this.exportButton.Image = Globals.MainForm.FindImage16("55|9|3|3", false);
            this.revertToDefault.Image = Globals.MainForm.FindImage("69", false);
            this.revertAllToDefault.Image = Globals.MainForm.FindImage("224", false);
        }

        /// <summary>
        /// Initialize the full shortcut list.
        /// </summary>
        private void InitializeShortcutListItems()
        {
            this.conflictsManager = new ConflictsManager();
            var collection = ShortcutManager.RegisteredItems;
            this.shortcutListItems = new ShortcutListItem[collection.Count];
            int counter = 0;
            foreach (var item in collection)
            {
                this.shortcutListItems[counter++] = new ShortcutListItem(item)
                {
                    UseItemStyleForSubItems = false
                };
            }
            Array.Sort(this.shortcutListItems, new ShorcutListItemComparer());
            this.UpdateAllShortcutsConflicts();
        }

        /// <summary>
        /// Applies additional scaling to controls in order to support HDPI.
        /// </summary>
        private void ApplyScaling()
        {
            this.idHeader.Width = ScaleHelper.Scale(this.idHeader.Width);
            this.keyHeader.Width = ScaleHelper.Scale(this.keyHeader.Width);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the shortcut dialog.
        /// </summary>
        public static new void Show()
        {
            ShortcutDialog shortcutDialog = null;
            try
            {
                shortcutDialog = new ShortcutDialog();
                Globals.MainForm.ThemeControls(shortcutDialog);
                shortcutDialog.filterTextBox.Select();
                shortcutDialog.ShowDialog(Globals.MainForm);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
            finally
            {
                shortcutDialog?.Dispose();
            }
        }

        /// <summary>
        /// Sets the filter text box text and immediately populates the list view.
        /// </summary>
        private void FilterListView(string filter)
        {
            this.filterTextBox.TextChanged -= this.FilterTextBox_TextChanged;
            this.filterTextBox.Text = filter;
            this.filterTextBox.TextChanged += this.FilterTextBox_TextChanged;
            this.PopulateListView();
        }

        /// <summary>
        /// Populates the shortcut list view.
        /// </summary>
        private void PopulateListView()
        {
            var selectedItem = this.listView.SelectedItems.Count > 0 ? this.listView.SelectedItems[0] : null;
            string filter = this.filterTextBox.Text;
            bool viewCustom = false;
            bool viewConflicts = false;
            ExtractFilterKeywords(ref filter, ref viewCustom, ref viewConflicts);
            this.listView.BeginUpdate();
            this.listView.Items.Clear();
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                var item = this.shortcutListItems[i];
                if (filter.Length == 0 ||
                    item.Id.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    item.KeysString.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (viewCustom && !item.IsModified) continue;
                    if (viewConflicts && !this.conflictsManager.HasConflicts(item)) continue;
                    this.listView.Items.Add(item);
                }
            }
            this.listView.EndUpdate();
            if (selectedItem != null && this.listView.Items.Count > 0)
            {
                int index = this.listView.Items.IndexOf(selectedItem);
                index = index >= 0 ? index : 0;
                this.listView.Items[index].Selected = true;
                this.listView.EnsureVisible(index);
            }
        }

        /// <summary>
        /// Allows users to enter a new shortcut.
        /// </summary>
        private void EnterNewShortcut()
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                this.shortcutTextBox.Select();
            }
        }

        /// <summary>
        /// Assign the new shortcut.
        /// </summary>
        private bool AssignNewShortcut(ShortcutListItem item, ShortcutKeys shortcut)
        {
            if (item.Custom.Contains(shortcut))
            {
                // Fail silently?
            }
            else
            {
                bool retry = false;
                if (this.conflictsManager.Contains(shortcut))
                {
                    string text = TextHelper.GetString("Info.ShortcutIsAlreadyUsed");
                    string caption = TextHelper.GetString("Title.WarningDialog");
                    switch (MessageBox.Show(this, text, " " + caption, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning))
                    {
                        case DialogResult.Abort:
                            return false;
                        case DialogResult.Retry:
                            retry = true;
                            break;
                    }
                }

                this.listView.BeginUpdate();
                item.Custom.Add(shortcut);
                item.UpdateText();
                int type = this.conflictsManager.Add(item, shortcut);
                this.listView.EndUpdate();

                if (retry)
                {
                    string filter;
                    if (type == 1)
                    {
                        filter = ViewConflictsKey + shortcut.ToString();
                    }
                    else
                    {
                        filter = ViewConflictsKey + ShortcutKeysConverter.ConvertToString(shortcut.First);
                    }
                    this.FilterListView(filter);
                    this.filterTextBox.SelectAll();
                }
            }

            return true;
        }

        /// <summary>
        /// Remove shortcut from the selected item.
        /// </summary>
        /// <param name="item"></param>
        private void RemoveShortcut(ShortcutListItem item, int shortcutIndex, ShortcutKeys shortcut)
        {
            this.listView.BeginUpdate();
            item.Custom.RemoveAt(shortcutIndex);
            item.UpdateText();
            this.conflictsManager.Remove(item, shortcut);
            this.listView.EndUpdate();
        }

        /// <summary>
        /// Reverts the selected items shortcut to default.
        /// </summary>
        private void RevertToDefault(ShortcutListItem item)
        {
            this.listView.BeginUpdate();
            while (item.Custom.Count > 0)
            {
                var keys = item.Custom[0];
                item.Custom.RemoveAt(0);
                this.conflictsManager.Remove(item, keys);
            }
            for (int i = 0; i < item.Default.Length; i++)
            {
                var keys = item.Default[i];
                item.Custom.Add(keys);
                this.conflictsManager.Add(item, keys);
            }
            item.UpdateText();
            this.listView.EndUpdate();
        }

        /// <summary>
        /// Reverts all visible shortcuts to their default value.
        /// </summary>
        private void RevertAllToDefault()
        {
            this.listView.BeginUpdate();
            foreach (ShortcutListItem item in this.listView.Items)
            {
                if (item.IsModified)
                {
                    this.RevertToDefault(item);
                }
            }
            this.listView.EndUpdate();
        }

        /// <summary>
        /// Update conflicts statuses of all shortcut items.
        /// </summary>
        private bool UpdateAllShortcutsConflicts()
        {
            this.conflictsManager.Clear();
            bool conflicts = false;
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                var item = this.shortcutListItems[i];
                bool itemHasConflicts = false;
                for (int j = 0; j < item.Custom.Count; j++)
                {
                    if (this.conflictsManager.Add(item, item.Custom[j], true) > 0)
                    {
                        itemHasConflicts = true;
                        conflicts = true;
                    }
                }
                if (!itemHasConflicts)
                {
                    UpdateItemDisplayStatus(item, false);
                }
            }
            return conflicts;
        }

        /// <summary>
        /// Display a warning message to show conflicts.
        /// </summary>
        private bool ShowConflictsPresent()
        {
            string text = TextHelper.GetString("Info.ShortcutConflictsPresent");
            string caption = TextHelper.GetString("Title.WarningDialog");
            if (MessageBox.Show(this, text, " " + caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                this.FilterListView(ViewConflictsKey.ToString());
                this.filterTextBox.SelectAll();
                this.filterTextBox.Select();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update the combo box to show all shortcuts of the selected item,
        /// enables/disables the remove button, clears the shortcut text box, and disables the add button.
        /// </summary>
        private void UpdateCurrentItemDetails()
        {
            this.comboBox.BeginUpdate();
            this.comboBox.Items.Clear();
            if (this.listView.SelectedItems.Count > 0)
            {
                foreach (var keys in ((ShortcutListItem) this.listView.SelectedItems[0]).Custom)
                {
                    this.comboBox.Items.Add(keys);
                }
            }
            this.comboBox.EndUpdate();
            if (this.comboBox.Items.Count > 0)
            {
                if (this.comboBox.SelectedIndex == -1)
                {
                    this.comboBox.SelectedIndex = 0;
                }
                this.removeButton.Enabled = true;
            }
            else
            {
                this.removeButton.Enabled = false;
            }
            this.shortcutTextBox.Clear();
            this.inputKeys = ShortcutKeys.None;
            this.addButton.Enabled = false;
        }

        /// <summary>
        /// Reads and removes filter keywords from the start of the filter.
        /// The order of the keywords is irrelevant.
        /// </summary>
        private static void ExtractFilterKeywords(ref string filter, ref bool viewCustom, ref bool viewConflicts)
        {
            if (!viewCustom && filter.StartsWith(ViewCustomKey))
            {
                filter = filter.Substring(1);
                viewCustom = true;
                ExtractFilterKeywords(ref filter, ref viewCustom, ref viewConflicts);
            }
            else if (!viewConflicts && filter.StartsWith(ViewConflictsKey))
            {
                filter = filter.Substring(1);
                viewConflicts = true;
                ExtractFilterKeywords(ref filter, ref viewCustom, ref viewConflicts);
            }
        }

        /// <summary>
        /// Updates the item display.
        /// <para/><see cref="FontStyle.Bold"/> - The item is modified.
        /// <para/><see cref="Color.DarkRed"/> - The item has conflicts.
        /// <para/><see cref="SystemColors.GrayText"/> - The item has no shortcut keys.
        /// </summary>
        private static void UpdateItemDisplayStatus(ShortcutListItem item, bool hasConflicts)
        {
            var fontStyle = item.IsModified ? FontStyle.Bold : FontStyle.Regular;
            item.SubItems[0].Font = new Font(Globals.Settings.DefaultFont, fontStyle);
            item.SubItems[1].Font = new Font(Globals.Settings.DefaultFont, fontStyle);
            if (hasConflicts)
            {
                item.ImageIndex = 1;
                item.SubItems[0].ForeColor = Color.DarkRed;
                item.SubItems[1].ForeColor = Color.DarkRed;
            }
            else
            {
                item.ImageIndex = 0;
                item.SubItems[0].ForeColor = SystemColors.ControlText;
                item.SubItems[1].ForeColor = item.Custom.Count == 0 ? SystemColors.GrayText : SystemColors.ControlText;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Ensures all key input combinations, including Alt+F4 and Enter, are passed to the shortcut text box if it is focused.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.shortcutTextBox.Focused)
            {
                var e = new KeyEventArgs(keyData);
                ShortcutTextBox_KeyDown(this.shortcutTextBox, e);
                if (e.Handled)
                {
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Restart the timer for updating the list.
        /// </summary>
        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            this.updateTimer.Stop();
            this.updateTimer.Start();
        }

        /// <summary>
        /// Update the list with filter.
        /// </summary>
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            this.updateTimer.Enabled = false;
            this.PopulateListView();
        }

        /// <summary>
        /// Clears the filter text field.
        /// </summary>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            this.FilterListView(string.Empty);
            this.filterTextBox.Select();
        }

        /// <summary>
        /// Raised when the client size of listView changes.
        /// </summary>
        private void ListView_ClientSizeChanged(object sender, EventArgs e)
        {
            this.listView.BeginUpdate();
            this.keyHeader.Width = this.listView.ClientSize.Width - this.idHeader.Width;
            this.listView.EndUpdate();
        }

        /// <summary>
        /// Assign a new valid shortcut when keys are pressed.
        /// </summary>
        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            this.EnterNewShortcut();
        }

        /// <summary>
        /// Updates the combo box, clears the new shortcut text box and disables the add button.
        /// </summary>
        private void ListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.UpdateCurrentItemDetails();
            this.shortcutTextBox.Enabled = this.listView.SelectedItems.Count > 0;
        }

        /// <summary>
        /// Handle key presses on the list view.
        /// </summary>
        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Back)
            {
                string text = this.filterTextBox.Text;
                if (text.Length > 0)
                {
                    this.filterTextBox.Text = text.Remove(text.Length - 1);
                }
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Handle key presses on the list view.
        /// </summary>
        private void ListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                this.filterTextBox.AppendText(e.KeyChar.ToString());
            }
            e.Handled = true;
        }

        /// <summary>
        /// Show the pressed shortcut key combination in the shortcut text box.
        /// </summary>
        private void ShortcutTextBox_KeyDown(object sedner, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.None:
                case Keys.ControlKey:
                case Keys.ShiftKey:
                case Keys.Menu:
                    break;
                default:
                    if (Globals.Settings.DisableExtendedShortcutKeys)
                    {
                        this.inputKeys = e.KeyData;
                    }
                    else
                    {
                        this.inputKeys = ShortcutKeysManager.UpdateShortcutKeys(this.inputKeys, e.KeyData);
                    }
                    this.shortcutTextBox.Text = this.inputKeys.ToString();
                    this.shortcutTextBox.SelectionStart = this.shortcutTextBox.TextLength;
                    this.addButton.Enabled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        /// <summary>
        /// Assign the current input keys to the current selected item, and updates the current item details.
        /// </summary>
        private void AddButton_Click(object sender, EventArgs e)
        {
            if (this.AssignNewShortcut((ShortcutListItem) this.listView.SelectedItems[0], this.inputKeys))
            {
                this.UpdateCurrentItemDetails();
            }
            else
            {
                this.inputKeys = ShortcutKeys.None;
            }
            this.shortcutTextBox.Select();
        }

        /// <summary>
        /// Removes the selected shortcut keys from the current selected item, and updates the current item details.
        /// </summary>
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            this.RemoveShortcut((ShortcutListItem) this.listView.SelectedItems[0], this.comboBox.SelectedIndex, (ShortcutKeys) this.comboBox.SelectedItem);
            this.UpdateCurrentItemDetails();
        }

        /// <summary>
        /// Raised when the context menu for the list view is opening.
        /// </summary>
        private void ContextMenuStrip_Opening(object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0 && ((ShortcutListItem) this.listView.SelectedItems[0]).IsModified)
            {
                this.revertToDefault.Enabled = true;
                this.revertAllToDefault.Enabled = true;
            }
            else
            {
                this.revertToDefault.Enabled = false;
                this.revertAllToDefault.Enabled = false;
                foreach (ShortcutListItem item in this.listView.Items)
                {
                    if (item.IsModified)
                    {
                        this.revertAllToDefault.Enabled = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Reverts the shortcut to default value.
        /// </summary>
        private void RevertToDefault_Click(object sender, EventArgs e)
        {
            this.RevertToDefault((ShortcutListItem) this.listView.SelectedItems[0]);
            this.UpdateCurrentItemDetails();
        }

        /// <summary>
        /// Reverts all visible shortcuts to their default value.
        /// </summary>
        private void RevertAllToDefault_Click(object sender, EventArgs e)
        {
            this.RevertAllToDefault();
            this.UpdateCurrentItemDetails();
        }

        /// <summary>
        /// Switch to a custom shortcut set.
        /// </summary>
        private void ImportButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda",
                InitialDirectory = PathHelper.ShortcutsDir,
                Title = " " + TextHelper.GetString("Title.OpenFileDialog")
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.listView.BeginUpdate();
                ShortcutManager.LoadCustomShortcuts(dialog.FileName, this.shortcutListItems);
                bool conflicts = this.UpdateAllShortcutsConflicts();
                this.listView.EndUpdate();
                if (conflicts)
                {
                    this.ShowConflictsPresent(); // Make sure the warning message shows up after listView is rendered.
                }
            }
            this.filterTextBox.Select();
        }

        /// <summary>
        /// Save the current shortcut set to a file.
        /// </summary>
        private void ExportButton_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".fda",
                Filter = TextHelper.GetString("Info.ArgumentFilter") + "|*.fda",
                InitialDirectory = PathHelper.ShortcutsDir,
                OverwritePrompt = true,
                Title = " " + TextHelper.GetString("Title.SaveFileDialog")
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ShortcutManager.SaveCustomShortcuts(dialog.FileName, this.shortcutListItems);
            }
            this.filterTextBox.Select();
        }

        /// <summary>
        /// Closes the shortcut dialog.
        /// </summary>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// When the form is about to close, checks for any conflicts.
        /// </summary>
        private void ShortcutDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                if (this.conflictsManager.HasConflicts(this.shortcutListItems[i]))
                {
                    e.Cancel = this.ShowConflictsPresent();
                    break;
                }
            }
        }

        /// <summary>
        /// When the form is closed, applies shortcuts.
        /// </summary>
        private void ShortcutDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                this.shortcutListItems[i].ApplyChanges();
            }
            Globals.MainForm.ApplyAllSettings();
            ShortcutManager.SaveCustomShortcuts();
        }

        #endregion

        #region ListViewComparer

        /// <summary>
        /// Defines a method that compares two <see cref="ShortcutListItem"/> objects.
        /// </summary>
        private sealed class ShorcutListItemComparer : IComparer<ShortcutListItem>
        {
            /// <summary>
            /// Compares two <see cref="ShortcutListItem"/> objects.
            /// </summary>
            int IComparer<ShortcutListItem>.Compare(ShortcutListItem x, ShortcutListItem y)
            {
                return StringComparer.Ordinal.Compare(x.Text, y.Text);
            }
        }

        #endregion

        #region ConflictsManager

        /// <summary>
        /// Manages all shortcut keys conflict logic.
        /// </summary>
        private sealed class ConflictsManager
        {
            private Dictionary<Keys, Dictionary<Keys, List<ShortcutListItem>>> list;

            /// <summary>
            /// Creates a new instance of <see cref="ConflictsManager"/>.
            /// </summary>
            internal ConflictsManager()
            {
                list = new Dictionary<Keys, Dictionary<Keys, List<ShortcutListItem>>>();
            }

            /// <summary>
            /// Adds the specified item with the shortcut keys and update conflict status.
            /// Returns <c>1</c> if direct conflicts, <c>2</c> if indirect conflicts, or <c>0</c> if no conflicts.
            /// </summary>
            internal int Add(ShortcutListItem item, ShortcutKeys keys, bool suppressUpdate = false)
            {
                Dictionary<Keys, List<ShortcutListItem>> first;
                List<ShortcutListItem> second;
                if (!this.list.TryGetValue(keys.First, out first))
                {
                    this.list.Add(keys.First, new Dictionary<Keys, List<ShortcutListItem>>()
                    {
                        [keys.Second] = new List<ShortcutListItem>() { item }
                    });
                }
                else if (!first.TryGetValue(keys.Second, out second))
                {
                    first.Add(keys.Second, new List<ShortcutListItem>() { item });

                    if (keys.Second == Keys.None)
                    {
                        // Indirect conflict with extended shortcuts that share the same first keys
                        foreach (var second2 in first.Values)
                        {
                            for (int i = 0; i < second2.Count; i++)
                            {
                                UpdateItemDisplayStatus(second2[i], true);
                            }
                        }
                        return 2;
                    }
                    else if (first.ContainsKey(Keys.None))
                    {
                        // Indirect conflict with simple shortcuts that share the same first keys
                        UpdateItemDisplayStatus(item, true);
                        return 2;
                    }
                }
                else
                {
                    second.Add(item);

                    // Direct conflict with two shortcuts that share the same keys
                    for (int i = 0; i < second.Count; i++)
                    {
                        UpdateItemDisplayStatus(second[i], true);
                    }
                    return 1;
                }

                if (!suppressUpdate)
                {
                    UpdateItemDisplayStatus(item, this.HasConflicts(item));
                }
                return 0;
            }

            /// <summary>
            /// Clears the list of items managed by this <see cref="ConflictsManager"/>. Conflict status of items are not modified.
            /// </summary>
            internal void Clear()
            {
                this.list.Clear();
            }

            /// <summary>
            /// Checks whether there is an item that will conflict with the specified keys.
            /// </summary>
            internal bool Contains(ShortcutKeys keys)
            {
                Dictionary<Keys, List<ShortcutListItem>> first;
                if (this.list.TryGetValue(keys.First, out first))
                {
                    if (first.ContainsKey(keys.Second))
                    {
                        // Direct match with a shortcut that has the same keys
                        return true;
                    }
                    else if (keys.Second == Keys.None)
                    {
                        // Indirect match with an extended shortcut that has the same first keys
                        return true;
                    }
                    else if (first.ContainsKey(Keys.None))
                    {
                        // Indirect match with a simple shortcut that has the same first keys
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Checks whether the specified item has conflicts.
            /// </summary>
            internal bool HasConflicts(ShortcutListItem item)
            {
                for (int i = 0; i < item.Custom.Count; i++)
                {
                    var keys = item.Custom[i];
                    var first = this.list[keys.First];
                    if (first[keys.Second].Count > 1)
                    {
                        // Direct conflict with shortcuts that share the same keys
                        return true;
                    }
                    else if (keys.Second == Keys.None ? first.Count > 1 : first.ContainsKey(Keys.None))
                    {
                        // Indirect conflict with simple/extended shortcuts that share the same first keys
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Removes the specified item with the shortcut keys. Conflict status is not updated.
            /// </summary>
            internal void Remove(ShortcutListItem item, ShortcutKeys keys)
            {
                var first = this.list[keys.First];
                var second = first[keys.Second];

                second.Remove(item);
                if (second.Count == 0)
                {
                    first.Remove(keys.Second);
                    if (first.Count == 0)
                    {
                        this.list.Remove(keys.First);
                    }
                    else
                    {
                        if (keys.Second == Keys.None)
                        {
                            // Had an indirect conflict with extended shortcuts that share the same first keys
                            foreach (var second2 in first.Values)
                            {
                                if (second2.Count == 1)
                                {
                                    UpdateItemDisplayStatus(second2[0], this.HasConflicts(second2[0]));
                                }
                            }
                        }
                        else if (first.TryGetValue(Keys.None, out second))
                        {
                            // Had an indirect conflict with simple shortcuts that share the same first keys
                            if (first.Count == 1 || second.Count == 1)
                            {
                                UpdateItemDisplayStatus(second[0], this.HasConflicts(second[0]));
                            }
                        }
                    }
                }
                else
                {
                    // Had a direct conflict with two shortcuts that share the same keys
                    if (second.Count == 1)
                    {
                        UpdateItemDisplayStatus(second[0], this.HasConflicts(second[0]));
                    }
                }

                UpdateItemDisplayStatus(item, this.HasConflicts(item));
            }
        }

        #endregion

        #region ShortcutListItem

        /// <summary>
        /// Represents a visual representation of a <see cref="ShortcutItem"/> object.
        /// </summary>
        private sealed class ShortcutListItem : ListViewItem, IShortcutItem
        {
            private ShortcutItem item;
            private List<ShortcutKeys> custom;

            /// <summary>
            /// Creates a new instance of <see cref="ShortcutListItem"/> with an associated <see cref="ShortcutItem"/>.
            /// </summary>
            internal ShortcutListItem(ShortcutItem shortcutItem)
            {
                this.item = shortcutItem;
                this.custom = new List<ShortcutKeys>(this.Item.Custom);
                this.Name = this.Text = this.Id;
                this.SubItems.Add(this.KeysString);
            }

            /// <summary>
            /// Gets the associated <see cref="ShortcutItem"/> object.
            /// </summary>
            internal ShortcutItem Item
            {
                get { return this.item; }
            }

            /// <summary>
            /// Gets the ID of the associated <see cref="ShortcutItem"/>.
            /// </summary>
            public string Id
            {
                get { return this.Item.Id; }
            }

            /// <summary>
            /// Gets the default shortcut keys.
            /// </summary>
            public ShortcutKeys[] Default
            {
                get { return this.Item.Default; }
            }

            /// <summary>
            /// Gets or sets the custom shortcut keys.
            /// </summary>
            public List<ShortcutKeys> Custom
            {
                get { return this.custom; }
                set
                {
                    this.custom = value;
                    UpdateText();
                }
            }

            /// <summary>
            /// Gets the modification status of the shortcut.
            /// </summary>
            public bool IsModified
            {
                get
                {
                    if (this.Default.Length != this.Custom.Count)
                    {
                        return true;
                    }
                    // This does a sequential equality check, meaning if the order is different, it's considered modified
                    for (int i = 0; i < this.Default.Length; i++)
                    {
                        if (this.Default[i] != this.Custom[i])
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            /// <summary>
            /// Gets the string representation of the custom shortcut keys.
            /// </summary>
            internal string KeysString
            {
                get
                {
                    if (this.Custom.Count == 0)
                    {
                        return ShortcutKeys.None.ToString();
                    }
                    else
                    {
                        string buffer = this.Custom[0].ToString();
                        for (int i = 1; i < this.Custom.Count; i++)
                        {
                            buffer += "; " + this.Custom[i];
                        }
                        return buffer;
                    }
                }
            }

            /// <summary>
            /// Updates the sub item text.
            /// </summary>
            internal void UpdateText()
            {
                this.SubItems[1].Text = this.KeysString;
            }

            /// <summary>
            /// Apply changes made to this instance to the associated <see cref="ShortcutItem"/>.
            /// </summary>
            internal void ApplyChanges()
            {
                this.Item.Custom = this.Custom.ToArray();
            }
        }

        #endregion

    }

}
