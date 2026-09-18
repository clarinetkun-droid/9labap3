using System;
using System.Drawing;
using System.Windows.Forms;

namespace _9labap3
{
    public class LoginForm : Form
    {
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnCancel;
        private Label lblError;

        private const string ADMIN_LOGIN = "admin";
        private const string ADMIN_PASSWORD = "admin123";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "🔐 Авторизация";
            this.Size = new Size(420, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.WhiteSmoke;
            this.Font = new Font("Segoe UI", 10);

            var lblTitle = new Label
            {
                Text = "Вход в систему",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 118, 210),
                Left = 20,
                Top = 20,
                Width = 370,
                Height = 35,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblSubtitle = new Label
            {
                Text = "Учёт СИЗ в цеху",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Left = 20,
                Top = 55,
                Width = 370,
                Height = 20,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblLogin = new Label
            {
                Text = "Логин:",
                Left = 40,
                Top = 95,
                Width = 80,
                Height = 25
            };

            txtLogin = new TextBox
            {
                Left = 130,
                Top = 95,
                Width = 240,
                Height = 28,
                Text = "admin"
            };

            var lblPassword = new Label
            {
                Text = "Пароль:",
                Left = 40,
                Top = 135,
                Width = 80,
                Height = 25
            };

            txtPassword = new TextBox
            {
                Left = 130,
                Top = 135,
                Width = 240,
                Height = 28,
                PasswordChar = '•'
            };

            lblError = new Label
            {
                Text = "",
                ForeColor = Color.Red,
                Left = 40,
                Top = 170,
                Width = 330,
                Height = 20,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnLogin = new Button
            {
                Text = "🔓 Войти",
                Left = 130,
                Top = 200,
                Width = 110,
                Height = 38,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(25, 118, 210),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnCancel = new Button
            {
                Text = "❌ Отмена",
                Left = 260,
                Top = 200,
                Width = 110,
                Height = 38,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(220, 220, 220),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(lblLogin);
            this.Controls.Add(txtLogin);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblError);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnLogin;
            this.CancelButton = btnCancel;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (login == ADMIN_LOGIN && password == ADMIN_PASSWORD)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblError.Text = "❌ Неверный логин или пароль!";
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}