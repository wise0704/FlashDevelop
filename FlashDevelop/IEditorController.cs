using System.Windows.Forms;

namespace FlashDevelop
{
    interface IEditorController
    {
        object Owner { get; }

        bool? ProcessCmdKey(Keys keydata);
    }
}
