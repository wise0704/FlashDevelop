using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PluginCore.Controls;
using PluginCore.Localization;
using ScintillaNet;
using ScintillaNet.Configuration;
using ScintillaNet.Enums;
using WeifenLuo.WinFormsUI.Docking;
using Keys = System.Windows.Forms.Keys;

namespace PluginCore
{
    public interface IPlugin : IEventHandler
    {
        #region IPlugin Methods

        void Dispose();
        void Initialize();

        #endregion

        #region IPlugin Properties

        Int32 Api { get; }
        String Name { get; }
        String Guid { get; }
        String Help { get; }
        String Author { get; }
        String Description { get; }
        Object Settings { get; }

        // List of valid API levels:
        // FlashDevelop 4.0 = 1

        #endregion
    }

    public interface IEventHandler
    {
        #region IEventHandler Methods

        void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority);

        #endregion
    }

    public interface ITabbedDocument : IDockContent
    {
        #region ITabbedDocument Properties

        Icon Icon { get; set; }
        String FileName { get; }
        String Text { get; set; }
        Boolean UseCustomIcon { get; set; }
        Control.ControlCollection Controls { get; }
        SplitContainer SplitContainer { get; }
        ScintillaControl SciControl { get; }
        ScintillaControl SplitSci1 { get; }
        ScintillaControl SplitSci2 { get; }
        Boolean IsModified { get; set; }
        Boolean IsSplitted { get; set; }
        Boolean IsBrowsable { get; }
        Boolean IsUntitled { get; }
        Boolean IsEditable { get; }
        Boolean HasBookmarks { get; }
        Boolean IsAloneInPane { get; }

        #endregion

        #region ITabbedDocument Methods

        void Close();
        void Activate();
        void RefreshTexts();
        void Reload(Boolean showQuestion);
        void Revert(Boolean showQuestion);
        void Save(String file);
        void Save(string file, string reason);
        void Save();

        #endregion
    }

    public interface ICompletionListItem
    {
        #region ICompletionListItem Properties

        String Label { get; }
        String Value { get; }
        String Description { get; }
        Bitmap Icon { get; }

        #endregion
    }

    public interface IMainForm : IContainerControl, IWin32Window
    {
        #region IMainForm Methods

        /// <summary>
        /// Refreshes the main form.
        /// </summary>
        void RefreshUI();
        /// <summary>
        /// Stop the currently running process.
        /// </summary>
        void KillProcess();
        /// <summary>
        /// Refreshes the scintilla configuration.
        /// </summary>
        void RefreshSciConfig();
        /// <summary>
        /// Shows a message to restart FD.
        /// </summary>
        void RestartRequired();
        /// <summary>
        /// Themes the controls from the parent.
        /// </summary>
        void ThemeControls(Object control);
        /// <summary>
        /// Clears the temporary file from disk.
        /// </summary>
        void ClearTemporaryFiles(String file);
        /// <summary>
        /// Shows the settings dialog.
        /// </summary>
        void ShowSettingsDialog(String itemName);
        /// <summary>
        /// Shows the error dialog if the sender is <see cref="Managers.ErrorManager"/>.
        /// </summary>
        void ShowErrorDialog(Object sender, Exception ex);
        /// <summary>
        /// Shows the settings dialog with a filter.
        /// </summary>
        void ShowSettingsDialog(String itemName, String filter);
        /// <summary>
        /// Lets you update menu items using the flag functionality.
        /// </summary>
        void AutoUpdateMenuItem(ToolStripItem item, String action);
        /// <summary>
        /// Adds an ignored key. Ignored keys are valid shortcut keys that are not defined with <see cref="RegisterShortcut(string, ShortcutKey[])"/>,
        /// but should not prompt an "undefined shortcut keys" message. Instead these keys should have their default behaviors.
        /// These are constant shortcuts which cannot be modified using the shortcut dialog.
        /// </summary>
        void AddIgnoredKeys(ShortcutKey keys);
        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether the specified key is ignored.
        /// </summary>
        bool ContainsIgnoredKeys(ShortcutKey keys);
        /// <summary>
        /// Removes the specified key from ignored keys.
        /// </summary>
        bool RemoveIgnoredKeys(ShortcutKey keys);
        /// <summary>
        /// Clears all ignored keys.
        /// </summary>
        void ClearIgnoredKeys();
        /// <summary>
        /// Registers a new menu item with the shortcut manager.
        /// <para/>
        /// [deprecated] Use the <see cref="RegisterShortcut(string, ShortcutKey[])"/> method instead.
        /// </summary>
        [Obsolete("This method has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void RegisterShortcutItem(string id, Keys keys);
        /// <summary>
        /// Registers a new menu item with the shortcut manager.
        /// <para/>
        /// [deprecated] Use the <see cref="RegisterShortcut(string, ToolStripItem[])"/> method instead.
        /// </summary>
        [Obsolete("This method has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void RegisterShortcutItem(string id, ToolStripMenuItem item);
        /// <summary>
        /// Registers a new secondary menu item with the shortcut manager
        /// <para/>
        /// [deprecated] Use the <see cref="RegisterShortcut(string, ToolStripItem[])"/> method instead.
        /// </summary>
        [Obsolete("This method has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void RegisterSecondaryItem(string id, ToolStripItem item);
        /// <summary>
        /// Registers the specified shortcut command, and sets the default keys to the specified key.
        /// If the shortcut command is already registered, the specified key is concatenated to the existing default keys.
        /// A <see cref="ShortcutKey.None"/> value is ignored.
        /// </summary>
        void RegisterShortcut(string command, [Optional] ShortcutKey defaultShortcut);
        /// <summary>
        /// Registers the specified shortcut command, and sets the default keys to the specified keys.
        /// If the shortcut command is already registered, the specified keys are concatenated to the existing default keys.
        /// <see cref="ShortcutKey.None"/> values are ignored.
        /// </summary>
        void RegisterShortcut(string command, params ShortcutKey[] defaultShortcuts);
        /// <summary>
        /// Registers the specified shortcut command, and sets the associated <see cref="ToolStripItem"/> objects.
        /// If <paramref name="toolStripItems"/> contains any <see cref="ToolStripMenuItem"/> or <see cref="ToolStripMenuItemEx"/> objects,
        /// their <see cref="ToolStripMenuItem.ShortcutKeys"/> and <see cref="ToolStripMenuItemEx.ShortcutKeys"/> properties will be used to set the default keys.
        /// <see cref="Keys.None"/>, <see cref="ShortcutKey.None"/> and <see langword="null"/> values are ignored.
        /// If the shortcut command is already registered, the specified tool strip items and their shortcut keys are concatenated to the existing items and default keys.
        /// </summary>
        void RegisterShortcut(string command, params ToolStripItem[] toolStripItems);
        /// <summary>
        /// Registers the specified shortcut command, sets the default keys to the specified key and sets the associated <see cref="ToolStripItem"/> objects.
        /// If <paramref name="toolStripItems"/> contains any <see cref="ToolStripMenuItem"/> or <see cref="ToolStripMenuItemEx"/> objects,
        /// their <see cref="ToolStripMenuItem.ShortcutKeys"/> and <see cref="ToolStripMenuItemEx.ShortcutKeys"/> properties will also be used to set the default keys.
        /// <see cref="Keys.None"/>, <see cref="ShortcutKey.None"/> and <see langword="null"/> values are ignored.
        /// If the shortcut command is already registered, the specified keys and tool strip items are concatenated to the existing default keys and items.
        /// </summary>
        void RegisterShortcut(string command, ShortcutKey defaultShortcut, params ToolStripItem[] toolStripItems);
        /// <summary>
        /// Registers the specified shortcut command, sets the default keys to the specified keys and sets the associated <see cref="ToolStripItem"/> objects.
        /// If <paramref name="toolStripItems"/> contains any <see cref="ToolStripMenuItem"/> or <see cref="ToolStripMenuItemEx"/> objects,
        /// their <see cref="ToolStripMenuItem.ShortcutKeys"/> and <see cref="ToolStripMenuItemEx.ShortcutKeys"/> properties will also be used to set the default keys.
        /// <see cref="Keys.None"/>, <see cref="ShortcutKey.None"/> and <see langword="null"/> values are ignored.
        /// If the shortcut command is already registered, the specified keys and tool strip items are concatenated to the existing default keys and items.
        /// </summary>
        void RegisterShortcut(string command, ShortcutKey[] defaultShortcuts, ToolStripItem[] toolStripItems);
        /// <summary>
        /// Updates a registered secondary menu item in the shortcut manager
        /// - should be called when the tooltip changes.
        /// </summary>
        void ApplySecondaryShortcut(ToolStripItem item);
        /// <summary>
        /// Create the specified new document from the given template.
        /// </summary>
        void FileFromTemplate(String templatePath, String newFilePath);
        /// <summary>
        /// Opens an editable document.
        /// </summary>
        DockContent OpenEditableDocument(String file, Boolean restoreFileState);
        /// <summary>
        /// Opens an editable document.
        /// </summary>
        DockContent OpenEditableDocument(String file);
        /// <summary>
        /// Creates a new custom document.
        /// </summary>
        DockContent CreateCustomDocument(Control ctrl);
        /// <summary>
        /// Creates a new empty document.
        /// </summary>
        DockContent CreateEditableDocument(String file, String text, Int32 codepage);
        /// <summary>
        /// Creates a floating panel for the plugin.
        /// </summary>
        DockContent CreateDockablePanel(Control form, String guid, Image image, DockState defaultDockState);
        /// <summary>
        /// Creates a dynamic persist panel for plugins.
        /// </summary>
        DockContent CreateDynamicPersistDockablePanel(Control ctrl, String guid, String id, Image image, DockState defaultDockState);
        /// <summary>
        /// Calls a normal <see cref="IMainForm"/> method.
        /// </summary>
        Boolean CallCommand(String command, String arguments);
        /// <summary>
        /// Finds the menu items that have the specified name.
        /// </summary>
        List<ToolStripItem> FindMenuItems(String name);
        /// <summary>
        /// Finds the specified menu item by name.
        /// </summary>
        ToolStripItem FindMenuItem(String name);
        /// <summary>
        /// Processes the argument string variables.
        /// </summary>
        String ProcessArgString(String args);
        /// <summary>
        /// Gets the specified item's shortcut keys.
        /// <para/>
        /// [deprecated] Use the <see cref="GetShortcutKeys(string)"/> method instead.
        /// </summary>
        [Obsolete("This method has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Keys GetShortcutItemKeys(String id);
        /// <summary>
        /// Gets the specified item's id.
        /// <para/>
        /// [deprecated] Use the <see cref="GetShortcutId(ShortcutKey)"/> method instead.
        /// </summary>
        [Obsolete("This method has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        String GetShortcutItemId(Keys keys);
        /// <summary>
        /// Gets the specified item's shortcut keys.
        /// </summary>
        ShortcutKey GetShortcutKeys(String id);
        /// <summary>
        /// Gets the specified item's id.
        /// </summary>
        String GetShortcutId(ShortcutKey keys);
        /// <summary>
        /// Gets a theme property value.
        /// </summary>
        String GetThemeValue(String id);
        /// <summary>
        /// Gets a theme property color.
        /// </summary>
        Color GetThemeColor(String id);
        /// <summary>
        /// Gets a theme flag value.
        /// </summary>
        Boolean GetThemeFlag(String id);
        /// <summary>
        /// Gets a theme flag value with a fallback.
        /// </summary>
        Boolean GetThemeFlag(String id, Boolean fallback);
        /// <summary>
        /// Gets a theme property value with a fallback.
        /// </summary>
        String GetThemeValue(String id, String fallback);
        /// <summary>
        /// Gets a theme property color with a fallback.
        /// </summary>
        Color GetThemeColor(String id, Color fallback);
        /// <summary>
        /// Finds the specified plugin.
        /// </summary>
        IPlugin FindPlugin(String guid);
        /// <summary>
        /// Adjusts the image for different themes.
        /// </summary>
        Image ImageSetAdjust(Image image);
        /// <summary>
        /// Gets a copy of the image that gets automatically adjusted according to the theme.
        /// </summary>
        Image GetAutoAdjustedImage(Image image);
        /// <summary>
        /// Finds the specified composed/ready image that is automatically adjusted according to the theme.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted.
        /// </summary>
        Image FindImage(String data);
        /// <summary>
        /// Finds the specified composed/ready image.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted, even if <code>autoAdjusted</code> is <code>true</code>.
        /// </summary>
        Image FindImage(String data, Boolean autoAdjust);
        /// <summary>
        /// Finds the specified composed/ready image that is automatically adjusted according to the theme.
        /// The image size is always 16x16.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted.
        /// </summary>
        Image FindImage16(String data);
        /// <summary>
        /// Finds the specified composed/ready image. The image size is always 16x16.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted, even if <code>autoAdjusted</code> is <code>true</code>.
        /// </summary>
        Image FindImage16(String data, Boolean autoAdjusted);
        /// <summary>
        /// Finds the specified composed/ready image and returns a copy of the image that has its color adjusted.
        /// This method is typically used for populating a <see cref="ImageList"/> object.
        /// <para/>
        /// Equivalent to calling <code>ImageSetAdjust(FindImage(data, false))</code>.
        /// </summary>
        Image FindImageAndSetAdjust(String data);
        /// <summary>
        /// Gets the amount of FD instances running
        /// </summary>
        Int32 GetInstanceCount();

        #endregion

        #region IMainFrom Properties

        /// <summary>
        /// Gets the <see cref="ISettings"/> interface.
        /// </summary>
        ISettings Settings { get; }
        /// <summary>
        /// Gets the tool strip.
        /// </summary>
        ToolStrip ToolStrip { get; }
        /// <summary>
        /// Gets the menu strip.
        /// </summary>
        MenuStrip MenuStrip { get; }
        /// <summary>
        /// Gets the <see cref="Scintilla"/> configuration.
        /// </summary>
        Scintilla SciConfig { get; }
        /// <summary>
        /// Gets the dock panel.
        /// </summary>
        DockPanel DockPanel { get; }
        /// <summary>
        /// Gets the application start arguments.
        /// </summary>
        String[] StartArguments { get; }
        /// <summary>
        /// Gets the application custom arguments.
        /// </summary>
        List<Argument> CustomArguments { get; }
        /// <summary>
        /// Gets the status strip.
        /// </summary>
        StatusStrip StatusStrip { get; }
        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        String WorkingDirectory { get; set; }
        /// <summary>
        /// Gets the tool strip panel.
        /// </summary>
        ToolStripPanel ToolStripPanel { get; }
        /// <summary>
        /// Gets the tool strip status label.
        /// Use <see cref="StatusLabelText"/> instead to modify the status text.
        /// </summary>
        ToolStripStatusLabel StatusLabel { get; }
        /// <summary>
        /// Gets or sets the text of the <see cref="StatusLabel"/>.
        /// Use this property instead of directly accessing the <code>Text</code> property of <see cref="StatusLabel"/>.
        /// </summary>
        string StatusLabelText { get; set; }
        /// <summary>
        /// Gets the tool strip progress label.
        /// </summary>
        ToolStripStatusLabel ProgressLabel { get; }
        /// <summary>
        /// Gets the tool strip progress bar.
        /// </summary>
        ToolStripProgressBar ProgressBar { get; }
        /// <summary>
        /// Gets the collection of controls contained within this control.
        /// </summary>
        Control.ControlCollection Controls { get; }
        /// <summary>
        /// Gets the tab menu.
        /// </summary>
        ContextMenuStrip TabMenu { get; }
        /// <summary>
        /// Gets the editor menu.
        /// </summary>
        ContextMenuStrip EditorMenu { get; }
        /// <summary>
        /// Gets the current <see cref="ITabbedDocument"/> object.
        /// </summary>
        ITabbedDocument CurrentDocument { get; }
        /// <summary>
        /// Gets all available documents.
        /// </summary>
        ITabbedDocument[] Documents { get; }
        /// <summary>
        /// Gets whether FlashDevelop holds modified documents.
        /// </summary>
        Boolean HasModifiedDocuments { get; }
        /// <summary>
        /// Gets whether FlashDevelop is closing.
        /// </summary>
        Boolean ClosingEntirely { get; }
        /// <summary>
        /// Gets whether a process is running.
        /// </summary>
        Boolean ProcessIsRunning { get; }
        /// <summary>
        /// Gets whether a document is reloading.
        /// </summary>
        Boolean ReloadingDocument { get; }
        /// <summary>
        /// Gets whether contents are being processed.
        /// </summary>
        Boolean ProcessingContents { get; }
        /// <summary>
        /// Gets whether contents are being restored.
        /// </summary>
        Boolean RestoringContents { get; }
        /// <summary>
        /// Gets saving multiple.
        /// </summary>
        Boolean SavingMultiple { get; }
        /// <summary>
        /// Gets whether the panel is active.
        /// </summary>
        Boolean PanelIsActive { get; }
        /// <summary>
        /// Gets whether FlashDevelop is in full screen.
        /// </summary>
        Boolean IsFullScreen { get; }
        /// <summary>
        /// Gets whether FlashDevelop is in standalone mode.
        /// </summary>
        Boolean StandaloneMode { get; }
        /// <summary>
        /// Gets whether FlashDevelop is in multi-instance mode.
        /// </summary>
        Boolean MultiInstanceMode { get; }
        /// <summary>
        /// Gets whether this <see cref="IMainForm"/> is the first instance.
        /// </summary>
        Boolean IsFirstInstance { get; }
        /// <summary>
        /// Gets whether a restart is required.
        /// </summary>
        Boolean RestartRequested { get; }
        /// <summary>
        /// Gets whether the application requires a restart to apply changes.
        /// </summary>
        Boolean RequiresRestart { get; }
        /// <summary>
        /// Gets whether the config should be refreshed.
        /// </summary>
        Boolean RefreshConfig { get; }
        /// <summary>
        /// Gets the ignored keys.
        /// <para/>
        /// [deprecated] This property always returns an empty <see cref="List{T}"/>.
        /// Use the <see cref="AddIgnoredKeys"/> method instead.
        /// </summary>
        [Obsolete("This property has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        List<Keys> IgnoredKeys { get; }
        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        String ProductVersion { get; }
        /// <summary>
        /// Gets the full human readable version string.
        /// </summary>
        String ProductName { get; }
        /// <summary>
        /// Gets the command prompt executable (custom or cmd.exe by default).
        /// </summary>
        String CommandPromptExecutable { get; }

        #endregion
    }

    public interface IProject
    {
        #region IProject Methods

        String[] GetHiddenPaths();
        String GetRelativePath(String path);
        String GetAbsolutePath(String path);

        /// <summary>
        /// When in Release configuration, remove 'debug' from the given path.
        /// Pattern: ([a-zA-Z0-9])[-_.]debug([\\/.])
        /// </summary>
        String FixDebugReleasePath(String path);

        #endregion

        #region IProject Properties

        String Name { get; }
        String Language { get; }
        String OutputPathAbsolute { get; }
        String[] SourcePaths { get; }
        Boolean TraceEnabled { get; }
        Boolean EnableInteractiveDebugger { get; }
        String ProjectPath { get; }
        String PreferredSDK { get; }
        String CurrentSDK { get; }
        String DefaultSearchFilter { get; }

        #endregion
    }

    public interface ISettings
    {
        #region ISettings Properties

        Font DefaultFont { get; set; }
        Font ConsoleFont { get; set; }
        List<String> DisabledPlugins { get; set; }
        List<String> PreviousDocuments { get; set; }
        LocaleVersion LocaleVersion { get; set; }
        UiRenderMode RenderMode { get; set; }
        CodingStyle CodingStyle { get; set; }
        CommentBlockStyle CommentBlockStyle { get; set; }
        FlatStyle ComboBoxFlatStyle { get; set; }
        String DefaultFileExtension { get; set; }
        String LatestDialogPath { get; set; }
        Boolean ConfirmOnExit { get; set; }
        String CustomSnippetDir { get; set; }
        String CustomTemplateDir { get; set; }
        String CustomProjectsDir { get; set; }
        String CustomCommandPrompt { get; set; }
        Boolean DisableFindOptionSync { get; set; }
        Boolean DisableSimpleQuickFind { get; set; }
        Boolean DisableReplaceFilesConfirm { get; set; }
        Boolean DisableExtendedShortcutKeys { get; set; }
        Boolean AutoReloadModifiedFiles { get; set; }
        Boolean UseListViewGrouping { get; set; }
        Boolean RedirectFilesResults { get; set; }
        Boolean DisableFindTextUpdating { get; set; }
        Boolean ApplyFileExtension { get; set; }
        Boolean RestoreFileStates { get; set; }
        Boolean RestoreFileSession { get; set; }
        Boolean BackSpaceUnIndents { get; set; }
        Boolean BraceMatchingEnabled { get; set; }
        Boolean CaretLineVisible { get; set; }
        Boolean EnsureConsistentLineEnds { get; set; }
        Boolean EnsureLastLineEnd { get; set; }
        Boolean UseSystemColors { get; set; }
        Boolean FoldAtElse { get; set; }
        Boolean FoldComment { get; set; }
        Boolean FoldCompact { get; set; }
        Boolean FoldHtml { get; set; }
        Boolean FoldPreprocessor { get; set; }
        Boolean HighlightGuide { get; set; }
        Boolean LineCommentsAfterIndent { get; set; }
        Boolean MoveCursorAfterComment { get; set; }
        Boolean StripTrailingSpaces { get; set; }
        Boolean SequentialTabbing { get; set; }
        Boolean TabIndents { get; set; }
        Boolean UseFolding { get; set; }
        Boolean UseTabs { get; set; }
        Boolean ViewEOL { get; set; }
        Boolean ViewBookmarks { get; set; }
        Boolean ViewLineNumbers { get; set; }
        Boolean ViewIndentationGuides { get; set; }
        Boolean ViewModifiedLines { get; set; }
        Boolean ViewToolBar { get; set; }
        Boolean ViewStatusBar { get; set; }
        Boolean ViewWhitespace { get; set; }
        Boolean ViewShortcuts { get; set; }
        Boolean WrapText { get; set; }
        EndOfLine EOLMode { get; set; }
        FoldFlag FoldFlags { get; set; }
        SmartIndent SmartIndentType { get; set; }
        VirtualSpaceMode VirtualSpaceMode { get; set; }
        IndentView IndentView { get; set; }
        HighlightMatchingWordsMode HighlightMatchingWordsMode { get; set; }
        Int32 HighlightMatchingWordsDelay { get; set; }
        CodePage DefaultCodePage { get; set; }
        Int32 TabWidth { get; set; }
        Int32 IndentSize { get; set; }
        Int32 CaretPeriod { get; set; }
        Int32 CaretWidth { get; set; }
        Int32 ScrollWidth { get; set; }
        Int32 PrintMarginColumn { get; set; }
        Size WindowSize { get; set; }
        FormWindowState WindowState { get; set; }
        Point WindowPosition { get; set; }
        Int32 HoverDelay { get; set; }
        Int32 DisplayDelay { get; set; }
        Boolean ShowDetails { get; set; }
        Boolean AutoFilterList { get; set; }
        Boolean EnableAutoHide { get; set; }
        Boolean WrapList { get; set; }
        Boolean DisableSmartMatch { get; set; }
        Boolean SaveUnicodeWithBOM { get; set; }
        Boolean KeepCaretCentered { get; set; }
        String InsertionTriggers { get; set; }
        Int32 ClipboardHistorySize { get; set; }

        #endregion
    }

    public interface ISession
    {
        #region ISession Properties

        Int32 Index { get; set; }
        List<String> Files { get; set; }
        SessionType Type { get; set; }

        #endregion
    }

    /// <summary>
    /// Represents a modal <see cref="Form"/> that handles the key input with the extended shortcut mechanism.
    /// </summary>
    public interface IShortcutHandlerForm
    {
        /// <summary>
        /// Handles a key event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An event object containing the shortcut keys event data.</param>
        void HandleEvent(object sender, NotifyEvent e);
        /// <summary>
        /// Calls <see cref="Form.ProcessMnemonic(char)"/>.
        /// </summary>
        /// <param name="charCode">The character to process.</param>
        bool ProcessMnemonic(char charCode);
    }

    /// <summary>
    /// Represents a modal <see cref="Form"/> that handles the key input with the extended shortcut mechanism,
    /// with the option to let certain keys skip the shortcut processing to be handled in the window procedure.
    /// </summary>
    public interface IShortcutHandlerModalForm : IShortcutHandlerForm
    {
        /// <summary>
        /// Returns whether the modal form should receive the unhandled key in <see cref="Control.WndProc(ref Message)"/>.
        /// </summary>
        /// <param name="key">The unhandled key.</param>
        bool IsInputKey(Keys keyData);
    }
}