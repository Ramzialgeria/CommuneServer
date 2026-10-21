namespace CommuneServer
{
    partial class ActivationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivationForm));
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelSouscription = new System.Windows.Forms.Label();
            this.panelClose = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.textBoxKey = new System.Windows.Forms.TextBox();
            this.labelCode = new System.Windows.Forms.Label();
            this.textBoxCode = new System.Windows.Forms.TextBox();
            this.buttonActivation = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.Black;
            this.panelTop.Controls.Add(this.labelSouscription);
            this.panelTop.Controls.Add(this.panelClose);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Font = new System.Drawing.Font("Cairo", 8.249999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(16, 14, 16, 14);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1610, 126);
            this.panelTop.TabIndex = 41;
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            // 
            // labelSouscription
            // 
            this.labelSouscription.BackColor = System.Drawing.Color.Transparent;
            this.labelSouscription.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelSouscription.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelSouscription.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSouscription.ForeColor = System.Drawing.Color.White;
            this.labelSouscription.Location = new System.Drawing.Point(1178, 0);
            this.labelSouscription.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.labelSouscription.Name = "labelSouscription";
            this.labelSouscription.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelSouscription.Size = new System.Drawing.Size(432, 126);
            this.labelSouscription.TabIndex = 163;
            this.labelSouscription.Text = "تفعيل التطبيقة";
            this.labelSouscription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelSouscription.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            // 
            // panelClose
            // 
            this.panelClose.BackColor = System.Drawing.Color.Transparent;
            this.panelClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelClose.BackgroundImage")));
            this.panelClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelClose.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelClose.Location = new System.Drawing.Point(0, 0);
            this.panelClose.Margin = new System.Windows.Forms.Padding(21, 38, 21, 38);
            this.panelClose.Name = "panelClose";
            this.panelClose.Size = new System.Drawing.Size(101, 126);
            this.panelClose.TabIndex = 34;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(138)))), ((int)(((byte)(52)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(184)))), ((int)(((byte)(43)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(45, 250);
            this.button1.Margin = new System.Windows.Forms.Padding(21, 21, 21, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(405, 81);
            this.button1.TabIndex = 47;
            this.button1.Text = "اختيار ملف التفعيل";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxKey
            // 
            this.textBoxKey.Enabled = false;
            this.textBoxKey.Location = new System.Drawing.Point(480, 250);
            this.textBoxKey.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.Size = new System.Drawing.Size(1020, 38);
            this.textBoxKey.TabIndex = 46;
            // 
            // labelCode
            // 
            this.labelCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCode.Location = new System.Drawing.Point(499, 405);
            this.labelCode.Name = "labelCode";
            this.labelCode.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelCode.Size = new System.Drawing.Size(1019, 55);
            this.labelCode.TabIndex = 48;
            this.labelCode.Text = "رمز التفعيل";
            // 
            // textBoxCode
            // 
            this.textBoxCode.Location = new System.Drawing.Point(480, 467);
            this.textBoxCode.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.textBoxCode.MaxLength = 1000;
            this.textBoxCode.Multiline = true;
            this.textBoxCode.Name = "textBoxCode";
            this.textBoxCode.Size = new System.Drawing.Size(1020, 187);
            this.textBoxCode.TabIndex = 49;
            // 
            // buttonActivation
            // 
            this.buttonActivation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonActivation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(138)))), ((int)(((byte)(52)))));
            this.buttonActivation.FlatAppearance.BorderSize = 0;
            this.buttonActivation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(184)))), ((int)(((byte)(43)))));
            this.buttonActivation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonActivation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonActivation.ForeColor = System.Drawing.Color.White;
            this.buttonActivation.Location = new System.Drawing.Point(45, 568);
            this.buttonActivation.Margin = new System.Windows.Forms.Padding(21, 21, 21, 21);
            this.buttonActivation.Name = "buttonActivation";
            this.buttonActivation.Size = new System.Drawing.Size(405, 86);
            this.buttonActivation.TabIndex = 45;
            this.buttonActivation.Text = "تفعيل";
            this.buttonActivation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonActivation.UseVisualStyleBackColor = false;
            this.buttonActivation.Click += new System.EventHandler(this.buttonActivation_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(0, 694);
            this.panel3.TabIndex = 44;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 694);
            this.panel2.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1610, 2);
            this.panel2.TabIndex = 43;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1610, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(3, 696);
            this.panel1.TabIndex = 42;
            // 
            // ActivationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1613, 696);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxKey);
            this.Controls.Add(this.labelCode);
            this.Controls.Add(this.textBoxCode);
            this.Controls.Add(this.buttonActivation);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ActivationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ActivationForm";
            this.panelTop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label labelSouscription;
        private System.Windows.Forms.Panel panelClose;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBoxKey;
        private System.Windows.Forms.Label labelCode;
        private System.Windows.Forms.TextBox textBoxCode;
        private System.Windows.Forms.Button buttonActivation;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
    }
}