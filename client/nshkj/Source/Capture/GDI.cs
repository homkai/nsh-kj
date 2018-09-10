using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace nshkj
{
	public static class GDI {
		/// <summary>
		/// Copy data from screen and save it to a Bitmap.
		/// </summary>
		/// <param name="area">The area to be captured from the screen.</param>
		/// <returns>The cropped bitmap of the specified area.</returns>
		public static Bitmap CaptureBitmapFromScreen(Rectangle area) {
			IntPtr desktop = NativeMethods.GetDesktopWindow(),
						 src = NativeMethods.GetWindowDC(desktop),
						 dest = NativeMethods.CreateCompatibleDC(src),
						 hbitmap = NativeMethods.CreateCompatibleBitmap(src, area.Width, area.Height);

			NativeMethods.SelectObject(dest, hbitmap);
			NativeMethods.BitBlt(dest, 0, 0, area.Width, area.Height, src, area.X, area.Y, NativeMethods.SRCCOPY);

			Bitmap bitmap = Image.FromHbitmap(hbitmap);

			NativeMethods.ReleaseDC(desktop, src);
			NativeMethods.ReleaseDC(desktop, dest);
			NativeMethods.DeleteObject(hbitmap);

			return bitmap;
		}
	}
}