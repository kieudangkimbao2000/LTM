namespace client.GUI
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.user = new System.Windows.Forms.Label();
            this.pwd = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.tbUser = new System.Windows.Forms.TextBox();
            this.tbPwd = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // user
            // 
            this.user.AutoSize = true;
            this.user.Location = new System.Drawing.Point(45, 128);
            this.user.Name = "user";
            this.user.Size = new System.Drawing.Size(73, 20);
            this.user.TabIndex = 1;
            this.user.Text = "username";
            // 
            // pwd
            // 
            this.pwd.AutoSize = true;
            this.pwd.Location = new System.Drawing.Point(45, 167);
            this.pwd.Name = "pwd";
            this.pwd.Size = new System.Drawing.Size(72, 20);
            this.pwd.TabIndex = 2;
            this.pwd.Text = "password";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(175, 210);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(94, 29);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // tbUser
            // 
            this.tbUser.Location = new System.Drawing.Point(152, 125);
            this.tbUser.Name = "tbUser";
            this.tbUser.Size = new System.Drawing.Size(179, 27);
            this.tbUser.TabIndex = 4;
            // 
            // tbPwd
            // 
            this.tbPwd.Location = new System.Drawing.Point(152, 167);
            this.tbPwd.Name = "tbPwd";
            this.tbPwd.Size = new System.Drawing.Size(179, 27);
            this.tbPwd.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(175, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(94, 83);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 287);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tbPwd);
            this.Controls.Add(this.tbUser);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.pwd);
            this.Controls.Add(this.user);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label user;
        private Label pwd;
        private Button btnLogin;
        private TextBox tbUser;
        private TextBox tbPwd;
        private PictureBox pictureBox1;
    }
}