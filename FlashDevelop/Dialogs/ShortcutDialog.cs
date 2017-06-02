using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
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
        private const char ViewChangedKey = '*';
        private const char ViewCustomKey = '!';
        private const char ViewConflictsKey = '?';

        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.ColumnHeader idHeader;
        private System.Windows.Forms.ColumnHeader keyHeader;
        private FlashDevelop.Dialogs.ShortcutDialog.ListViewEx listView;
        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Label pressNewLabel;
        private System.Windows.Forms.TextBox shortcutTextBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.ImageList imageList;
        private ToolStripMenuItem revertChanges;
        private ToolStripMenuItem revertToDefault;
        private ToolStripMenuItem revertAllToDefault;

        private ConflictsManager conflictsManager;
        private ShortcutListItem[] shortcutListItems;
        private ShortcutKey inputKeys;

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
                this.components.Dispose();
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
            this.listView = new FlashDevelop.Dialogs.ShortcutDialog.ListViewEx();
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
            this.saveButton = new System.Windows.Forms.Button();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(8, 9);
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
            this.listView.ShowGroups = true;
            this.listView.Size = new System.Drawing.Size(797, 425);
            this.listView.SmallImageList = this.imageList;
            this.listView.TabIndex = 2;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ClientSizeChanged += new System.EventHandler(this.ListView_ClientSizeChanged);
            this.listView.DoubleClick += new System.EventHandler(this.ListView_DoubleClick);
            this.listView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListView_ItemSelectionChanged);
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
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.saveButton.Location = new System.Drawing.Point(689, 595);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(120, 35);
            this.saveButton.TabIndex = 9;
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // updateTimer
            // 
            this.updateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // ShortcutDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 642);
            this.Controls.Add(this.saveButton);
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
            this.revertChanges = new ToolStripMenuItem(TextHelper.GetString("Label.Revert"), null, this.RevertChanges_Click);
            this.revertToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertToDefault"), null, this.RevertToDefault_Click);
            this.revertAllToDefault = new ToolStripMenuItem(TextHelper.GetString("Label.RevertAllToDefault"), null, this.RevertAllToDefault_Click);

            this.listView.ContextMenuStrip = new ContextMenuStrip(this.components)
            {
                Font = Globals.Settings.DefaultFont,
                ImageScalingSize = ScaleHelper.Scale(new Size(16, 16)),
                Renderer = new DockPanelStripRenderer(false, false)
            };
            this.listView.ContextMenuStrip.Items.AddRange(new[]
            {
                this.revertChanges,
                this.revertToDefault,
                this.revertAllToDefault,
            });
            this.listView.ContextMenuStrip.Opening += this.ContextMenuStrip_Opening;

            this.shortcutTextBox.ContextMenuStrip = new ContextMenuStrip(this.components);
        }

        /// <summary>
        /// Applies the localized texts to the form.
        /// </summary>
        private void InitializeLocalization()
        {
            var tooltip = new ToolTip(this.components);
            tooltip.SetToolTip(this.importButton, TextHelper.GetStringWithoutMnemonics("Label.Import"));
            tooltip.SetToolTip(this.exportButton, TextHelper.GetStringWithoutMnemonics("Label.Export"));
            this.Text = " " + TextHelper.GetString("Title.Shortcuts");
            this.idHeader.Text = TextHelper.GetString("Label.Command");
            this.keyHeader.Text = TextHelper.GetString("Label.Shortcut");
            this.searchLabel.Text = string.Format(TextHelper.GetString("Label.ShortcutSearch"), ViewChangedKey, ViewCustomKey, ViewConflictsKey);
            this.removeButton.Text = TextHelper.GetString("Label.Remove");
            this.pressNewLabel.Text = TextHelper.GetString("Info.PressNewShortcut");
            this.addButton.Text = TextHelper.GetString("Label.Add");
            this.infoLabel.Text = TextHelper.GetString("Info.ShortcutEditInfo");
            this.saveButton.Text = TextHelper.GetString("Label.Cancel");
        }

        /// <summary>
        /// Initializes the graphics.
        /// </summary>
        private void InitializeGraphics()
        {
            this.listView.GetType().InvokeMember(nameof(this.DoubleBuffered), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, this.listView, new object[] { true });
            this.imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.imageList.Images.AddRange(new[]
            {
                Globals.MainForm.FindImage("545", false),
                Globals.MainForm.FindImage("545|2|3|3", false),
                Globals.MainForm.FindImage("545|11|3|3", false),
                Globals.MainForm.FindImage("545|6|3|3", false),
            });
            this.clearButton.Image = Globals.MainForm.FindImage16("153", false);
            this.pictureBox.Image = Globals.MainForm.FindImage16("229", false);
            this.importButton.Image = Globals.MainForm.FindImage16("55|1|3|3", false);
            this.exportButton.Image = Globals.MainForm.FindImage16("55|9|3|3", false);
            this.revertChanges.Image = Globals.MainForm.FindImage("66", false);
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

        /// <summary>
        /// Apply undefined control theme colors.
        /// </summary>
        private void ThemeControls()
        {
            var backColor = Globals.MainForm.GetThemeColor("MenuStrip.BackColor", this.BackColor);
            var foreColor = Globals.MainForm.GetThemeColor("MenuStrip.ForeColor", SystemColors.ControlText);
            this.BackColor = backColor;
            this.searchLabel.ForeColor = foreColor;
            this.pressNewLabel.ForeColor = foreColor;
            this.infoLabel.ForeColor = foreColor;
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
                shortcutDialog.ThemeControls();
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
            bool viewModified = false;
            bool viewCustom = false;
            bool viewConflicts = false;
            ExtractFilterKeywords(ref filter, ref viewModified, ref viewCustom, ref viewConflicts);
            this.listView.BeginUpdate();
            this.listView.Items.Clear();
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                var item = this.shortcutListItems[i];
                if (filter.Length == 0 ||
                    item.Command.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    item.KeysString.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (viewModified && !item.IsModified) continue;
                    if (viewCustom && !item.IsCustomized) continue;
                    if (viewConflicts && !this.conflictsManager.HasConflicts(item)) continue;

                    int dotIndex = item.Text.IndexOf('.');
                    string groupName = dotIndex >= 0 ? item.Text.Remove(dotIndex) : "";
                    this.listView.AddToGroup(item, groupName);
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
        private bool AssignNewShortcut(ShortcutListItem item, ShortcutKey shortcut)
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
                        filter = ViewConflictsKey + ShortcutKeyConverter.ConvertToString(shortcut.First);
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
        private void RemoveShortcut(ShortcutListItem item, int shortcutIndex, ShortcutKey shortcut)
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
        private void RevertTo(ShortcutListItem item, ShortcutKey[] shortcuts)
        {
            this.listView.BeginUpdate();
            while (item.Custom.Count > 0)
            {
                var keys = item.Custom[0];
                item.Custom.RemoveAt(0);
                this.conflictsManager.Remove(item, keys);
            }
            for (int i = 0; i < shortcuts.Length; i++)
            {
                var keys = shortcuts[i];
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
                if (item.IsCustomized)
                {
                    this.RevertTo(item, item.Default);
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
            this.inputKeys = ShortcutKey.None;
            this.addButton.Enabled = false;
        }

        /// <summary>
        /// Updates the dialog result and text of the save button.
        /// </summary>
        private void UpdateSaveButton()
        {
            for (int i = 0; i < this.shortcutListItems.Length; i++)
            {
                if (this.shortcutListItems[i].IsModified)
                {
                    this.saveButton.DialogResult = DialogResult.OK;
                    this.saveButton.Text = TextHelper.GetString("Label.Save");
                    return;
                }
            }

            this.saveButton.DialogResult = DialogResult.Cancel;
            this.saveButton.Text = TextHelper.GetString("Label.Cancel");
        }

        /// <summary>
        /// Reads and removes filter keywords from the start of the filter.
        /// The order of the keywords is irrelevant.
        /// </summary>
        private static void ExtractFilterKeywords(ref string filter, ref bool viewModified, ref bool viewCustom, ref bool viewConflicts)
        {
            if (!viewModified && filter.StartsWith(ViewChangedKey))
            {
                filter = filter.Substring(1);
                viewModified = true;
                ExtractFilterKeywords(ref filter, ref viewModified, ref viewCustom, ref viewConflicts);
            }
            else if (!viewCustom && filter.StartsWith(ViewCustomKey))
            {
                filter = filter.Substring(1);
                viewCustom = true;
                ExtractFilterKeywords(ref filter, ref viewModified, ref viewCustom, ref viewConflicts);
            }
            else if (!viewConflicts && filter.StartsWith(ViewConflictsKey))
            {
                filter = filter.Substring(1);
                viewConflicts = true;
                ExtractFilterKeywords(ref filter, ref viewModified, ref viewCustom, ref viewConflicts);
            }
        }

        /// <summary>
        /// Updates the item display.
        /// </summary>
        private static void UpdateItemDisplayStatus(ShortcutListItem item, bool hasConflicts)
        {
            bool isModified = item.IsModified;
            bool isCustomized = item.IsCustomized;
            item.SubItems[0].Font = new Font(Globals.Settings.DefaultFont, (isModified ? FontStyle.Italic : FontStyle.Regular) | (isCustomized ? FontStyle.Bold : FontStyle.Regular));
            item.SubItems[1].Font = new Font(Globals.Settings.DefaultFont, hasConflicts ? FontStyle.Bold : FontStyle.Regular);
            if (hasConflicts)
            {
                item.ImageIndex = 3;
                item.SubItems[0].ForeColor = Color.DarkRed;
                item.SubItems[1].ForeColor = Color.DarkRed; // No light colour on conflicts
            }
            else if (isModified)
            {
                item.ImageIndex = 2;
                item.SubItems[0].ForeColor = Color.DarkGreen;
                item.SubItems[1].ForeColor = item.Custom.Count > 0 ? Color.DarkGreen : Color.DarkSeaGreen;
            }
            else if (isCustomized)
            {
                item.ImageIndex = 1;
                item.SubItems[0].ForeColor = Color.Black;
                item.SubItems[1].ForeColor = item.Custom.Count > 0 ? Color.Black : Color.Gray;
            }
            else
            {
                item.ImageIndex = 0;
                item.SubItems[0].ForeColor = Color.Black;
                item.SubItems[1].ForeColor = item.Custom.Count > 0 ? Color.Black : Color.Gray;
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
                        this.inputKeys += e.KeyData;
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
                this.UpdateSaveButton();
            }
            else
            {
                this.inputKeys = ShortcutKey.None;
            }
            this.shortcutTextBox.Select();
        }

        /// <summary>
        /// Removes the selected shortcut keys from the current selected item, and updates the current item details.
        /// </summary>
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            this.RemoveShortcut((ShortcutListItem) this.listView.SelectedItems[0], this.comboBox.SelectedIndex, (ShortcutKey) this.comboBox.SelectedItem);
            this.UpdateCurrentItemDetails();
            this.UpdateSaveButton();
        }

        /// <summary>
        /// Raised when the context menu for the list view is opening.
        /// </summary>
        private void ContextMenuStrip_Opening(object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                var item = (ShortcutListItem) this.listView.SelectedItems[0];
                this.revertChanges.Enabled = item.IsModified;

                if (item.IsCustomized)
                {
                    this.revertToDefault.Enabled = true;
                    this.revertAllToDefault.Enabled = true;
                    return;
                }
                else
                {
                    this.revertToDefault.Enabled = false;
                }
            }
            else
            {
                this.revertChanges.Enabled = false;
                this.revertToDefault.Enabled = false;
            }

            this.revertAllToDefault.Enabled = false;
            foreach (ShortcutListItem item in this.listView.Items)
            {
                if (item.IsCustomized)
                {
                    this.revertAllToDefault.Enabled = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Reverts the shortcut to current value.
        /// </summary>
        private void RevertChanges_Click(object sender, EventArgs e)
        {
            var item = (ShortcutListItem) this.listView.SelectedItems[0];
            this.RevertTo(item, item.Current);
            this.UpdateCurrentItemDetails();
            this.UpdateSaveButton();
        }

        /// <summary>
        /// Reverts the shortcut to default value.
        /// </summary>
        private void RevertToDefault_Click(object sender, EventArgs e)
        {
            var item = (ShortcutListItem) this.listView.SelectedItems[0];
            this.RevertTo(item, item.Default);
            this.UpdateCurrentItemDetails();
            this.UpdateSaveButton();
        }

        /// <summary>
        /// Reverts all visible shortcuts to their default value.
        /// </summary>
        private void RevertAllToDefault_Click(object sender, EventArgs e)
        {
            this.RevertAllToDefault();
            this.UpdateCurrentItemDetails();
            this.UpdateSaveButton();
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
                this.UpdateSaveButton();
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
        /// When the form is about to close, checks for any conflicts.
        /// </summary>
        private void ShortcutDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
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
        }

        /// <summary>
        /// When the form is closed, applies shortcuts.
        /// </summary>
        private void ShortcutDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                for (int i = 0; i < this.shortcutListItems.Length; i++)
                {
                    this.shortcutListItems[i].ApplyChanges();
                }
                Globals.MainForm.ApplyAllSettings();
                ShortcutManager.SaveCustomShortcuts();
            }
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
                this.list = new Dictionary<Keys, Dictionary<Keys, List<ShortcutListItem>>>();
            }

            /// <summary>
            /// Adds the specified item with the shortcut keys and update conflict status.
            /// Returns <c>1</c> if direct conflicts, <c>2</c> if indirect conflicts, or <c>0</c> if no conflicts.
            /// </summary>
            internal int Add(ShortcutListItem item, ShortcutKey keys, bool suppressUpdate = false)
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
                            if (second2.Count == 1)
                            {
                                UpdateItemDisplayStatus(second2[0], true);
                            }
                        }
                        return 2;
                    }
                    else if (first.TryGetValue(Keys.None, out second))
                    {
                        // Indirect conflict with simple shortcuts that share the same first keys
                        UpdateItemDisplayStatus(item, true);
                        if (second.Count == 1)
                        {
                            UpdateItemDisplayStatus(second[0], true);
                        }
                        return 2;
                    }
                }
                else
                {
                    second.Add(item);

                    // Direct conflict with two shortcuts that share the same keys
                    UpdateItemDisplayStatus(item, true);
                    if (second.Count == 2)
                    {
                        UpdateItemDisplayStatus(second[0], true);
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
            internal bool Contains(ShortcutKey keys)
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
            internal void Remove(ShortcutListItem item, ShortcutKey keys)
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
            private List<ShortcutKey> custom;

            /// <summary>
            /// Creates a new instance of <see cref="ShortcutListItem"/> with an associated <see cref="ShortcutItem"/>.
            /// </summary>
            internal ShortcutListItem(ShortcutItem shortcutItem)
            {
                this.item = shortcutItem;
                this.custom = new List<ShortcutKey>(this.Item.Custom);
                this.Name = this.Text = this.Command;
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
            public string Command
            {
                get { return this.Item.Command; }
            }

            /// <summary>
            /// Gets the default shortcut keys.
            /// </summary>
            public ShortcutKey[] Default
            {
                get { return this.Item.Default; }
            }

            /// <summary>
            /// Gets or sets the custom shortcut keys.
            /// </summary>
            public List<ShortcutKey> Custom
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
            public bool IsCustomized
            {
                get
                {
                    return !SequenceEqual(this.Default, this.Custom);
                }
            }

            /// <summary>
            /// Gets the current shortcut keys.
            /// </summary>
            internal ShortcutKey[] Current
            {
                get { return this.item.Custom; }
            }

            /// <summary>
            /// Gets whether the shortcut has changed from the current keys.
            /// </summary>
            internal bool IsModified
            {
                get
                {
                    return !SequenceEqual(this.Current, this.Custom);
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
                        return ShortcutKey.None.ToString();
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
            /// Performs a sequential equality check between an array and a list.
            /// </summary>
            private static bool SequenceEqual(ShortcutKey[] array, List<ShortcutKey> list)
            {
                if (array.Length != list.Count)
                {
                    return false;
                }
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] != list[i])
                    {
                        return false;
                    }
                }
                return true;
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

        #region ListView Native Methods

        /// <summary>
        /// Provides collapsible groups.
        /// </summary>
        private sealed class ListViewEx : ListView
        {
            private const int LVGF_STATE = 0x00000004;
            private const int LVGS_COLLAPSIBLE = 0x00000008;
            private const int LVM_SETGROUPINFO = 0x00001000 + 147;

            /// <summary>
            /// LVGROUP structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            private struct LVGROUP
            {
                internal uint cbSize;
                internal uint mask;
                [MarshalAs(UnmanagedType.LPWStr)]
                internal string pszHeader;
                internal int cchHeader;
                [MarshalAs(UnmanagedType.LPWStr)]
                internal string pszFooter;
                internal int cchFooter;
                internal int iGroupId;
                internal uint stateMask;
                internal uint state;
                internal uint uAlign;
            }

            /// <summary>
            /// Adds an item to the list view with the specified group name.
            /// </summary>
            internal void AddToGroup(ListViewItem item, string groupName)
            {
                if (this.Groups[groupName] == null)
                {
                    var group = new ListViewGroup(groupName, groupName);
                    this.Groups.Add(group);
                    SetGroupState(this.Handle, group, LVGS_COLLAPSIBLE);
                }

                this.Groups[groupName].Items.Add(item);
                this.Items.Add(item);
            }

            /// <summary>
            /// Uses the native list view API to enable collapsible groups.
            /// </summary>
            private static void SetGroupState(IntPtr hwnd, ListViewGroup group, uint state)
            {
                if (Win32.ShouldUseWin32())
                {
                    var lvgroup = new LVGROUP()
                    {
                        cbSize = (uint) Marshal.SizeOf(typeof(LVGROUP)),
                        mask = LVGF_STATE,
                        iGroupId = (int) typeof(ListViewGroup).InvokeMember("ID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null, group, null),
                        state = state
                    };
                    var ptr = Marshal.AllocHGlobal((int) lvgroup.cbSize);
                    try
                    {
                        Marshal.StructureToPtr(lvgroup, ptr, false);
                        Win32.SendMessage(hwnd, LVM_SETGROUPINFO, (IntPtr) lvgroup.iGroupId, ptr);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                }
            }

            /// <summary>
            /// Override WindowProc to call DefWindowProc, enabling mouse clicks to collapse/expand groups.
            /// </summary>
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == Win32.WM_LBUTTONUP)
                {
                    this.DefWndProc(ref m);
                }
                base.WndProc(ref m);
            }
        }

        #endregion
    }

}
