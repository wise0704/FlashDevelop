using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using PluginCore.Controls;

namespace PluginCore.Helpers
{
    /// <summary>
    /// Provides a user interface for editing a <see cref="ShortcutKey"/> value at design time.
    /// </summary>
    public class ShortcutKeyEditor : UITypeEditor
    {
        /// <summary>
        /// Edits the <see cref="ShortcutKey"/> value using the modal editor.
        /// </summary>
        /// <param name="value">The <see cref="ShortcutKey"/> to edit.</param>
        /// <param name="allowNone">Whether to allow <see cref="ShortcutKey.None"/> to be set.</param>
        /// <param name="supportExtended">Whether extended shortcut is supported.</param>
        public ShortcutKey EditValue(ShortcutKey value, bool allowNone, bool supportExtended)
        {
            var dialog = new ShortcutKeyEditorDialog(value, allowNone, supportExtended);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                value = dialog.NewKeys;
            }
            return value;
        }

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="GetEditStyle"/> method.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The <see cref="object"/> to edit.</param>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return EditValue((ShortcutKey) value, true, !PluginBase.Settings.DisableExtendedShortcutKeys);
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="EditValue(ITypeDescriptorContext, IServiceProvider, object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
