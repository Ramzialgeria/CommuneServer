using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CommuneSecurity.Encryption;
using CommuneServer.Activation;
using CommuneServer.Api;
using CommuneServer.Data;
using CommuneServer.Models;
using CommuneServer.Services;
using Newtonsoft.Json.Linq;
using CommuneSecurity.Encryption;

namespace CommuneServer
{
    public partial class MainForm : Form
    {
        private readonly Label _status = new Label();
        private readonly Label _local = new Label();
        private readonly Label _central = new Label();

        private readonly NetworkDiagnosticService _network;
        private readonly Timer _timer;
        private CommuneDbContext db;
        public MainForm()
        {

            //string hash = BCrypt.Net.BCrypt.HashPassword("Test@12345");
            //Console.WriteLine(hash);
            //string original = "Ahmed BENALI";

            //string encrypted =
            //    EncryptionService.Encrypt(original);

            //string decrypted =
            //    EncryptionService.Decrypt(encrypted);

            //MessageBox.Show(
            //    "Original: " + original +
            //    Environment.NewLine +
            //    Environment.NewLine +
            //    "Encrypted: " + encrypted +
            //    Environment.NewLine +
            //    Environment.NewLine +
            //    "Decrypted: " + decrypted);

            db = new CommuneDbContext();
            ActivationService activationService = new ActivationService();

            if(!activationService.Activated(db))
            {
                ActivationForm formActivation = new ActivationForm();
                formActivation.ShowDialog();
                if (formActivation.activated == false)
                {
                    this.Close();
                    return;
                }
            }

            _network = new NetworkDiagnosticService();

            Text = "Commune Server";
            Width = 900;
            Height = 600;
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;

            //var statusPanel = new TableLayoutPanel();

            //statusPanel.Dock = DockStyle.Fill;
            //statusPanel.Padding = new Padding(20);
            //statusPanel.ColumnCount = 2;
            //statusPanel.RowCount = 3;

            //statusPanel.ColumnStyles.Add(
            //    new ColumnStyle(
            //        SizeType.Percent,
            //        35));

            //statusPanel.ColumnStyles.Add(
            //    new ColumnStyle(
            //        SizeType.Percent,
            //        65));

            //AddRow(statusPanel, 0, "Status", _status);
            //AddRow(statusPanel, 1, "Local network", _local);
            //AddRow(statusPanel, 2, "Central server", _central);

            _timer = new Timer();

            var value =
                ConfigurationManager.AppSettings[
                    "HeartbeatSeconds"];

            int seconds;

            if (!int.TryParse(value, out seconds))
                seconds = 10;

            if (seconds < 5)
                seconds = 5;

            _timer.Interval = seconds * 1000;

            _timer.Tick += Timer_Tick;

            Shown += MainForm_Shown;
        }

        string clientSecret;
        string clientCode;
        string centralUrl;

        ApiClient api;

        private async void MainForm_Shown(
            object sender,
            EventArgs e)
        {
            try
            {
                
                centralUrl =
                    ConfigurationManager.AppSettings[
                        "CentralApiUrl"];

                clientCode =
                    ConfigurationManager.AppSettings[
                        "ServerCode"];

                clientSecret =
                    ConfigurationManager.AppSettings[
                        "ClientSecret"];

                api = new ApiClient(centralUrl);

                api.SetCredentials(clientCode, clientSecret);


                var authenticated =
                    await api.AuthenticateAsync(
                        clientCode,
                        clientSecret);

                if (!authenticated)
                {
                    _central.Text = "Disconnected";
                    _status.Text = "Offline";
                }
                else
                {
                    await CheckStatusAsync();
                }

                //await CheckStatusAsync();

                _timer.Start();
            }
            catch (Exception ex)
            {
                _status.Text = "Error";
                _central.Text = ex.Message;
            }
        }

        private async void Timer_Tick(
            object sender,
            EventArgs e)
        {
            await CheckStatusAsync();

        }
        private async System.Threading.Tasks.Task CheckStatusAsync()
        {
            try
            {
                var heartbeat =
                    new HeartbeatRequest
                    {
                        ClientCode = clientCode
                    };

                var heartbeatOk =
                    await api.HeartbeatAsync(
                        heartbeat);

                _central.Text =
                    heartbeatOk
                        ? "Connected"
                        : "Disconnected";

                _status.Text =
                    heartbeatOk
                        ? "Online"
                        : "Offline";
            }
            catch (Exception ex)
            {
                _status.Text = "Error";
                _central.Text = ex.Message;
            }
        }

        protected override void OnFormClosing(
            FormClosingEventArgs e)
        {
            _timer.Stop();

            base.OnFormClosing(e);
        }
    }
}