﻿using System;
using System.Windows.Forms;

namespace FlashDevelop
{
    public interface IEditorController
    {
        object Owner { get; }

        object QuickFindControl { get; }

        /// <summary>
        /// Enables or disables controls
        /// </summary>
        bool CanSearch { get; set; }

        bool? ProcessCmdKey(Keys keydata);

        void ApplyAllSettings();

        /// <summary>
        /// Sets the text to find globally
        /// </summary>
        void SetFindText(object sender, string text);

        /// <summary>
        /// Sets the case setting to find globally
        /// </summary>
        void SetMatchCase(object sender, bool matchCase);

        /// <summary>
        /// Sets the whole word setting to find globally
        /// </summary>
        void SetWholeWord(object sender, bool wholeWord);

        /// <summary>
        /// Opens a goto dialog
        /// </summary>
        void ShowGoTo();

        /// <summary>
        /// Displays the next result
        /// </summary>
        void FindNext();

        /// <summary>
        /// Displays the previous result
        /// </summary>
        void FindPrevious();

        /// <summary>
        /// Opens a find and replace dialog
        /// </summary>
        void ShowFindAndReplace();

        /// <summary>
        /// Opens a find and replace in files dialog
        /// </summary>
        void ShowFindAndReplaceInFiles();

        /// <summary>
        /// Opens a find and replace in files dialog with a location
        /// </summary>
        void ShowFindAndReplaceInFilesFrom(string path);

        /// <summary>
        /// Shows the quick find control
        /// </summary>
        void ShowQuickFind();
    }
}
