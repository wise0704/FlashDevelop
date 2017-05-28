using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace PluginCore.Helpers
{
    /// <summary>
    /// Provides a <see cref="TypeConverter"/> to convert <see cref="ShortcutKey"/> objects to and from other representations.
    /// </summary>
    public class ShortcutKeyConverter : TypeConverter
    {
        private const string Alt = "Alt+";
        private const string Ctrl = "Ctrl+";
        private const string Shift = "Shift+";

        private static Dictionary<Keys, string> nameTable;
        private static Dictionary<string, Keys> keyTable;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutKeyConverter"/> class.
        /// </summary>
        public ShortcutKeyConverter()
        {

        }

        #endregion

        #region TypeConverter Overrides

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string)
                || sourceType == typeof(Keys) || sourceType == typeof(Keys[])
                || sourceType == typeof(Shortcut) || sourceType == typeof(Shortcut[])
                || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to.</param>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType)
                || destinationType == typeof(Keys) || destinationType == typeof(Keys[])
                || destinationType == typeof(Shortcut) || destinationType == typeof(Shortcut[]);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="object"/> to convert.</param>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return ConvertFromString((string) value);
            }
            if (value is Keys)
            {
                return (ShortcutKey) (Keys) value;
            }
            if (value is Keys[])
            {
                var array = (Keys[]) value;
                switch (array.Length)
                {
                    case 0: return new ShortcutKey();
                    case 1: return new ShortcutKey(array[0]);
                    case 2: return new ShortcutKey(array[0], array[1]);
                    default:
                        throw new ArgumentException("Length of the specified array is out of range.", nameof(value));
                }
            }
            if (value is Shortcut)
            {
                return (ShortcutKey) (Shortcut) value;
            }
            if (value is Shortcut[])
            {
                var array = (Shortcut[]) value;
                switch (array.Length)
                {
                    case 0: return new ShortcutKey();
                    case 1: return new ShortcutKey(array[0]);
                    case 2: return new ShortcutKey(array[0], array[1]);
                    default:
                        throw new ArgumentException("Length of the specified array is out of range.", nameof(value));
                }
            }
            if (value == null)
            {
                return ShortcutKey.None;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="CultureInfo"/>. If <code>null</code> is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="Type"/> to convert the <code>value</code> parameter to.</param>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            if (value is ShortcutKey)
            {
                var key = (ShortcutKey) value;
                if (destinationType == typeof(string))
                {
                    return ConvertToString(key);
                }
                if (destinationType == typeof(Keys))
                {
                    return (Keys) key;
                }
                if (destinationType == typeof(Keys[]))
                {
                    return new[] { key.First, key.Second };
                }
                if (destinationType == typeof(Shortcut))
                {
                    return (Shortcut) key;
                }
                if (destinationType == typeof(Shortcut[]))
                {
                    return new[] { (Shortcut) key.First, (Shortcut) key.Second };
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts a <see cref="string"/> to <see cref="ShortcutKey"/>.
        /// </summary>
        /// <param name="value">A <see cref="string"/> to convert.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public new static ShortcutKey ConvertFromString(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (keyTable == null)
            {
                Initialize();
            }
            return ConvertFromStringInternal(value);
        }

        /// <summary>
        /// Converts a <see cref="string"/> to <see cref="ShortcutKey"/>. A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">A <see cref="string"/> to convert.</param>
        /// <param name="result">
        /// When this method returns, contains the <see cref="ShortcutKey"/> value equivalent of the value represented in the specified string, if the conversion succeeded, or <see cref="ShortcutKey.None"/> if the conversion failed.
        /// The conversion fails if the specified string is <see langword="null"/> or <see cref="string.Empty"/>, or is not of the correct format.
        /// This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        public static bool TryConvertFromString(string value, out ShortcutKey result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = ShortcutKey.None;
                return false;
            }
            if (keyTable == null)
            {
                Initialize();
            }
            return TryConvertFromStringInternal(value, out result);
        }

        /// <summary>
        /// Converts a <see cref="ShortcutKey"/> value to <see cref="string"/>.
        /// </summary>
        /// <param name="key">A <see cref="ShortcutKey"/> value to convert.</param>
        public static string ConvertToString(ShortcutKey key)
        {
            if (nameTable == null)
            {
                Initialize();
            }
            if (key.IsExtended)
            {
                return ConvertToStringInternal(key.First) + ", " + ConvertToStringInternal(key.Second);
            }
            return ConvertToStringInternal(key.First);
        }

        /// <summary>
        /// Converts a <see cref="Keys"/> value to <see cref="string"/>.
        /// </summary>
        /// <param name="key">A <see cref="Keys"/> value to convert.</param>
        public static string ConvertToString(Keys key)
        {
            if (nameTable == null)
            {
                Initialize();
            }
            return ConvertToStringInternal(key);
        }

        /// <summary>
        /// Converts a <see cref="Shortcut"/> value to <see cref="string"/>.
        /// </summary>
        /// <param name="key">A <see cref="Shortcut"/> value to convert.</param>
        public static string ConvertToString(Shortcut key)
        {
            if (nameTable == null)
            {
                Initialize();
            }
            return ConvertToStringInternal((Keys) key);
        }

        #endregion

        #region Private Methods

        private static void Initialize()
        {
            nameTable = new Dictionary<Keys, string>(49);
            keyTable = new Dictionary<string, Keys>(49);
            Add(Keys.Back, "Backspace");
            Add(Keys.Enter, "Enter"); // Keys.Return
            Add(Keys.Pause, "Break");
            Add(Keys.CapsLock, "CapsLock"); // Keys.Capital
            Add(Keys.HangulMode, "HangulMode"); // Keys.HanguelMode, Keys.KannaMode
            Add(Keys.HanjaMode, "HanjaMode"); // Keys.KanjiMode
            Add(Keys.PageUp, "PgUp"); // Keys.Prior
            Add(Keys.PageDown, "PgDn"); // Keys.Next
            Add(Keys.Insert, "Ins");
            Add(Keys.Delete, "Del");
            Add(Keys.D0, "0");
            Add(Keys.D1, "1");
            Add(Keys.D2, "2");
            Add(Keys.D3, "3");
            Add(Keys.D4, "4");
            Add(Keys.D5, "5");
            Add(Keys.D6, "6");
            Add(Keys.D7, "7");
            Add(Keys.D8, "8");
            Add(Keys.D9, "9");
            Add(Keys.NumPad0, "Num 0");
            Add(Keys.NumPad1, "Num 1");
            Add(Keys.NumPad2, "Num 2");
            Add(Keys.NumPad3, "Num 3");
            Add(Keys.NumPad4, "Num 4");
            Add(Keys.NumPad5, "Num 5");
            Add(Keys.NumPad6, "Num 6");
            Add(Keys.NumPad7, "Num 7");
            Add(Keys.NumPad8, "Num 8");
            Add(Keys.NumPad9, "Num 9");
            Add(Keys.Multiply, "Num *");
            Add(Keys.Add, "Num +");
            Add(Keys.Subtract, "Num -");
            Add(Keys.Decimal, "Num .");
            Add(Keys.Divide, "Num /");
            Add(Keys.OemSemicolon, ";"); // Keys.Oem1
            Add(Keys.Oemplus, "=");
            Add(Keys.Oemcomma, ",");
            Add(Keys.OemMinus, "-");
            Add(Keys.OemPeriod, ".");
            Add(Keys.OemQuestion, "/"); // Keys.Oem2
            Add(Keys.Oemtilde, "`"); // Keys.Oem3
            Add(Keys.OemOpenBrackets, "["); // Keys.Oem4
            Add(Keys.OemPipe, "\\"); // Keys.Oem5
            Add(Keys.OemCloseBrackets, "]"); // Keys.Oem6
            Add(Keys.OemQuotes, "'"); // Keys.Oem7
            Add(Keys.Shift, "Shift");
            Add(Keys.Control, "Ctrl");
            Add(Keys.Alt, "Alt");
        }

        private static void Add(Keys key, string name)
        {
            nameTable.Add(key, name);
            keyTable.Add(name, key);
        }

        private static ShortcutKey ConvertFromStringInternal(string value)
        {
            int index = 0;
            bool extended = false;
            var first = Keys.None;
            var second = Keys.None;
            int length = value.Length;

            for (int i = 0; i < length; i++)
            {
                switch (value[i])
                {
                    case '+':
                        if (i < 4 || string.CompareOrdinal(value, i - 4, "Num +", 0, 4) != 0)
                        {
                            if (extended)
                            {
                                second |= GetKey(value.Substring(index, i - index));
                            }
                            else
                            {
                                first |= GetKey(value.Substring(index, i - index));
                            }
                            do
                            {
                                if (++i == length)
                                {
                                    throw new FormatException($"Input string was not in a correct format. Missing part after '{value[i]}'");
                                }
                            }
                            while (char.IsWhiteSpace(value[i]));
                            index = i--;
                        }
                        break;

                    case ',':
                        if (index != i)
                        {
                            if (extended)
                            {
                                throw new FormatException($"Input string was not in a correct format. {nameof(ShortcutKey)} cannot have more than two parts.");
                            }
                            else
                            {
                                first |= GetKey(value.Substring(index, i - index));
                            }
                            do
                            {
                                if (++i == length)
                                {
                                    throw new FormatException($"Input string was not in a correct format. Missing part after '{value[i]}'");
                                }
                            }
                            while (char.IsWhiteSpace(value[i]));
                            index = i--;
                            extended = true;
                        }
                        break;
                }
            }

            if (extended)
            {
                second |= GetKey(value.Substring(index, length - index));
            }
            else
            {
                first |= GetKey(value.Substring(index, length - index));
            }

            return new ShortcutKey(first, second);
        }

        private static bool TryConvertFromStringInternal(string value, out ShortcutKey key)
        {
            int index = 0;
            bool extended = false;
            var first = Keys.None;
            var second = Keys.None;
            var result = Keys.None;
            int length = value.Length;

            for (int i = 0; i < length; i++)
            {
                switch (value[i])
                {
                    case '+':
                        if (i < 4 || string.CompareOrdinal(value, i - 4, "Num +", 0, 4) != 0)
                        {
                            if (extended)
                            {
                                if (!TryGetKey(value.Substring(index, i - index), out result))
                                {
                                    key = ShortcutKey.None;
                                    return false;
                                }
                                second |= result;
                            }
                            else
                            {
                                if (!TryGetKey(value.Substring(index, i - index), out result))
                                {
                                    key = ShortcutKey.None;
                                    return false;
                                }
                                first |= result;
                            }
                            do
                            {
                                if (++i == length)
                                {
                                    key = ShortcutKey.None;
                                    return false;
                                }
                            }
                            while (char.IsWhiteSpace(value[i]));
                            index = i--;
                        }
                        break;

                    case ',':
                        if (index != i)
                        {
                            if (extended)
                            {
                                key = ShortcutKey.None;
                                return false;
                            }
                            else
                            {
                                if (!TryGetKey(value.Substring(index, i - index), out result))
                                {
                                    key = ShortcutKey.None;
                                    return false;
                                }
                                first |= result;
                            }
                            do
                            {
                                if (++i == length)
                                {
                                    key = ShortcutKey.None;
                                    return false;
                                }
                            }
                            while (char.IsWhiteSpace(value[i]));
                            index = i--;
                            extended = true;
                        }
                        break;
                }
            }

            if (!TryGetKey(value.Substring(index, length - index), out result))
            {
                key = ShortcutKey.None;
                return false;
            }

            if (extended)
            {
                second |= result;
            }
            else
            {
                first |= result;
            }

            if (first == Keys.None && second != Keys.None)
            {
                key = ShortcutKey.None;
                return false;
            }

            key = new ShortcutKey(first, second);
            return true;
        }

        private static string ConvertToStringInternal(Keys key)
        {
            // For performance reasons, instead of using a string or StringBuilder buffer and appending
            // text such as Ctrl, Alt and Shift on it, use the string concatenation to utilize the compiler optimization,
            // which turns them into string.Concat() calls.
            if ((key & Keys.Control) != Keys.None)
            {
                if ((key & Keys.Alt) != Keys.None)
                {
                    if ((key & Keys.Shift) != Keys.None)
                    {
                        return Ctrl + Alt + Shift + GetName(key & Keys.KeyCode);
                    }
                    return Ctrl + Alt + GetName(key & Keys.KeyCode);
                }
                if ((key & Keys.Shift) != Keys.None)
                {
                    return Ctrl + Shift + GetName(key & Keys.KeyCode);
                }
                return Ctrl + GetName(key & Keys.KeyCode);
            }
            if ((key & Keys.Alt) != Keys.None)
            {
                if ((key & Keys.Shift) != Keys.None)
                {
                    return Alt + Shift + GetName(key & Keys.KeyCode);
                }
                return Alt + GetName(key & Keys.KeyCode);
            }
            if ((key & Keys.Shift) != Keys.None)
            {
                return Shift + GetName(key & Keys.KeyCode);
            }
            return GetName(key);
        }

        private static Keys GetKey(string name)
        {
            Keys key;
            if (keyTable.TryGetValue(name, out key))
            {
                return key;
            }

            try
            {
                try
                {
                    return (Keys) Enum.Parse(typeof(Keys), name);
                }
                catch
                {
                    return (Keys) (Shortcut) Enum.Parse(typeof(Shortcut), name);
                }
            }
            catch (Exception e)
            {
                throw new FormatException($"Input string was not in a correct format. '{name}' is not a named constant defined for {nameof(ShortcutKey)}.", e);
            }
        }

        private static bool TryGetKey(string name, out Keys result)
        {
            if (keyTable.TryGetValue(name, out result))
            {
                return true;
            }

            // TODO: Use TryParse with .NET 4+
            try
            {
                try
                {
                    result = (Keys) Enum.Parse(typeof(Keys), name);
                }
                catch
                {
                    result = (Keys) (Shortcut) Enum.Parse(typeof(Shortcut), name);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetName(Keys key)
        {
            string name;
            if (nameTable.TryGetValue(key, out name))
            {
                return name;
            }

            return Enum.GetName(typeof(Keys), key);
        }

        #endregion
    }
}
