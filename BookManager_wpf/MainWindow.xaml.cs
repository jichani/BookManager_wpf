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
using BookManager_wpf.Models;

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
                            int totalBookCount = books.Sum(book => book.Quantity);
                            int totalBookTypes = books.Count;

                            lblTotalBooks.Content = $"전체 도서 수 : {totalBookTypes}종, 총 {totalBookCount}권";

                            bookStatusGrid.ItemsSource = books;
                            bookStatusAdminGrid.ItemsSource = books;
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
                            int totalMemberCount = members.Count;
                            lblTotalMembers.Content = $"전체 회원 수 : {totalMemberCount}명";

                            memberStatusGrid.ItemsSource = members;
                            memberStatusAdminGrid.ItemsSource = members;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Members found.");
                    }
                }));
            });
        }

        private void bookStatusAdminGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bookStatusAdminGrid.SelectedItem is not null)
            {
                var selectedBook = (Books)bookStatusAdminGrid.SelectedItem;
                txtBookIdAdmin.Text = selectedBook.BookId.ToString();
                txtTitleAdmin.Text = selectedBook.Title;
                txtCategoryAdmin.Text = selectedBook.Category;
                txtAuthorAdmin.Text = selectedBook.Author;
                txtDescriptionAdmin.Text = selectedBook.Description;
                txtPublisherAdmin.Text = selectedBook.Publisher;
                txtPublicationDateAdmin.Text = selectedBook.PublicationDate;
                txtQuantityDateAdmin.Text = selectedBook.Quantity.ToString();
            }
        }

        private void memberStatusAdminGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (memberStatusAdminGrid.SelectedItem is not null)
            {
                var selectedMember = (Members)memberStatusAdminGrid.SelectedItem;
                txtMemberIdAdmin.Text = selectedMember.MemberId.ToString();
                txtMemberNameAdmin.Text = selectedMember.Name;
                txtMemberMobileAdmin.Text = selectedMember.MobileNumber;
            }
        }

        private void bookStatusGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bookStatusGrid.SelectedItem is not null)
            {
                var selectedBook = (Books)bookStatusGrid.SelectedItem;
                txtBookId.Text = selectedBook.BookId.ToString();
                txtBookTitle.Text = selectedBook.Title;
            }
        }

        private void memberStatusGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (memberStatusGrid.SelectedItem is not null)
            {
                var selectedMember = (Members)memberStatusGrid.SelectedItem;
                txtMemberId.Text = selectedMember.MemberId.ToString();
                txtMemberName.Text = selectedMember.Name;
            }
        }
    }
}
