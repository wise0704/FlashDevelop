using System;
using System.ComponentModel;

namespace PluginCore
{
    [Flags]
    public enum EventType : long
    {
        FileNew        = 0x0000000000000001, // TextEvent (file)
        FileOpen       = 0x0000000000000002, // TextEvent (file)
        FileOpening    = 0x0000000000000004, // TextEvent (file)
        FileClose      = 0x0000000000000008, // TextEvent (file)
        FileSwitch     = 0x0000000000000010, // NotifyEvent
        FileModify     = 0x0000000000000020, // TextEvent (file)
        FileModifyRO   = 0x0000000000000040, // TextEvent (file)
        FileSave       = 0x0000000000000080, // TextEvent (file)
        FileSaving     = 0x0000000000000100, // TextEvent (file)
        FileReload     = 0x0000000000000200, // TextEvent (file)
        FileRevert     = 0x0000000000000400, // TextEvent (file)
        FileRename     = 0x0000000000000800, // TextEvent (old;new)
        FileRenaming   = 0x0000000000001000, // TextEvent (old;new)
        FileEncode     = 0x0000000000002000, // DataEvent (file, text)
        FileDecode     = 0x0000000000004000, // DataEvent (file, null)
        FileEmpty      = 0x0000000000008000, // NotifyEvent
        FileTemplate   = 0x0000000000010000, // TextEvent (file)
        RestoreSession = 0x0000000000020000, // DataEvent (file, session)
        RestoreLayout  = 0x0000000000040000, // TextEvent (file)
        SyntaxChange   = 0x0000000000080000, // TextEvent (language)
        SyntaxDetect   = 0x0000000000100000, // TextEvent (language)
        UIStarted      = 0x0000000000200000, // NotifyEvent
        UIRefresh      = 0x0000000000400000, // NotifyEvent
        UIClosing      = 0x0000000000800000, // NotifyEvent
        ApplySettings  = 0x0000000001000000, // NotifyEvent
        SettingChanged = 0x0000000002000000, // TextEvent (setting)
        ProcessArgs    = 0x0000000004000000, // TextEvent (content)
        ProcessStart   = 0x0000000008000000, // NotifyEvent
        ProcessEnd     = 0x0000000010000000, // TextEvent (result)
        StartArgs      = 0x0000000020000000, // NotifyEvent
        [Obsolete("This enum value has been deprecated.", true)]
        Shortcut       = 0x0000000040000000, // DataEvent (id, keys)
        Command        = 0x0000000080000000, // DataEvent (command)
        Trace          = 0x0000000100000000, // NotifyEvent
        Keys           = 0x0000000200000000, // KeyEvent (keys)
        Completion     = 0x0000000400000000, // NotifyEvent
        AppChanges     = 0x0000000800000000, // NotifyEvent
        ApplyTheme     = 0x0000001000000000, // NotifyEvent
        ShortcutKeys   = 0x0000002000000000, // ShortcutKeysEvent
        ShortcutUpdate = 0x0000004000000000, // ShortcutUpdateEvent
    }

    public enum UpdateInterval
    {
        Never = -1,
        Monthly = 0,
        Weekly = 1
    }

    public enum SessionType
    {
        Startup = 0,
        Layout = 1,
        External = 2
    }

    public enum CodingStyle
    {
        BracesOnLine = 0,
        BracesAfterLine = 1
    }

    public enum CommentBlockStyle
    {
        Indented = 0,
        NotIndented = 1
    }

    public enum UiRenderMode
    {
        Professional,
        System
    }

    public enum HandlingPriority
    {
        High = 0,
        Normal = 1,
        Low = 2
    }

    public enum TraceType
    {
        ProcessStart = -1,
        ProcessEnd = -2,
        ProcessError = -3,
        Info = 0,
        Debug = 1,
        Warning = 2,
        Error = 3,
        Fatal = 4
    }

    public enum CodePage
    {
        EightBits = 0,
        BigEndian = 1201,
        LittleEndian = 1200,
        [Browsable(false)]
        UTF32 = 65005,
        UTF8 = 65001,
        UTF7 = 65000
    }

}
