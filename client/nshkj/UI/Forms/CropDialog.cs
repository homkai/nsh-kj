using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace nshkj
{
	public class CropDialog : Form {
		private Rectangle mArea;
		private bool mMouseDown;
		private int mInitialX, mInitialY;

		/// <summary>
		/// Area selected by the user.
		/// </summary>
		public Rectangle Area {
			get {
				return mArea;
			}
		}

		/// <summary>
		/// Determines whether there's another assigned instance of this Form or not.
		/// </summary>
		public static bool IsOpen {
			get;
			private set;
		}

		/// <summary>
		/// GTFO, flickering.
		/// </summary>
		protected override CreateParams CreateParams {
			get {
				CreateParams handleParam = base.CreateParams;

				handleParam.ExStyle |= NativeMethods.WS_EX_COMPOSITED;
				return handleParam;
			}
		}

		/// <summary>
		/// Initializer for this class.
		/// </summary>
		public CropDialog() {
			DoubleBuffered = true;
			BackColor = Color.Black;
			Opacity = .5d;
			TopMost = true;
			FormBorderStyle = FormBorderStyle.None;
			ShowIcon = false;
			ShowInTaskbar = false;
			Bounds = Screen.FromControl(this).Bounds;
			AllowTransparency = true;
			TransparencyKey = Color.White;

			// HACK: prevent dialog to be shown twice, as a fast-enough user could open it more than once before IsOpen is set to true on Shown event handler
			IsOpen = true;

			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

		/// <summary>
		/// Draws the selected area.
		/// </summary>
		/// <param name="e">Arguments for this event.</param>
		protected override void OnPaint(PaintEventArgs e) {
			if (mArea.Width > 0 && mArea.Height > 0) {
				e.Graphics.SmoothingMode = SmoothingMode.None;
				e.Graphics.FillRectangle(new SolidBrush(TransparencyKey), mArea);
			}

			base.OnPaint(e);
		}

		/// <summary>
		/// Override background paint event handler by a no-op, so we gain a bit more performance.
		/// </summary>
		/// <param name="e">Arguments for this event.</param>
		protected override void OnPaintBackground(PaintEventArgs e) { }

		/// <summary>
		/// Dismisses the crop dialog when a key is pressed.
		/// </summary>
		/// <param name="e">Arguments for this event.</param>
		protected override void OnKeyDown(KeyEventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();

			base.OnKeyDown(e);
		}

		/// <summary>
		/// Dismisses the crop dialog when the overlay gets clicked.
		/// </summary>
		/// <param name="e">Arguments for this event.</param>
		protected override void OnMouseUp(MouseEventArgs e) {
			if (mArea.Width == 0 && mArea.Height == 0) {
				DialogResult = DialogResult.Cancel;
				Close();
			} else {
				DialogResult = DialogResult.OK;
				Close();
			}

			base.OnMouseUp(e);
		}

		/// <summary>
		/// Sets initial X and Y coordinates for the mouse when user started dragging mouse.
		/// </summary>
		/// <param name="e">Arguments for this event.</param>
		protected override void OnMouseDown(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				mMouseDown = true;
				mInitialX = e.X;
				mInitialY = e.Y;
			} else {
				DialogResult = DialogResult.Cancel;
				Close();
			}

			base.OnMouseDown(e);
		}

		/// <summary>
		/// Handle mouse movement events for changing area panel bounds when the mouse is being dragged.
		/// </summary>
		/// <param name="mEventArgs"></param>
		protected override void OnMouseMove(MouseEventArgs mEventArgs) {
			if (mMouseDown) {
				Invalidate(mArea, false);

				if (mInitialX < mEventArgs.X) {
					mArea.X = mInitialX;
					mArea.Width = mEventArgs.X - mInitialX;
				} else {
					mArea.X = mEventArgs.X;
					mArea.Width = mInitialX - mEventArgs.X;
				}

				if (mInitialY < mEventArgs.Y) {
					mArea.Y = mInitialY;
					mArea.Height = mEventArgs.Y - mInitialY;
				} else {
					mArea.Y = mEventArgs.Y;
					mArea.Height = mInitialY - mEventArgs.Y;
				}

				Invalidate(mArea, false);
			}

			base.OnMouseMove(mEventArgs);
		}

		/// <summary>
		/// Raises the Form.FormClosed event
		/// </summary>
		/// <param name="e">Arguments for this event</param>
		protected override void OnFormClosed(FormClosedEventArgs e) {
			IsOpen = false;
			base.OnFormClosed(e);
		}
	}
}