using CommuneServer.Activation;
using CommuneServer.Api;
using CommuneServer.Data;
using System;
using System.Windows.Forms;

namespace CommuneServer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // The application must be activated before anything starts.
            // The user chooses Activation_xxx.enc and types the code.
            if (!EnsureActivated())
                return;

            var api = new LocalApiServer();

            try
            {
                api.Start();

                Application.Run(new MainForm());
            }
            finally
            {
                api.Stop();
            }
        }

        private static bool EnsureActivated()
        {
            try
            {
                using (var db = new CommuneDbContext())
                {
                    if (new ActivationService().Activated(db))
                        return true;
                }

                using (var form = new ActivationForm())
                {
                    form.ShowDialog();
                    return form.activated;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Commune Server",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }
    }
}