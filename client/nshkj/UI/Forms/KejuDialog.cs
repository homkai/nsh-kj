using System;
using System.Windows.Forms;

namespace nshkj.UI.Forms
{
    public partial class KejuDialog : Form
    {
        
        /// <summary>
        /// Determines whether there's another assigned instance of this Form or not.
        /// </summary>
        public static bool IsOpen
        {
            get;
            private set;
        }

        private static KejuDialog _instance = null;

        public static KejuDialog GetInstance(KejuQA kejuQA)
        {
            if (_instance == null)
            {
                _instance = new KejuDialog();
                _instance.UpdateDialog(kejuQA);
            }

            return _instance;
        }

        protected KejuDialog()
        {
            InitializeComponent();
        }

        public void UpdateDialog(KejuQA kejuQA)
        {
            ocrTextBox.Text = String.Join("\r\n", kejuQA.Topic, String.Join("\r\n", kejuQA.Options));
            ocrTextBox.SelectionStart = 0;
            ocrTextBox.SelectionLength = 0;
            answerRTextBox.Text = String.IsNullOrEmpty(kejuQA.Answer) ? "" : kejuQA.Answer;
        }


        /// <summary>
        /// Raises the Form.FormClosed event
        /// </summary>
        /// <param name="e">Arguments for this event</param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            KejuDialog.IsOpen = false;
            _instance = null;
            base.OnFormClosed(e);
        }

        /// <summary>
        /// Raises the Form.Shown event
        /// </summary>
        /// <param name="e">Arguments for this event</param>
        protected override void OnShown(EventArgs e)
        {
            KejuDialog.IsOpen = true;
            base.OnShown(e);
        }
    }
}
