using System;
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
    /// Events with Key data
    /// </summary>
    [Obsolete("This class has been deprecated.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class KeyEvent : NotifyEvent
    {
        private Keys value;
        private Boolean processKey;

        /// <summary>
        /// Gets the <see cref="Keys"/> value associated with this <see cref="KeyEvent"/>.
        /// </summary>
        public Keys Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets or sets whether to process the keys associated with this <see cref="KeyEvent"/>.
        /// </summary>
        public Boolean ProcessKey
        {
            get { return this.processKey; }
            set { this.processKey = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEvent"/> class.
        /// </summary>
        public KeyEvent(EventType type, Keys value) : base(type)
        {
            this.value = value;
            this.processKey = false;
        }
    }

    /// <summary>
    /// Represents events with shortcut keys.
    /// </summary>
    public class ShortcutKeysEvent : NotifyEvent
    {
        private string id;
        private ShortcutKeys shortcutKeys;

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutKeysEvent"/> class.
        /// </summary>
        public ShortcutKeysEvent(EventType type, string id, ShortcutKeys shortcutKeys) : base(type)
        {
            this.id = id;
            this.shortcutKeys = shortcutKeys;
        }

        /// <summary>
        /// Gets the shortcut ID associated with this <see cref="ShortcutKeysEvent"/> object.
        /// If this property is <see langword="null"/>, the associated shortcut key is not a registered shortcut.
        /// </summary>
        public string Id
        {
            get { return this.id; }
        }

        /// <summary>
        /// Gets the shortcut keys associated with this <see cref="ShortcutKeysEvent"/> object.
        /// </summary>
        public ShortcutKeys ShortcutKeys
        {
            get { return this.shortcutKeys; }
        }
    }

    /// <summary>
    /// Represents events with shortcut update event data.
    /// </summary>
    public class ShortcutUpdateEvent : NotifyEvent
    {
        private string command;
        private ShortcutKeys[] shortcutKeys;

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutUpdateEvent"/> class.
        /// </summary>
        public ShortcutUpdateEvent(EventType type, string command, ShortcutKeys[] shortcutKeys) : base(type)
        {
            this.command = command;
            this.shortcutKeys = shortcutKeys;
        }

        /// <summary>
        /// Gets the shortcut command string associated with this <see cref="ShortcutUpdateEvent"/> object.
        /// </summary>
        public string Command
        {
            get { return this.command; }
        }

        /// <summary>
        /// Gets the array of shortcut keys associated with this <see cref="ShortcutUpdateEvent"/> object.
        /// </summary>
        public ShortcutKeys[] ShortcutKeys
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
