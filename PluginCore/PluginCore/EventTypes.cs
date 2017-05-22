using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace PluginCore
{
    /// <summary>
    /// Events without arguments
    /// </summary>
    public class NotifyEvent
    {
        private EventType type;
        private Boolean handled;

        public EventType Type
        {
            get { return this.type; }
        }

        public Boolean Handled
        {
            get { return this.handled; }
            set { this.handled = value; }
        }

        public NotifyEvent(EventType type)
        {
            this.handled = false;
            this.type = type;
        }
    }

    /// <summary>
    /// Events with text data
    /// </summary>
    public class TextEvent : NotifyEvent
    {
        private String value;

        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public TextEvent(EventType type, String value) : base(type)
        {
            this.value = value;
        }
    }

    /// <summary>
    /// Events with number data
    /// </summary>
    public class NumberEvent : NotifyEvent
    {
        private Int32 value;

        public Int32 Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public NumberEvent(EventType type, Int32 value) : base(type)
        {
            this.value = value;
        }

    }

    /// <summary>
    /// Provides data for the <see cref="EventType.Keys"/> event.
    /// </summary>
    public class KeyEvent : NotifyEvent
    {
        private Keys keyData;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEvent"/> class.
        /// </summary>
        public KeyEvent(EventType type, Keys keyData) : base(type)
        {
            this.keyData = keyData;
        }

        #region Deprecated

        /// <summary>
        /// [deprecated] Use the <see cref="KeyData"/> property instead.
        /// </summary>
        [Obsolete("This property has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Keys Value
        {
            get { return this.KeyData; }
            set { }
        }

        /// <summary>
        /// [deprecated] Use the <see cref="NotifyEvent.Handled"/> property instead.
        /// </summary>
        [Obsolete("This property has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ProcessKey
        {
            get { return this.Handled; }
            set { this.Handled = value; }
        }

        #endregion

        /// <summary>
        /// Gets the key data for the event.
        /// </summary>
        public Keys KeyData
        {
            get { return this.keyData; }
        }

        /// <summary>
        /// Gets the keyboard code for the event.
        /// </summary>
        public Keys KeyCode
        {
            get { return this.keyData & Keys.KeyCode; }
        }

        /// <summary>
        /// Gets the keyboard value for the event.
        /// </summary>
        public int KeyValue
        {
            get { return (int) (this.keyData & Keys.KeyCode); }
        }

        /// <summary>
        /// Gets the modifier flags for the event.
        /// The flags indicate which combination of CTRL, SHIFT, and ALT keys was pressed.
        /// </summary>
        public Keys Modifiers
        {
            get { return this.keyData & Keys.Modifiers; }
        }

        /// <summary>
        /// Gets a value indicating whether the CTRL key was pressed.
        /// </summary>
        public bool Control
        {
            get { return (this.keyData & Keys.Control) == Keys.Control; }
        }

        /// <summary>
        /// Gets a value indicating whether the ALT key was pressed.
        /// </summary>
        public bool Alt
        {
            get { return (this.keyData & Keys.Alt) == Keys.Alt; }
        }

        /// <summary>
        /// Gets a value indicating whether the SHIFT key was pressed.
        /// </summary>
        public bool Shift
        {
            get { return (this.keyData & Keys.Shift) == Keys.Shift; }
        }
    }

    /// <summary>
    /// Provides data for the <see cref="EventType.ShortcutKey"/> event.
    /// </summary>
    public class ShortcutKeyEvent : NotifyEvent
    {
        private string command;
        private ShortcutKey shortcutKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutKeyEvent"/> class.
        /// </summary>
        public ShortcutKeyEvent(EventType type, string command, ShortcutKey shortcutKeys) : base(type)
        {
            this.command = command;
            this.shortcutKeys = shortcutKeys;
        }

        /// <summary>
        /// Gets the shortcut command for the event.
        /// </summary>
        public string Command
        {
            get { return this.command; }
        }

        /// <summary>
        /// Gets the shortcut keys for the event.
        /// </summary>
        public ShortcutKey ShortcutKeys
        {
            get { return this.shortcutKeys; }
        }
    }

    /// <summary>
    /// Provides data for the <see cref="EventType.ShortcutUpdate"/> event.
    /// </summary>
    public class ShortcutUpdateEvent : NotifyEvent
    {
        private string command;
        private ReadOnlyCollection<ShortcutKey> shortcutKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutUpdateEvent"/> class.
        /// </summary>
        public ShortcutUpdateEvent(EventType type, string command, ReadOnlyCollection<ShortcutKey> shortcutKeys) : base(type)
        {
            this.command = command;
            this.shortcutKeys = shortcutKeys;
        }

        /// <summary>
        /// Gets the shortcut command for the event.
        /// </summary>
        public string Command
        {
            get { return this.command; }
        }

        /// <summary>
        /// Gets the read-only collection of shortcut keys for the event.
        /// </summary>
        public ReadOnlyCollection<ShortcutKey> ShortcutKeys
        {
            get { return this.shortcutKeys; }
        }
    }

    /// <summary>
    /// Events with custom data
    /// </summary>
    public class DataEvent : NotifyEvent
    {
        private Object data;
        private String action;

        public String Action
        {
            get { return this.action; }
        }

        public Object Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        public DataEvent(EventType type, String action, Object data) : base(type)
        {
            this.action = action;
            this.data = data;
        }

    }

    public class TextDataEvent : TextEvent
    {
        public object Data { get; }

        public TextDataEvent(EventType type, string text, object data) : base(type, text)
        {
            Data = data;
        }
    }

}
