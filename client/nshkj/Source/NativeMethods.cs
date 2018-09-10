using System;
using System.Runtime.InteropServices;

namespace nshkj
{
	public static class NativeMethods {
		// Window messages.
		public const int WM_SYSCOLORCHANGE = 0x15;
		public const int WM_SETCURSOR = 0x20;
		public const int WM_HOTKEY = 0x312;
		public const int WM_THEMECHANGED = 0x31A;

		// Hand cursor.
		public const int IDC_HAND = 0x7F89;

		// Extended window styles
		public const int WS_EX_COMPOSITED = 0x2;

		// GDI+ ternary raster operations
		public const uint SRCCOPY = 0xCC0020;

		// Cursor P/Invokes (UI/Controls/LinkLabelEx.cs)
		[DllImport("user32.dll")]
		public static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

		[DllImport("user32.dll")]
		public static extern IntPtr SetCursor(IntPtr hCursor);

		// GDI P/Invokes (Source/Capture/GDI.cs)
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hobject);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		// UxTheme P/Invokes (Source/App.cs)
		[DllImport("uxtheme.dll")]
		public static extern bool IsThemeActive();

		// Hotkey P/Invokes (Source/KeyboardHookWindow.cs)
		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetStdHandle(uint nStdHandle);

		[DllImport("kernel32.dll")]
		public static extern void SetStdHandle(uint nStdHandle, IntPtr handle);
		[DllImport("kernel32.dll")]
		public static extern int AllocConsole();

		public const uint STD_OUTPUT_HANDLE = 0xFFFFFFF5;
	}
}
