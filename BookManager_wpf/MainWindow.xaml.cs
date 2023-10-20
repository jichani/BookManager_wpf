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

namespace BookManager_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataManager dataManager;

        public MainWindow()
        {
            InitializeComponent();

            // DataManager 인스턴스 생성
            dataManager = new DataManager();

            Task.Run(() =>
            {
                var books = dataManager.LoadBooks();  // 데이터베이스에서 책 정보 불러오기
                var members = dataManager.LoadMembers();  // 데이터베이스에서 회원 정보 불러오기

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (books.Count > 0)  // 책 정보가 있으면...
                    {
                        foreach (var book in books)
                        {
                            bookStatusGrid.ItemsSource = books;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No books found.");
                    }

                    if (members.Count > 0)     //회원 정보가 있으면...
                    {
                        foreach (var member in members)
                        {
                            memberStatusGrid.ItemsSource = members;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Members found.");
                    }
                }));
            });
        }
    }

}
