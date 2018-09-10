using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nshkj
{
	/// <summary>
	/// High-level wrapper for the Windows API hot keys used in RegisterHotKey and UnregisterHotKey
	/// </summary>
	public struct HotKey {
		public ModifierKeys modifiers;
		public Keys keys;

		public static HotKey Nil = new HotKey();

		/// <summary>
		/// Initializer for the structure
		/// </summary>
		/// <param name="modifiers">Modifier keys</param>
		/// <param name="keys">Key values</param>
		public HotKey(Keys modifiers = 0, Keys keys = 0) {
			this.modifiers = GetModifierKeys(modifiers);
			this.keys = keys;
		}

		/// <summary>
		/// Encodes a hot key for storing in registry.
		/// </summary>
		/// <returns>The encoded hot key string.</returns>
		public string Encode() {
			string result = "";

			if (((int)modifiers & (int)ModifierKeys.Alt) != 0)
				result += "A";

			if (((int)modifiers & (int)ModifierKeys.Control) != 0)
				result += "C";

			if (((int)modifiers & (int)ModifierKeys.Shift) != 0)
				result += "S";

			if (((int)modifiers & (int)ModifierKeys.Win) != 0)
				result += "W";

			return result + ":" + keys.ToString();
		}

		/// <summary>
		/// Format hot key in a human-readable way.
		/// </summary>
		/// <param name="hotKey">The hotKey structure to be formatted.</param>
		/// <returns>The human-readable hotKey string.</returns>
		public override string ToString() {
			if (this == Nil)
				return "未设置快捷键";

			string result = "";

			if (((int)modifiers & (int)ModifierKeys.Alt) != 0)
				result += "Alt + ";

			if (((int)modifiers & (int)ModifierKeys.Control) != 0)
				result += "Ctrl + ";

			if (((int)modifiers & (int)ModifierKeys.Shift) != 0)
				result += "Shift + ";

			if (((int)modifiers & (int)ModifierKeys.Win) != 0)
				result += "Win + ";

			return result + keys.ToString();
		}

		/// <summary>
		/// Compares two hotkeys
		/// </summary>
		/// <param name="a">First hotkey</param>
		/// <param name="b">Second hotkey</param>
		/// <returns>Whether the hotkeys are equal</returns>
		public static bool operator ==(HotKey a, HotKey b) {
			return a.keys == b.keys && a.modifiers == b.modifiers;
		}

		/// <summary>
		/// Compares two hotkeys
		/// </summary>
		/// <param name="a">First hotkey</param>
		/// <param name="b">Second hotkey</param>
		/// <returns>Whether the hotkeys are distinct</returns>
		public static bool operator !=(HotKey a, HotKey b) {
			return !(a == b);
		}

		/// <summary>
		/// Retrieve a ModifierKeys set from a Keys set.
		/// </summary>
		/// <param name="keys">Value of type keys containing the pressed keys.</param>
		/// <returns>A ModifierKeys containing the modifiers extracted from keys.</returns>
		public static ModifierKeys GetModifierKeys(Keys keys) {
			int result = 0;

			if ((keys & Keys.Alt) != 0)
				result |= (int)ModifierKeys.Alt;

			if ((keys & Keys.Control) != 0)
				result |= (int)ModifierKeys.Control;

			if ((keys & Keys.Shift) != 0)
				result |= (int)ModifierKeys.Shift;

			if ((keys & Keys.LWin) != 0 || (keys & Keys.RWin) != 0)
				result |= (int)ModifierKeys.Win;


			return (ModifierKeys)result;
		}

		/// <summary>
		/// Parse an encoded hot key from the settings file
		/// </summary>
		/// <param name="value">The encoded value</param>
		/// <returns>A HotKey structure with the parsed values</returns>
		public static HotKey Parse(string value) {
			if (String.IsNullOrEmpty(value))
				return Nil;

			HotKey hotKey = new HotKey();
			string[] split = value.Split(':');

			if (split[0].Contains("A"))
				hotKey.modifiers |= ModifierKeys.Alt;

			if (split[0].Contains("C"))
				hotKey.modifiers |= ModifierKeys.Control;

			if (split[0].Contains("S"))
				hotKey.modifiers |= ModifierKeys.Shift;

			if (split[0].Contains("W"))
				hotKey.modifiers |= ModifierKeys.Win;

			hotKey.keys = (Keys)Enum.Parse(typeof(Keys), split[1]);

			return hotKey;
		}
	}
}
