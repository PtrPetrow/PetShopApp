using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PetShop_petro.Pages
{
    public partial class Page1 : Page
    {
        private int loginAttempts = 0;
        private string captchaCode;
        private DispatcherTimer blockTimer = new DispatcherTimer();

        public Page1()
        {
            InitializeComponent();
            blockTimer.Interval = TimeSpan.FromSeconds(10);
            blockTimer.Tick += BlockTimer_Tick;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginAttempts > 0 && !ValidateCaptcha())
            {
                MessageBox.Show("Неверная CAPTCHA!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(LoginTextBox.Text))
            {
                errors.AppendLine("Заполните логин");
            }
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                errors.AppendLine("Заполните пароль");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (PetModel.PetrouEntities.GetContext().User
                .Any(d => d.UserLogin == LoginTextBox.Text
                && d.UserPassword == PasswordBox.Password))
            {
                var user = PetModel.PetrouEntities.GetContext().User
                .FirstOrDefault(d => d.UserLogin == LoginTextBox.Text && d.UserPassword == PasswordBox.Password);

                switch (user.Role.RoleName)
                {
                    case "Администратор":
                        Classes.Manager.MainFrame.Navigate(new Pages.AdminLKPage());
                        break;
                    case "Клиент":
                        Classes.Manager.MainFrame.Navigate(new Pages.ViewProductsPage());
                        break;
                    case "Менеджер":
                        Classes.Manager.MainFrame.Navigate(new Pages.ViewProductsPage());
                        break;
                }

                MessageBox.Show("Успех!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                loginAttempts = 0; /*Сбрасываем попытки*/
                HideCaptcha();
            }
            else
            {
                loginAttempts++;
                MessageBox.Show("Некорректный логин/пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

                if (loginAttempts == 1)
                {
                    ShowCaptcha();
                }
                else if (loginAttempts > 1)
                {
                    /*Блокируем ввод на 10 секунд*/
                    LoginButton.IsEnabled = false;
                    blockTimer.Start();
                }
            }
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new Pages.ViewProductsPage());
        }

        private void BlockTimer_Tick(object sender, EventArgs e)
        {
            LoginButton.IsEnabled = true;
            blockTimer.Stop();
        }

        private void ShowCaptcha()
        {
            captchaCode = GenerateCaptchaCode();
            CaptchaLabel.Visibility = Visibility.Visible;
            CaptchaTextBox.Visibility = Visibility.Visible;
            CaptchaImage.Visibility = Visibility.Visible;
            CaptchaImage.Source = GenerateCaptchaImage(captchaCode);
        }

        private void HideCaptcha()
        {
            CaptchaLabel.Visibility = Visibility.Collapsed;
            CaptchaTextBox.Visibility = Visibility.Collapsed;
            CaptchaImage.Visibility = Visibility.Collapsed;
        }

        private bool ValidateCaptcha()
        {
            return CaptchaTextBox.Text == captchaCode;
        }

        private string GenerateCaptchaCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

        }
        private ImageSource GenerateCaptchaImage(string captchaCode)
        {
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var formattedText = new FormattedText(
                captchaCode, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                new Typeface("Comic Sans MS"), 32, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);

                drawingContext.DrawText(formattedText, new Point(20, 20));

                var pen = new Pen(Brushes.Red, 1);
                drawingContext.DrawLine(pen, new Point(10, 60), new Point(190, 100));
                drawingContext.DrawLine(pen, new Point(10, 10), new Point(190, 90));
                drawingContext.DrawLine(pen, new Point(10, 80), new Point(190, 20));
                drawingContext.DrawLine(pen, new Point(20, 90), new Point(190, 50));

            }

            var renderBitmap = new RenderTargetBitmap(200, 100, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);
            return renderBitmap;
        }
    }
}
