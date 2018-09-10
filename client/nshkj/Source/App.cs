using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using nshkj.UI.Forms;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace nshkj {
	public static class App {
        public static string APP_NAME = "凯哥 逆水寒-科举答题助手 " + Application.ProductVersion;

		private const int NSHKJ_CUP_BASE = 0xBA01;

		private static ContextMenu mContextMenu;
		private static NotifyIcon mNotifyIcon;

		private static KeyboardHookWindow mKeyboardHookWindow;

        private static string aboutURL = "http://www.homkai.com/?app=nsh-kj";
        private static string indexTip = "";

        /// <summary>
        /// Whether to render controls with a classic style (i.e. Windows XP and older) or not.
        /// </summary>
        public static bool UseClassicTheme {
			get;
			private set;
		}

		public static string AppDirectory {
			get {
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
			}
		}

		public static Logger Logger {
			get;
			private set;
		}

		public static Preferences Preferences {
			get;
			set;
		}

		/// <summary>
		/// Defines the main application entry point.
		/// </summary>
		/// <param name="args">Command-line arguments passed to the program.</param>
		[STAThread]
		public static void Main(string[] args) {
			Mutex mutex = new Mutex(false, Application.ProductName + Application.ProductVersion.ToString());

			try {
				if (!mutex.WaitOne(0, false)) {
					// another instance of the application may be running
					return;
				}

				if (args.Contains("/verbose")) {
					ConsoleManager.Show();
				}

				Logger = new Logger();
				Logger.WriteLine(LogLevel.Informational, "{0} version {1}", Application.ProductName, Application.ProductVersion);

        Directory.CreateDirectory(AppDirectory);
				FileStream stream = new FileStream(Path.Combine(AppDirectory, Application.ProductName + ".log"), args.Contains("/appendLog") ? FileMode.Append : FileMode.OpenOrCreate);
				Logger.Streams.Add(stream);

#if !DEBUG
				AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e) {
					Logger.WriteLine(LogLevel.Error, "unhandled exception: {0}", e.ExceptionObject);

					if (e.IsTerminating) {
						Logger.WriteLine(LogLevel.Warning, "CLR terminating");
						Environment.Exit(1);
					}
				};
#endif

				// Disable Windows XP-like text rendering
				Application.SetCompatibleTextRenderingDefault(false);
                
				// Initialize context menu
				mContextMenu = new ContextMenu(new MenuItem[] {
					new MenuItem("科举截图", OnScreenshotClicked) { DefaultItem = true },
					new MenuItem("-"),
					new MenuItem("设置", OnPreferencesClicked),
					new MenuItem("关于", OnAboutClicked),
					new MenuItem("-"),
					new MenuItem("退出", OnQuitClicked)
				});

				// Initialize notify icon
				mNotifyIcon = new NotifyIcon() {
					Icon = IconExtensions.ExtractFromAssembly("AppIcon", new Size(16, 16)),
					Text = APP_NAME,

					ContextMenu = mContextMenu
				};
				mNotifyIcon.MouseClick += delegate (object sender, MouseEventArgs mouseEventArgs) {
					if (mouseEventArgs.Button == MouseButtons.Left) {
						OnScreenshotClicked(sender, mouseEventArgs);
					} else if (mouseEventArgs.Button == MouseButtons.Middle) {
						OnQuitClicked(sender, mouseEventArgs);
					}
				};

				SetIconFrame(0, 16, GetNotifyIconVariant());
				mNotifyIcon.Visible = true;
                
				Preferences = Preferences.Load();
				RegisterHotKeys();

				InitializeAssetCache();
                CheckSettings();

                // start application event loop
                // 先出提示弹窗
                KejuQA kejuQA = new KejuQA();
                kejuQA.Topic = "Alt+Q截图题目即可，“拼诗句”需要截题目+9宫格候选字";
                List<string> tips = new List<string>();
                tips.Add("不要截取太多无用的东西，以免影响检索成功率");
                tips.Add(indexTip);
                kejuQA.Options = tips;
                Application.Run(KejuDialog.GetInstance(kejuQA));


            } finally {
				if (mutex != null) {
					mutex.Close();
					mutex = null;
				}
			}

            // UpdateKejuDialog(kejuQA);
        }

        private static void CheckSettings() {
            string res = "";
            try
            {
                res = App.GetHttpResponse("/api/settings"); // TODO 你自己的服务
            }
            catch (Exception e)
            {
                Console.WriteLine("checkSettings failed");
                return;
            }

            bool isOutOfDate = false;
            string versionOutOfDateTip = "";

            JArray jlist = JArray.Parse(res);
            for (int i = 0; i < jlist.Count; i++) {
                JObject setting = JObject.Parse(jlist[i].ToString());
                if (setting["key"].ToString() == "minVersion")
                {
                    Version minVersion = new Version(setting["value"].ToString());
                    int diff = minVersion.CompareTo(new Version(Application.ProductVersion));
                    if (diff > 0)
                    {
                        isOutOfDate = true;
                    }
                }
                if (setting["key"].ToString() == "versionOutOfDateTip")
                {
                    versionOutOfDateTip = setting["value"].ToString();
                }
                if (setting["key"].ToString() == "aboutURL")
                {
                    string url = setting["value"].ToString();
                    aboutURL = url + (url.Contains("?") ? "&" : "?") + "version=" + Application.ProductVersion;
                }
                if (setting["key"].ToString() == "indexTip")
                {
                    indexTip = setting["value"].ToString();
                }
            }

            if (isOutOfDate) {
                DialogResult result = MessageBox.Show(versionOutOfDateTip, "逆水寒-科举答题 启动检查", MessageBoxButtons.OK);
                if (result == DialogResult.OK) {
                    Process.Start(aboutURL);
                    Quit();
                }
            }
        }

		/// <summary>
		/// Initializes the asset cache directory
		/// </summary>
		/// <returns>Whether the operation completed successfully</returns>
		private static bool InitializeAssetCache() {
			try {
				// create cache directory
				Directory.CreateDirectory(Path.Combine(AppDirectory, "Cache"));
				Logger.WriteLine(LogLevel.Informational, "asset cache directory is OK");
			} catch {
				Logger.WriteLine(LogLevel.Warning, "error creating asset cache directory - some related features may be unavailable");
				return false;
			}

			try {
				// extract app icon
				Properties.Resources.AppIcon.Save(Path.Combine(AppDirectory, "Cache", "AppIcon.png"), ImageFormat.Png);
				return true;
			} catch {
				Logger.WriteLine(LogLevel.Warning, "error extracting AppIcon");
			}

			return false;
		}
        

		/// <summary>
		/// Initiates an action
		/// </summary>
		/// <param name="action">The Action instance</param>
		private static void InitiateAction() {
			if (CropDialog.IsOpen) {
				Logger.WriteLine(LogLevel.Warning, "crop dialog already open - ignoring");
				return;
			}

			using (CropDialog cropDialog = new CropDialog()) {
				if (cropDialog.ShowDialog() == DialogResult.OK) {
					Thread thread = new Thread(new ThreadStart(delegate {
						using (Bitmap bitmap = GDI.CaptureBitmapFromScreen(cropDialog.Area)) {
							// Disable context menu items so no concurrent captures can be made
							mContextMenu.MenuItems[0].Enabled = false;
							mContextMenu.MenuItems[1].Enabled = false;

                            // Perform the action
                            new Action().Process(bitmap);

                            // Re-enable the context menu items again
                            mContextMenu.MenuItems[0].Enabled = true;
							mContextMenu.MenuItems[1].Enabled = true;
						}
					}));

					thread.SetApartmentState(ApartmentState.STA);
					thread.Start();
				}
			}
		}

		/// <summary>
		/// Event handler triggered when the "Take screenshot" context menu item gets activated.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">Event arguments.</param>
		private static void OnScreenshotClicked(object sender, EventArgs e) {
			// initiate default action
			InitiateAction();
		}
        

        /// <summary>
		/// Updates Keju dialog
		/// </summary>
		public static void UpdateKejuDialog(KejuQA kejuQA)
        {
            KejuDialog dialog = KejuDialog.GetInstance(kejuQA);

            if (!KejuDialog.IsOpen)
            {
                dialog.ShowDialog();
                return;
            }

            dialog.Invoke(new MethodInvoker(() => {
                dialog.WindowState = FormWindowState.Normal;
                dialog.Activate();
                dialog.UpdateDialog(kejuQA);
            }));
        }


        /// <summary>
        /// Event handler triggered when the "Preferences" context menu item gets activated.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPreferencesClicked(object sender, EventArgs e) {
			if (PreferencesDialog.IsOpen) {
				Logger.WriteLine(LogLevel.Warning, "preferences dialog already open - ignoring");
				return;
			}

			using (PreferencesDialog preferencesDialog = new PreferencesDialog()) {
				preferencesDialog.ShowDialog();
			}
		}

		/// <summary>
		/// Event handler triggered when the "About" context menu item gets activated.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">Event arguments.</param>
		private static void OnAboutClicked(object sender, EventArgs e) {
            Process.Start(aboutURL);
		}

        private static void Quit() {
            UnregisterHotKeys();

            mNotifyIcon.Visible = false;
            mNotifyIcon.Dispose();

            Environment.Exit(0);
        }

		/// <summary>
		/// Event handler triggered when the "Quit" context menu item gets activated.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">Event arguments.</param>
		private static void OnQuitClicked(object sender, EventArgs e) {
            Quit();
		}

		public static void SetIconFrame(int frame, int squareSize = 16, int variant = 0) {
			try {
				using (Bitmap bitmap = new Bitmap(squareSize, squareSize)) {
					using (Graphics graphics = Graphics.FromImage(bitmap)) {
						graphics.DrawImage(Properties.Resources.TrayIconStrip, new Rectangle(0, 0, squareSize, squareSize), new Rectangle(frame * squareSize, variant * squareSize, squareSize, squareSize), GraphicsUnit.Pixel);
					}

					mNotifyIcon.Icon = Icon.FromHandle(bitmap.GetHicon());
				}
			} catch { }
		}

		/// <summary>
		/// Returns the tray icon variant based on the current platform version.
		/// </summary>
		public static int GetNotifyIconVariant() {
			if (Environment.OSVersion.Version.Major < 6) {
				// Windows XP (5.1) and older (first row)
				return 0;
			} else if (Environment.OSVersion.Version.Major >= 10) {
				// Windows 10 (10.0, fourth row)
				return 3;
			} else if (Environment.OSVersion.Version.Minor >= 2) {
				// Windows 8 and fist Windows 10 Previews (>= 6.2, third row)
				return 2;
			}

			// Windows Vista and 7 (6.0, 6.1), second row (default)
			return 1;
		}
        

		/// <summary>
		/// Registers hot keys
		/// </summary>
		public static void RegisterHotKeys() {
			mKeyboardHookWindow = new KeyboardHookWindow();

			int hotKeyId = NSHKJ_CUP_BASE;
			List<string> keysToRemove = new List<string>();

            try
            {
                Action action = new Action();
                HotKey hotKey = HotKey.Parse(Preferences.CuptureHotKey);

                if (hotKey != HotKey.Nil)
                {
                    if (mKeyboardHookWindow.RegisterHotKey(hotKeyId, hotKey))
                    {
                        Logger.WriteLine(LogLevel.Informational, "registered hotkey " + hotKey);
                    }
                    else
                    {
                        Logger.WriteLine(LogLevel.Warning, "unable to register hotkey - ignoring");
                    }
                }
            }
            catch
            {
                // may be an invalid key value pair - remove kebab
                Logger.WriteLine(LogLevel.Warning, "invalid hot key detected - removing " + Preferences.CuptureHotKey);
                Preferences.CuptureHotKey = HotKey.Nil.Encode();
                Preferences.Save();
            }

			mKeyboardHookWindow.OnKeyboardHookTriggered += OnKeyboardHookTriggered;
		}

		/// <summary>
		/// Unregisters previously reigstered hotkeys, if any
		/// </summary>
		public static void UnregisterHotKeys() {
			if (mKeyboardHookWindow == null)
				return;
            
			mKeyboardHookWindow.OnKeyboardHookTriggered -= OnKeyboardHookTriggered;
		}

		/// <summary>
		/// Event triggered when a hot key message has been received
		/// </summary>
		/// <param name="sender">The sender of the event</param>
		/// <param name="e">Arguments for this event</param>
		private static void OnKeyboardHookTriggered(object sender, KeyboardHookTriggeredEventArgs e) {
			Logger.WriteLine(LogLevel.Debug, "keyboard hook 0x{0:X2} triggered", e.ID);

            InitiateAction();
        }


        public static string GetHttpResponse(string url, int Timeout = 6000)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = null;
            request.Timeout = Timeout;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
    }
}
