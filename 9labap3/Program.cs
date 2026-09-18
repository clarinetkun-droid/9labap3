using System;
using System.Windows.Forms;

namespace _9labap3
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Показываем форму авторизации
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }

            // Запускаем главную форму
            Application.Run(new Form1());
        }
    }
}