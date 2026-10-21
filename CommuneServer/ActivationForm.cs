using CommuneServer.Activation;
using CommuneServer.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace CommuneServer
{
    public partial class ActivationForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int HT_CAPTION = 0x0002;

        [DllImport("user32.dll")]
        public static extern int SendMessage(
            IntPtr hWnd,
            int Msg,
            int wParam,
            int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public bool activated = false;
        public ActivationForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "ملف التفعيل";
            openFileDialog.Filter = "Key File (*.enc)|*.enc";
            openFileDialog.Multiselect = false;
            openFileDialog.InitialDirectory = @"C:\";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                textBoxKey.Text = path;
            }
        }

        private void buttonActivation_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBoxKey.Text))
            {
                MessageBox.Show("الملف غير موجود", "حصل خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxCode.Text))
            {
                MessageBox.Show("الرجاء إدخال رمز التفعيل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxCode.Focus();
                return;
            }



            CommuneDbContext db = new CommuneDbContext();
            ActivationService activationService = new ActivationService();

         
            try
            {
                var result = activationService.Activate(textBoxKey.Text, textBoxCode.Text, db);

                if (result.IsSuccess)
                {
                    string message = "تم التفعيل بنجاح "
                        + (result.CodeCommune.Length == 2 ? ",المجلس الشعبي الولائي لولاية "
                        : ",المجلس الشعبي البلدي لبلدية ") + result.CommuneName;
                    MessageBox.Show("تم التفعيل بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    activated = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("لم يتم التفعيل, " + result.Message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("--- الملف غير مطابق ---" + ex.Message, "حصل خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panelTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left &&
               WindowState != FormWindowState.Maximized)
            {
                ReleaseCapture();

                SendMessage(
                    Handle,
                    WM_NCLBUTTONDOWN,
                    HT_CAPTION,
                    0);
            }
        }
    }
}
