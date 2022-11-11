namespace client.GUI
{
    partial class ChatForm
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
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelFill = new System.Windows.Forms.Panel();
            this.rtbContent = new System.Windows.Forms.RichTextBox();
            this.panelBottom = new System.Windows.Forms.FlowLayoutPanel();
            this.btnIcon = new System.Windows.Forms.Button();
            this.rtbSend = new System.Windows.Forms.RichTextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.FlowLayoutPanel();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.btnLogIn = new System.Windows.Forms.Button();
            this.tbReceiver = new System.Windows.Forms.TextBox();
            this.dgvIcon = new System.Windows.Forms.DataGridView();
            this.btn_signUp = new System.Windows.Forms.Button();
            this.panelFill.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(174, 544);
            this.panelLeft.TabIndex = 0;
            // 
            // panelFill
            // 
            this.panelFill.Controls.Add(this.rtbContent);
            this.panelFill.Controls.Add(this.panelBottom);
            this.panelFill.Controls.Add(this.panelTop);
            this.panelFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFill.Location = new System.Drawing.Point(174, 0);
            this.panelFill.Name = "panelFill";
            this.panelFill.Size = new System.Drawing.Size(632, 544);
            this.panelFill.TabIndex = 1;
            // 
            // rtbContent
            // 
            this.rtbContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbContent.Location = new System.Drawing.Point(0, 53);
            this.rtbContent.Name = "rtbContent";
            this.rtbContent.Size = new System.Drawing.Size(632, 437);
            this.rtbContent.TabIndex = 2;
            this.rtbContent.Text = "";
            this.rtbContent.Enter += new System.EventHandler(this.rtbContent_Enter);
            this.rtbContent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rtbContent_KeyPress);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.btnIcon);
            this.panelBottom.Controls.Add(this.rtbSend);
            this.panelBottom.Controls.Add(this.btnSend);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 490);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(632, 54);
            this.panelBottom.TabIndex = 1;
            // 
            // btnIcon
            // 
            this.btnIcon.Location = new System.Drawing.Point(3, 3);
            this.btnIcon.Name = "btnIcon";
            this.btnIcon.Size = new System.Drawing.Size(75, 23);
            this.btnIcon.TabIndex = 0;
            this.btnIcon.Text = "Icon";
            this.btnIcon.UseVisualStyleBackColor = true;
            this.btnIcon.Click += new System.EventHandler(this.btnIcon_Click);
            this.btnIcon.Leave += new System.EventHandler(this.btnIcon_Leave);
            // 
            // rtbSend
            // 
            this.rtbSend.Location = new System.Drawing.Point(84, 3);
            this.rtbSend.Name = "rtbSend";
            this.rtbSend.Size = new System.Drawing.Size(462, 23);
            this.rtbSend.TabIndex = 2;
            this.rtbSend.Text = "";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(552, 3);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.tbUsername);
            this.panelTop.Controls.Add(this.tbPassword);
            this.panelTop.Controls.Add(this.btnLogIn);
            this.panelTop.Controls.Add(this.tbReceiver);
            this.panelTop.Controls.Add(this.btn_signUp);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(632, 53);
            this.panelTop.TabIndex = 0;
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(3, 3);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(100, 23);
            this.tbUsername.TabIndex = 1;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(109, 3);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(100, 23);
            this.tbPassword.TabIndex = 2;
            // 
            // btnLogIn
            // 
            this.btnLogIn.Location = new System.Drawing.Point(215, 3);
            this.btnLogIn.Name = "btnLogIn";
            this.btnLogIn.Size = new System.Drawing.Size(75, 23);
            this.btnLogIn.TabIndex = 0;
            this.btnLogIn.Text = "Log in";
            this.btnLogIn.UseVisualStyleBackColor = true;
            this.btnLogIn.Click += new System.EventHandler(this.btnLogIn_Click);
            // 
            // tbReceiver
            // 
            this.tbReceiver.Location = new System.Drawing.Point(296, 3);
            this.tbReceiver.Name = "tbReceiver";
            this.tbReceiver.Size = new System.Drawing.Size(100, 23);
            this.tbReceiver.TabIndex = 3;
            // 
            // dgvIcon
            // 
            this.dgvIcon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIcon.Location = new System.Drawing.Point(150, 366);
            this.dgvIcon.Name = "dgvIcon";
            this.dgvIcon.RowTemplate.Height = 25;
            this.dgvIcon.Size = new System.Drawing.Size(150, 150);
            this.dgvIcon.TabIndex = 3;
            this.dgvIcon.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIcon_CellContentClick);
            // 
            // btn_signUp
            // 
            this.btn_signUp.Location = new System.Drawing.Point(402, 3);
            this.btn_signUp.Name = "btn_signUp";
            this.btn_signUp.Size = new System.Drawing.Size(75, 23);
            this.btn_signUp.TabIndex = 4;
            this.btn_signUp.Text = "Sign Up";
            this.btn_signUp.UseVisualStyleBackColor = true;
            this.btn_signUp.Click += new System.EventHandler(this.btn_signUp_Click);
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 544);
            this.Controls.Add(this.panelFill);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.dgvIcon);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ChatForm";
            this.Text = "ChatForm";
            this.panelFill.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panelLeft;
        private Panel panelFill;
        private FlowLayoutPanel panelBottom;
        private FlowLayoutPanel panelTop;
        private RichTextBox rtbContent;
        private Button btnIcon;
        private Button btnSend;
        private Button btnLogIn;
        private RichTextBox rtbSend;
        private TextBox tbUsername;
        private TextBox tbPassword;
        private TextBox tbReceiver;
        private DataGridView dgvIcon;
        private Button btn_signUp;
    }
}