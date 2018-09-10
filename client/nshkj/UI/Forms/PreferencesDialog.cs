using System;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace nshkj {
	public partial class PreferencesDialog : Form {
		/// <summary>
		/// Determines whether there's another assigned instance of this Form or not.
		/// </summary>
		public static bool IsOpen {
			get;
			private set;
		}

		/// <summary>
		/// Initializer method for this class.
		/// </summary>
		public PreferencesDialog() {
			InitializeComponent();
            
			// Read settings
			if (App.Preferences.CuptureHotKey.Length > 0) {
				screenshotHotKeyTextBox.Text = HotKey.Parse(App.Preferences.CuptureHotKey).ToString();
			} else {
				screenshotHotKeyTextBox.Text = "未设置快捷键";
			}
		}
        

		/// <summary>
		/// Displays the keys pressed by the user.
		/// </summary>
		/// <param name="sender">Sender object.</param>
		/// <param name="e">Arguments for this event.</param>
		private void screenshotHotKeyTextBox_KeyDown(object sender, KeyEventArgs e) {
			e.SuppressKeyPress = true;

			HotKey hotKey = e.Modifiers == Keys.None ? HotKey.Nil : new HotKey(e.Modifiers, e.KeyCode);
			string encodedHotKey = hotKey.Encode();

			if (hotKey != HotKey.Nil) {
                Console.WriteLine("encodedHotKey " + encodedHotKey);
				App.Preferences.CuptureHotKey = encodedHotKey;
			} else {
				App.Preferences.CuptureHotKey = HotKey.Nil.Encode();
			}

			screenshotHotKeyTextBox.Text = hotKey.ToString();
		}
        
     
		/// <summary>
		/// Raises the Form.Shown event.
		/// </summary>
		/// <param name="e">Arguments for this event.</param>
		protected override void OnShown(EventArgs e) {
			IsOpen = true;
			base.OnShown(e);
		}

		/// <summary>
		/// Raises the Form.FormClosing event.
		/// </summary>
		/// <param name="e">Arguments for this event.</param>
		protected override void OnFormClosing(FormClosingEventArgs e) {
			IsOpen = false;
			Hide();
            App.Preferences.Save();
			App.UnregisterHotKeys();
			App.RegisterHotKeys();
			App.Logger.WriteLine(LogLevel.Debug, "settings saved");
			base.OnFormClosing(e);
		}
        
		/// <summary>
		/// Unregister hotkeys so they are not triggered while the focus is on the hotkey input
		/// </summary>
		/// <param name="sender">Sender of this event</param>
		/// <param name="e">Arguments for this event</param>
		private void screenshotHotKeyTextBox_Enter(object sender, EventArgs e) {
			App.UnregisterHotKeys();
		}

		/// <summary>
		/// Re-registers hotkeys after leaving focus on the hotkey input
		/// </summary>
		/// <param name="sender">Sender of this event</param>
		/// <param name="e">Arguments for this event</param>
		private void screenshotHotKeyTextBox_Leave(object sender, EventArgs e) {
			App.RegisterHotKeys();
		}
        
        
	}
}
