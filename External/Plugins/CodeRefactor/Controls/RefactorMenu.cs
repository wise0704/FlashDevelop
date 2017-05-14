using System;
using System.Drawing;
using System.Windows.Forms;
using ASCompletion;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Localization;

namespace CodeRefactor.Controls
{
    public class RefactorMenu : ToolStripMenuItem
    {
        public RefactorMenu(Boolean createSurroundMenu)
        {
            this.Text = TextHelper.GetString("Label.Refactor");
            this.RenameMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Rename"), PluginBase.MainForm.FindImage("331"));
            this.DropDownItems.Add(this.RenameMenuItem);
            this.MoveMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Move"));
            this.DropDownItems.Add(this.MoveMenuItem);
            this.ExtractMethodMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.ExtractMethod"));
            this.DropDownItems.Add(this.ExtractMethodMenuItem);
            this.ExtractLocalVariableMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.ExtractLocalVariable"));
            this.DropDownItems.Add(this.ExtractLocalVariableMenuItem);
            this.DelegateMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.DelegateMethods"));
            this.DropDownItems.Add(this.DelegateMenuItem);
            if (createSurroundMenu)
            {
                this.SurroundMenu = new SurroundMenu();
                this.DropDownItems.Add(this.SurroundMenu);
            }
            this.DropDownItems.Add(new ToolStripSeparator());
            this.CodeGeneratorMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.InvokeCodeGenerator"));
            this.DropDownItems.Add(this.CodeGeneratorMenuItem);
            this.DropDownItems.Add(new ToolStripSeparator());
            this.OrganizeMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.OrganizeImports"), Overlay(ASContext.Panel.GetIcon(PluginUI.ICON_PACKAGE), "-1|22|4|4"));
            this.DropDownItems.Add(this.OrganizeMenuItem);
            this.TruncateMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.TruncateImports"), Overlay(ASContext.Panel.GetIcon(PluginUI.ICON_PACKAGE), "-1|18|4|4"));
            this.DropDownItems.Add(this.TruncateMenuItem);
            this.DropDownItems.Add(new ToolStripSeparator());
            this.BatchMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.BatchProcess"));
            this.DropDownItems.Add(this.BatchMenuItem);
        }

        /// <summary>
        /// Accessor to the SurroundMenu
        /// </summary>
        public SurroundMenu SurroundMenu { get; }

        /// <summary>
        /// Accessor to the BatchMenuItem
        /// </summary>
        public ToolStripMenuItem BatchMenuItem { get; }

        /// <summary>
        /// Accessor to the RenameMenuItem
        /// </summary>
        public ToolStripMenuItem RenameMenuItem { get; }

        /// <summary>
        /// Accessor to the MoveMenuItem
        /// </summary>
        public ToolStripMenuItem MoveMenuItem { get; }

        /// <summary>
        /// Accessor to the TruncateMenuItem
        /// </summary>
        public ToolStripMenuItem TruncateMenuItem { get; }

        /// <summary>
        /// Accessor to the OrganizeMenuItem
        /// </summary>
        public ToolStripMenuItem OrganizeMenuItem { get; }

        /// <summary>
        /// Accessor to the ExtractMethodMenuItem
        /// </summary>
        public ToolStripMenuItem ExtractMethodMenuItem { get; }

        /// <summary>
        /// Accessor to the DelegateMenuItem
        /// </summary>
        public ToolStripMenuItem DelegateMenuItem { get; }

        /// <summary>
        /// Accessor to the ExtractLocalVariableMenuItem
        /// </summary>
        public ToolStripMenuItem ExtractLocalVariableMenuItem { get; }

        /// <summary>
        /// Accessor to the CodeGeneratorMenuItem
        /// </summary>
        public ToolStripMenuItem CodeGeneratorMenuItem { get; }

        private static Image Overlay(Image source, string overlayData)
        {
            var image = new Bitmap(source);
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.DrawImage(PluginBase.MainForm.FindImage16(overlayData), 0, 0);
            }
            return PluginBase.MainForm.GetAutoAdjustedImage(image);
        }
    }

}
