using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NevaTelecom
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Создаем пустую переменную
        string code;
        //Выключаем ненужные поля
        public MainWindow()
        {
            InitializeComponent();
            PasswordBox.IsEnabled = false;
            CodeBox.IsEnabled = false;
            EnterBtn.IsEnabled = false;
            RefreshBtn.IsEnabled = false;
            NumberBox.Focus();
        }

        //Таймер и генератор случайного кода
        DispatcherTimer timer = new DispatcherTimer();
        private void gencode()
        {
            code = null;
            Random random = new Random();
            string[] massiveCharacters = new string[] { "A", "b", "C", "d", "E", "f", "G", "h", "I", "j", "K", "l", "M", "n", "P", "q", "R", "!", "@", "#", "$", "%", "1", "2", "3", "4" ,"5", "6","7","8","9"};
            for (int i = 0; i < 8; i++)
                code += massiveCharacters[random.Next(0, massiveCharacters.Length)];
            if (MessageBox.Show(code.ToString(), "Code", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                timer.Interval = TimeSpan.FromSeconds(10);
                timer.Tick += TimerTick;
                timer.Start();
                CodeBox.IsEnabled = true;
                EnterBtn.IsEnabled = true;
                RefreshBtn.IsEnabled = true;
            }
        }

        //Метод который выводит сообщение и обнуляет переменную кода аунтификации
        void TimerTick(object sender, EventArgs e)
        {
            code = null;
            MessageBox.Show("Код сброшен, повтоите попытку.");
            timer.Stop();
        }

        // Обработчик и проверка логина на подлинность из Базы Данных
        private void NumberBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                using (var db = new TelecomNevaEntities())
                {
                    var number = db.Users.AsNoTracking().FirstOrDefault(m => m.Number == NumberBox.Text.Trim());
                    if (number == null)
                    {
                        MessageBox.Show("Неверный номер пользователя, такого не существует");
                    }
                    else
                    {
                        NumberBox.IsEnabled = false;
                        PasswordBox.IsEnabled = true;
                        PasswordBox.Focus();
                    }
                }
            }
        }

        // Обработчик и повторная проверка логина на подлинность из Базы Данных, а так же пароля
        private void PasswordBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                using (var db = new TelecomNevaEntities())
                {
                    var password = db.Users.AsNoTracking().FirstOrDefault(m => m.Number == NumberBox.Text.Trim() & (m.Password == PasswordBox.Password));
                    {
                        if (password == null)
                        {
                            MessageBox.Show("Пароль неверный.");
                        }
                        else
                        {
                            PasswordBox.IsEnabled = false;
                            gencode();
                            CodeBox.Focus();
                        }
                    }
                }
            }
        }

        //Стирание всей информации из полей
        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            NumberBox.Text = String.Empty;
            PasswordBox.Password = String.Empty;
            CodeBox.Text = String.Empty;
        }

        // Кнопка Входа, разграничение пользователей и проверка кода аунтификации на правильность ввода
        private void EnterBtnClick(object sender, RoutedEventArgs e)
        {
            if (code == CodeBox.Text)
            {
                timer.Stop();
                if (NumberBox.Text == "1")
                { 
                    MessageBox.Show("Ваша роль: Специалист ТП (выездной инженер)");
                }

                else if (NumberBox.Text == "2")
                {
                    MessageBox.Show("Ваша роль: Директор по развитию");
                }
                else
                {
                    MessageBox.Show("Еще не внесли такого пользователя в программу");
                }
                
            }
            else
            {
                timer.Stop();
                MessageBox.Show("Неверно введен код. Повторите попытку");
            }
        }

        //Обновление кода аунтификации
        private void RefreshBtnClick(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            gencode();
            CodeBox.Focus();
        }
    }
}
