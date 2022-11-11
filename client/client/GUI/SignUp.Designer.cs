namespace client.GUI
{
    partial class SignUp
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
            this.lb_userName = new System.Windows.Forms.Label();
            this.lb_password = new System.Windows.Forms.Label();
            this.lb_confirmPassword = new System.Windows.Forms.Label();
            this.tb_userName = new System.Windows.Forms.TextBox();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.tb_confirmPassword = new System.Windows.Forms.TextBox();
            this.btn_signUp = new System.Windows.Forms.Button();
            this.richTB_alert = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lb_userName
            // 
            this.lb_userName.AutoSize = true;
            this.lb_userName.Location = new System.Drawing.Point(56, 22);
            this.lb_userName.Name = "lb_userName";
            this.lb_userName.Size = new System.Drawing.Size(60, 15);
            this.lb_userName.TabIndex = 0;
            this.lb_userName.Text = "Username";
            // 
            // lb_password
            // 
            this.lb_password.AutoSize = true;
            this.lb_password.Location = new System.Drawing.Point(59, 57);
            this.lb_password.Name = "lb_password";
            this.lb_password.Size = new System.Drawing.Size(57, 15);
            this.lb_password.TabIndex = 1;
            this.lb_password.Text = "Password";
            // 
            // lb_confirmPassword
            // 
            this.lb_confirmPassword.AutoSize = true;
            this.lb_confirmPassword.Location = new System.Drawing.Point(12, 90);
            this.lb_confirmPassword.Name = "lb_confirmPassword";
            this.lb_confirmPassword.Size = new System.Drawing.Size(104, 15);
            this.lb_confirmPassword.TabIndex = 2;
            this.lb_confirmPassword.Text = "Confirm password";
            // 
            // tb_userName
            // 
            this.tb_userName.Location = new System.Drawing.Point(122, 19);
            this.tb_userName.Name = "tb_userName";
            this.tb_userName.Size = new System.Drawing.Size(139, 23);
            this.tb_userName.TabIndex = 3;
            // 
            // tb_password
            // 
            this.tb_password.Location = new System.Drawing.Point(122, 54);
            this.tb_password.Name = "tb_password";
            this.tb_password.PasswordChar = '*';
            this.tb_password.Size = new System.Drawing.Size(139, 23);
            this.tb_password.TabIndex = 4;
            // 
            // tb_confirmPassword
            // 
            this.tb_confirmPassword.Location = new System.Drawing.Point(122, 87);
            this.tb_confirmPassword.Name = "tb_confirmPassword";
            this.tb_confirmPassword.PasswordChar = '*';
            this.tb_confirmPassword.Size = new System.Drawing.Size(139, 23);
            this.tb_confirmPassword.TabIndex = 5;
            // 
            // btn_signUp
            // 
            this.btn_signUp.Location = new System.Drawing.Point(186, 116);
            this.btn_signUp.Name = "btn_signUp";
            this.btn_signUp.Size = new System.Drawing.Size(75, 23);
            this.btn_signUp.TabIndex = 6;
            this.btn_signUp.Text = "Sign Up";
            this.btn_signUp.UseVisualStyleBackColor = true;
            this.btn_signUp.Click += new System.EventHandler(this.btn_signUp_Click);
            // 
            // richTB_alert
            // 
            this.richTB_alert.BackColor = System.Drawing.SystemColors.Menu;
            this.richTB_alert.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTB_alert.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.richTB_alert.ForeColor = System.Drawing.Color.LimeGreen;
            this.richTB_alert.Location = new System.Drawing.Point(12, 153);
            this.richTB_alert.Name = "richTB_alert";
            this.richTB_alert.ReadOnly = true;
            this.richTB_alert.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTB_alert.Size = new System.Drawing.Size(249, 96);
            this.richTB_alert.TabIndex = 7;
            this.richTB_alert.Text = "";
            // 
            // SignUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.richTB_alert);
            this.Controls.Add(this.btn_signUp);
            this.Controls.Add(this.tb_confirmPassword);
            this.Controls.Add(this.tb_password);
            this.Controls.Add(this.tb_userName);
            this.Controls.Add(this.lb_confirmPassword);
            this.Controls.Add(this.lb_password);
            this.Controls.Add(this.lb_userName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SignUp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sign Up";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lb_userName;
        private Label lb_password;
        private Label lb_confirmPassword;
        private TextBox tb_userName;
        private TextBox tb_password;
        private TextBox tb_confirmPassword;
        private Button btn_signUp;
        private RichTextBox richTB_alert;
        private Boolean flag = true;
        private string message = "";
    }
}