using System;
using System.Windows.Forms;

namespace nshkj
{
	public class KeyboardHookWindow : NativeWindow, IDisposable {
		public delegate void KeyboardHookTriggeredEventHandler(object sender, KeyboardHookTriggeredEventArgs e);
		public event KeyboardHookTriggeredEventHandler OnKeyboardHookTriggered;

		/// <summary>
		/// Initializer for this class.
		/// </summary>
		public KeyboardHookWindow() {
			// Create window handle, as RegisterHotKey needs it
			base.CreateHandle(new CreateParams());
		}

		/// <summary>
		/// Release all allocated resources
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Class destructor
		/// </summary>
		~KeyboardHookWindow() {
			Dispose(true);
		}

		/// <summary>
		/// Release all allocated resources
		/// </summary>
		/// <param name="disposing">Whether to release managed resources or not</param>
		protected virtual void Dispose(bool disposing) {
			DestroyHandle();
		}

		/// <summary>
		/// Register a new hot key given its ID, the modifier keys and the key data.
		/// </summary>
		/// <param name="id">Unique identifier for this hot key</param>
		/// <param name="hotKey">The HotKey to be registered</param>
		public bool RegisterHotKey(int id, HotKey hotKey) {
			return NativeMethods.RegisterHotKey(Handle, id, (uint)hotKey.modifiers, (uint)hotKey.keys);
		}

		/// <summary>
		/// Unregisters a previously registered hot key given its ID.
		/// </summary>
		/// <param name="id">The ID of the hot key.</param>
		public bool UnregisterHotKey(int id) {
			return NativeMethods.UnregisterHotKey(Handle, id);
		}

		/// <summary>
		/// Processes Windows messages
		/// </summary>
		/// <param name="msg">The meessage being received</param>
		protected override void WndProc(ref Message msg) {
			if (msg.Msg == NativeMethods.WM_HOTKEY && OnKeyboardHookTriggered != null) {
				OnKeyboardHookTriggered(this, new KeyboardHookTriggeredEventArgs(msg.WParam.ToInt32()));
			}

			base.WndProc(ref msg);
		}
	}
}