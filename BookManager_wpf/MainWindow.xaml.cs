using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

            // 대여 로그를 텍스트 파일에서 불러오기
            if (File.Exists("checkoutLogs.txt"))
            {
                string[] logLines = File.ReadAllLines("checkoutLogs.txt");

                // 라인들을 거꾸로 순회하여 최신 로그가 위로 오게 함
                for (int i = logLines.Length - 1; i >= 0; i--)
                {
                    checkoutLogs.Items.Add(logLines[i]);
                }
            }

            Task.Run(() =>
            {
                var books = dataManager.LoadBooks();  // 데이터베이스에서 책 정보 불러오기
                var members = dataManager.LoadMembers();  // 데이터베이스에서 회원 정보 불러오기
                var checkouts = dataManager.LoadCheckouts();  // 데이터베이스에서 체크아웃 정보 불러오기

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (books.Count > 0)  // 책 정보가 있으면...
                    {
                        foreach (var book in books)
                        {
                            int totalBookCount = books.Sum(book =>
                            {
                                int quantity;
                                if (int.TryParse(book.Quantity, out quantity))
                                    return quantity;
                                else
                                    return 0;
                            });
                            int totalBookTypes = books.Count;

                            int totalBorrowedBookCount = checkouts.Count(checkout => checkout.ReturnDate == null);

                            int totalAvailableBookCount = totalBookCount - totalBorrowedBookCount;

                            lblTotalBooks.Content = $"전체 도서 수 : {totalBookTypes}종, 총 {totalBookCount}권";
                            lblAvailableBooks.Content = $"대출 가능한 도서 수 : {totalAvailableBookCount}권";
                            lblBorrowedBooks.Content = $"대출 중인 도서 수 : {totalBorrowedBookCount}권";

                            bookStatusGrid.ItemsSource = books;
                            bookStatusAdminGrid.ItemsSource = books;

                            // 연체 중인 도서 개수 계산 및 업데이트
                            int overdueBooksCount = dataManager.GetOverdueBookCount();
                            lblOverdueBooks.Content = $"연체 중인 도서 수 : {overdueBooksCount}권";
                        }
                    }
                    else
                    {
                        MessageBox.Show("책 정보를 못찾았습니다.");
                    }

                    if (members.Count > 0)     //회원 정보가 있으면...
                    {
                        foreach (var member in members)
                        {
                            int totalMemberCount = members.Count;
                            lblTotalMembers.Content = $"전체 회원 수 : {totalMemberCount}명";

                            memberStatusGrid.ItemsSource = members;
                            memberStatusAdminGrid.ItemsSource = members;
                            
                            checkoutAllGrid.ItemsSource = checkouts;

                            var checkoutsNotReturned = checkouts.Where(c => c.IsReturned == false).ToList();
                            checkoutStatusGrid.ItemsSource = checkoutsNotReturned;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Members found.");
                    }
                }));
            });
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BookStatusAdminGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        private void MemberStatusAdminGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (memberStatusAdminGrid.SelectedItem is not null)
            {
                var selectedMember = (Members)memberStatusAdminGrid.SelectedItem;
                txtMemberIdAdmin.Text = selectedMember.MemberId.ToString();
                txtMemberNameAdmin.Text = selectedMember.Name;
                txtMemberMobileAdmin.Text = selectedMember.MobileNumber;
            }
        }
        private void BookStatusGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bookStatusGrid.SelectedItem is not null)
            {
                var selectedBook = (Books)bookStatusGrid.SelectedItem;
                txtBookId.Text = selectedBook.BookId.ToString();
                txtBookTitle.Text = selectedBook.Title;
            }
        }
        private void MemberStatusGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (memberStatusGrid.SelectedItem is not null)
            {
                var selectedMember = (Members)memberStatusGrid.SelectedItem;
                txtMemberId.Text = selectedMember.MemberId.ToString();
                txtMemberName.Text = selectedMember.Name;
            }
        }

        private void MemberAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (txtMemberIdAdmin.Text != "")
            {
                MessageBox.Show("사용자 ID값을 비워주세요");
                return;
            }

            // 새로운 Member 객체 생성
            var newMember = new Members()
            {
                Name = txtMemberNameAdmin.Text,
                MobileNumber = txtMemberMobileAdmin.Text,
            };

            // DataManager의 AddNewMemeber 메소드 호출하여 DB에 저장
            dataManager.AddNewMember(newMember);

            // DB에서 최신 멤버 리스트 불러오기
            var members = dataManager.LoadMembers();

            // UI 갱신 
            memberStatusGrid.ItemsSource = members;
            memberStatusAdminGrid.ItemsSource = members;

            int totalMemberCount = members.Count;
            lblTotalMembers.Content = $"전체 회원 수 : {totalMemberCount}명";
        }
        private void MemberEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (txtMemberIdAdmin.Text == "")
            {
                MessageBox.Show("사용자 ID값을 입력해주세요");
                return;
            }

            int memberId;

            if (!int.TryParse(txtMemberIdAdmin.Text, out memberId))
            {
                MessageBox.Show("사용자 ID값은 숫자여야 합니다");
                return;
            }

            // DataManager의 GetMemeberById 메소드 호출하여 DB에서 멤버 정보 가져오기
            var memberToEdit = dataManager.GetMemberById(memberId);

            if (memberToEdit == null)
            {
                MessageBox.Show("해당 ID의 회원이 존재하지 않습니다");
                return;
            }

            // 수정을 확인합니다.
            MessageBoxResult result = MessageBox.Show("정말로 수정하실건가요?", "수정 확인", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                // 사용자가 도서 정보 수정을 원하지 않는 경우, 메서드 실행을 여기서 종료합니다.
                return;
            }

            // 수정할 Member 객체 생성
            var updatedMemeber = new Members()
            {
                MemberId = memberId,
                Name = txtMemberNameAdmin.Text,
                MobileNumber = txtMemberMobileAdmin.Text,
            };

            // DataManager의 UpdateMemeber 메소드 호출하여 DB에 저장
            dataManager.UpdateMember(updatedMemeber);

            // DB에서 최신 멤버 리스트 불러오기
            var members = dataManager.LoadMembers();

            // UI 갱신 
            memberStatusGrid.ItemsSource = members;
            memberStatusAdminGrid.ItemsSource = members;

            int totalMemeberCount = members.Count;
            lblTotalMembers.Content = $"전체 회원 수 : {totalMemeberCount}명";
        }
        private void MemberDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (txtMemberIdAdmin.Text == "")
            {
                MessageBox.Show("사용자 ID값을 입력해주세요");
                return;
            }

            int memberId;

            if (!int.TryParse(txtMemberIdAdmin.Text, out memberId))
            {
                MessageBox.Show("사용자 ID값은 숫자여야 합니다");
                return;
            }

            // DataManager의 GetMemeberById 메소드 호출하여 DB에서 멤버 정보 가져오기
            var memberToDelete = dataManager.GetMemberById(memberId);

            if (memberToDelete == null)
            {
                MessageBox.Show("해당 ID의 회원이 존재하지 않습니다");
                return;
            }

            // 이름과 연락처가 일치하는지 확인
            if (memberToDelete.Name != txtMemberNameAdmin.Text || memberToDelete.MobileNumber != txtMemberMobileAdmin.Text)
            {
                MessageBox.Show("입력한 이름 또는 연락처가 일치하지 않습니다");
                return;
            }

            // 삭제를 확인합니다.
            MessageBoxResult result = MessageBox.Show("정말로 삭제하실건가요?", "삭제 확인", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                // 사용자가 도서 삭제를 원하지 않는 경우, 메서드 실행을 여기서 종료합니다.
                return;
            }

            // DataManager의 DeleteMemeber 메소드 호출하여 DB에서 해당 멤버 삭제
            dataManager.DeleteMember(memberId);

            // DB에서 최신 멤버 리스트 불러오기
            var members = dataManager.LoadMembers();

            // UI 갱신 
            memberStatusGrid.ItemsSource = members;
            memberStatusAdminGrid.ItemsSource = members;

            int totalMemeberCount = members.Count;
            lblTotalMembers.Content = $"전체 회원 수 : {totalMemeberCount}명";
        }


        private void BookAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (txtBookIdAdmin.Text != "")
            {
                MessageBox.Show("도서번호를 비워주세요");
                return;
            }

            if (
                string.IsNullOrWhiteSpace(txtTitleAdmin.Text) ||
                string.IsNullOrWhiteSpace(txtCategoryAdmin.Text) ||
                string.IsNullOrWhiteSpace(txtAuthorAdmin.Text) ||
                string.IsNullOrWhiteSpace(txtDescriptionAdmin.Text) ||
                string.IsNullOrWhiteSpace(txtPublisherAdmin.Text) ||
                string.IsNullOrWhiteSpace(txtPublicationDateAdmin.Text) ||
                string.IsNullOrWhiteSpace(txtQuantityDateAdmin.Text)
                )
            {
                MessageBox.Show("도서번호를 제외한 모든 필드를 채워주세요");
                return;
            }

            var newBook = new Books
            {
                Title = txtTitleAdmin.Text,
                Category = txtCategoryAdmin.Text,
                Author = txtAuthorAdmin.Text,
                Description = txtDescriptionAdmin.Text,
                Publisher = txtPublisherAdmin.Text,
                PublicationDate = txtPublicationDateAdmin.Text,
                Quantity = txtQuantityDateAdmin.Text,
                RegisteredDate = DateTime.Now
            };

            dataManager.AddNewBook(newBook);

            // DB에서 최신 멤버 리스트 불러오기
            var books = dataManager.LoadBooks();

            // UI 갱신 
            bookStatusGrid.ItemsSource = books;
            bookStatusAdminGrid.ItemsSource = books;

            int totalBookCount = books.Sum(book =>
            {
                int quantity;
                if (int.TryParse(book.Quantity, out quantity))
                    return quantity;
                else
                    return 0;
            });
            int totalBookTypes = books.Count;

            lblTotalBooks.Content = $"전체 도서 수 : {totalBookTypes}종, 총 {totalBookCount}권";
        }
        private void BookUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            int bookId;

            if (!int.TryParse(txtBookIdAdmin.Text, out bookId))
            {
                MessageBox.Show("유효한 도서 번호를 입력하세요.");
                return;
            }

            var existingBook = dataManager.GetBookById(bookId);

            if (existingBook == null)
            {
                MessageBox.Show($"도서번호 {bookId}에 해당하는 도서가 없습니다.");
                return;
            }

            // 수정을 확인합니다.
            MessageBoxResult result = MessageBox.Show("정말로 수정하실건가요?", "수정 확인", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                // 사용자가 도서 정보 수정을 원하지 않는 경우, 메서드 실행을 여기서 종료합니다.
                return;
            }

            existingBook.Title = txtTitleAdmin.Text;
            existingBook.Category = txtCategoryAdmin.Text;
            existingBook.Author = txtAuthorAdmin.Text;
            existingBook.Description = txtDescriptionAdmin.Text;
            existingBook.Publisher = txtPublisherAdmin.Text;

            // PublicationDate는 문자열로 처리되므로 그대로 전달합니다.
            existingBook.PublicationDate = txtPublicationDateAdmin.Text;

            existingBook.Quantity = txtQuantityDateAdmin.Text;

            dataManager.UpdateBook(existingBook);

            // DB에서 최신 멤버 리스트 불러오기
            var books = dataManager.LoadBooks();

            // UI 갱신 
            bookStatusGrid.ItemsSource = books;
            bookStatusAdminGrid.ItemsSource = books;

            int totalBookCount = books.Sum(book =>
            {
                int quantity;
                if (int.TryParse(book.Quantity, out quantity))
                    return quantity;
                else
                    return 0;
            });
            int totalTypesCount = books.Count;

            lblTotalBooks.Content = $"전체 도서 수 : {totalTypesCount}종, 총 {totalBookCount}권";
        }
        private void BookDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int bookId;

            if (!int.TryParse(txtBookIdAdmin.Text, out bookId))
            {
                MessageBox.Show("유효한 도서 번호를 입력하세요.");
                return;
            }

            var existingBook = dataManager.GetBookById(bookId);

            if (existingBook == null)
            {
                MessageBox.Show($"도서번호 {bookId}에 해당하는 도서가 없습니다.");
                return;
            }

            // 삭제를 확인합니다.
            MessageBoxResult result = MessageBox.Show("정말로 삭제하실건가요?", "삭제 확인", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                // 사용자가 도서 삭제를 원하지 않는 경우, 메서드 실행을 여기서 종료합니다.
                return;
            }

            dataManager.DeleteBook(bookId);

            // DB에서 최신 멤버 리스트 불러오기
            var books = dataManager.LoadBooks();

            // UI 갱신 
            bookStatusGrid.ItemsSource = books;
            bookStatusAdminGrid.ItemsSource = books;

            int totalQuantityCount = books.Sum(book =>
            {
                int quantity;
                if (int.TryParse(book.Quantity, out quantity))
                    return quantity;
                else
                    return 0;
            });

            int totalTypesCount = books.Count;

            lblTotalBooks.Content = $"전체 도서 수 : {totalTypesCount}종, 총 {totalQuantityCount}권";
        }


        private void BookSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string? selectedField = (BookSearchComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string searchText = BookSearchTextBox.Text.Trim();

            if (string.IsNullOrEmpty(selectedField) || string.IsNullOrEmpty(searchText))
            {
                // 필드 또는 검색어가 비어있으면 모든 도서를 표시합니다.
                bookStatusAdminGrid.ItemsSource = dataManager.LoadBooks();
                return;
            }

            List<Books> filteredBooks;

            switch (selectedField)
            {
                case "제목":
                    filteredBooks = dataManager.SearchBooksByTitle(searchText);
                    break;
                case "분류":
                    filteredBooks = dataManager.SearchBooksByCategory(searchText);
                    break;
                case "출판사":
                    filteredBooks = dataManager.SearchBooksByPublisher(searchText);
                    break;
                default:
                    // 선택된 필드가 없으면 모든 도서를 표시합니다.
                    bookStatusAdminGrid.ItemsSource = dataManager.LoadBooks();
                    return;
            }

            bookStatusAdminGrid.ItemsSource = filteredBooks;
        }
        private void BookSearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BookSearchButton_Click(sender, e);
            }
        }
        private void CheckoutSearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CheckoutSearchButton_Click(sender, e);
            }
        }


        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            int bookId;
            int memberId;
            string bookTitle = txtBookTitle.Text.Trim();
            string memberName = txtMemberName.Text.Trim();

            if (!int.TryParse(txtBookId.Text, out bookId) || !int.TryParse(txtMemberId.Text, out memberId))
            {
                MessageBox.Show("유효한 도서 번호와 사용자 ID를 입력하세요.");
                return;
            }

            var selectedMember = dataManager.GetMemberById(memberId);

            // 멤버 이름 확인
            if (selectedMember == null || selectedMember.Name != memberName)
            {
                MessageBox.Show("해당 ID의 회원이 존재하지 않거나 이름이 일치하지 않습니다.");
                return;
            }

            var selectedBook = dataManager.GetBookById(bookId);

            // 도서 제목 확인
            if (selectedBook == null || selectedBook.Title != bookTitle)
            {
                MessageBox.Show("해당 ID의 도서가 존재하지 않거나 제목이 일치하지 않습니다.");
                return;
            }

            // 책의 가용 수량 확인
            if (selectedBook.QuantityAvailable <= 0)
            {
                MessageBox.Show("더 이상 대여할 수 없는 도서입니다.");
                return;
            }

            if (selectedMember.AvailableBookCount <= 0)
            {
                MessageBox.Show("더 이상 대출이 불가능합니다.");
                return;
            }

            // DataManager의 RentBook 메소드 호출하여 책 대여 처리
            bool success = dataManager.RentBook(bookId, memberId, bookTitle, memberName);

            if (!success)
            {
                MessageBox.Show("동일한 책은 한 번만 대여할 수 있습니다.");
                return;
            }

            MessageBox.Show("도서가 성공적으로 대여되었습니다.");

            string logMessage = $"[{DateTime.Now}] 회원 {memberName}이(가) '{bookTitle}' 도서를 대여하였습니다.";
            checkoutLogs.Items.Insert(0, logMessage);

            // 로그 메시지를 텍스트 파일에 추가
            using (StreamWriter sw = File.AppendText("checkoutLogs.txt"))
            {
                sw.WriteLine(logMessage);
            }

            // 필요한 경우 UI 업데이트 등 추가 작업 수행
            ClearRentalFields();

            var checkouts = dataManager.LoadCheckouts();  // 데이터베이스에서 체크아웃 정보 불러오기

            // checkoutStatusGrid 업데이트
            checkoutStatusGrid.ItemsSource = null;
            var checkoutsNotReturned = checkouts.Where(c => c.IsReturned == false).ToList();
            checkoutStatusGrid.ItemsSource = checkoutsNotReturned;

            checkoutAllGrid.ItemsSource = null;
            checkoutAllGrid.ItemsSource = checkouts;

            UpdateLabelsAndGrid(bookId);
            UpdateMembersGrid();

            UpdateBooksGrid();
        }

        private void UpdateBooksGrid()
        {
            var books = dataManager.LoadBooks();
            bookStatusGrid.ItemsSource = null;
            bookStatusGrid.ItemsSource = books;
        }

        // 새로운 함수: 회원 데이터 그리드 업데이트 
        private void UpdateMembersGrid()
        {
            var members = dataManager.LoadMembers();
            memberStatusGrid.ItemsSource = null;
            memberStatusGrid.ItemsSource = members;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            int bookId;
            int memberId;
            string bookTitle = txtBookTitle.Text.Trim();
            string memberName = txtMemberName.Text.Trim();

            if (!int.TryParse(txtBookId.Text, out bookId) || !int.TryParse(txtMemberId.Text, out memberId))
            {
                MessageBox.Show("유효한 도서 번호와 사용자 ID를 입력하세요.");
                return;
            }

            var selectedMember = dataManager.GetMemberById(memberId);

            // 멤버 이름 확인
            if (selectedMember == null || selectedMember.Name != memberName)
            {
                MessageBox.Show("해당 ID의 회원이 존재하지 않거나 이름이 일치하지 않습니다.");
                return;
            }

            var selectedBook = dataManager.GetBookById(bookId);

            // 도서 제목 확인
            if (selectedBook == null || selectedBook.Title != bookTitle)
            {
                MessageBox.Show("해당 ID의 도서가 존재하지 않거나 제목이 일치하지 않습니다.");
                return;
            }

            // 해당 도서가 실제로 대여되었는지 확인합니다.
            if (!dataManager.IsBookCheckedOutByMember(bookId, memberId))
            {
                MessageBox.Show("대여 정보가 없습니다.");
                return;
            }

            // 연체 상태 확인
            var checkout = dataManager.GetCheckoutByBookAndMemberId(bookId, memberId);
            if (checkout != null && checkout.IsOverdue)
            {
                MessageBoxResult result = MessageBox.Show("이 책은 연체 상태입니다. 그래도 반납하시겠습니까?", "연체 알림", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                    return;
            }

            // DataManager의 ReturnBook 메소드 호출하여 책 반납 처리
            bool success = dataManager.ReturnBook(bookId, memberId);

            if (success)
            {
                MessageBox.Show("도서가 성공적으로 반납되었습니다.");

                // 로그 메시지 설정
                string logMessage;
                if (checkout != null && checkout.IsOverdue)
                    logMessage = $"[{DateTime.Now}] 회원 {memberName}이(가) '{bookTitle}' 도서를 연체 반납하였습니다.";
                else
                    logMessage = $"[{DateTime.Now}] 회원 {memberName}이(가) '{bookTitle}' 도서를 반납하였습니다.";

                checkoutLogs.Items.Insert(0, logMessage);

                // 로그 메시지를 텍스트 파일에 추가
                using (StreamWriter sw = File.AppendText("checkoutLogs.txt"))
                {
                    sw.WriteLine(logMessage);
                }

                // 필요한 경우 UI 업데이트 등 추가 작업 수행
                ClearRentalFields();

                var checkouts = dataManager.LoadCheckouts();  // 데이터베이스에서 체크아웃 정보 불러오기

                // checkoutStatusGrid 업데이트
                checkoutStatusGrid.ItemsSource = null;
                var checkoutsNotReturned = checkouts.Where(c => c.IsReturned == false).ToList();
                checkoutStatusGrid.ItemsSource = checkoutsNotReturned;

                checkoutAllGrid.ItemsSource = null;
                checkoutAllGrid.ItemsSource = checkouts;

                UpdateLabelsAndGrid(bookId);
                UpdateMembersGrid();
            }
        }
        private void ClearRentalFields()
        {
            txtBookId.Clear();
            txtMemberId.Clear();
            txtBookTitle.Clear();
            txtMemberName.Clear();
        }

        private void UpdateLabelsAndGrid(int bookId)
        {
            // DataManager의 GetBookById 메소드 호출하여 DB에서 책 정보 가져오기
            var selectedBook = dataManager.GetBookById(bookId);

            if (selectedBook != null)
            {
                // 데이터 그리드 업데이트
                var books = dataManager.LoadBooks();  // LoadBooks()는 모든 도서 정보를 가져오는 메소드입니다.
                bookStatusGrid.ItemsSource = null;
                bookStatusGrid.ItemsSource = books;

                // 라벨 업데이트
                int totalAvailableCopies = 0;
                int totalCopies = 0;

                foreach (var book in books)
                {
                    totalAvailableCopies += dataManager.GetAvailableCopies(book.BookId);
                    totalCopies += Convert.ToInt32(book.Quantity);
                }

                lblAvailableBooks.Content = $"대출 가능한 도서 수 : {totalAvailableCopies}권";

                // 전체 복사본 수에서 사용 가능한 복사본 수를 뺌으로써 대출 중인 복사본수 계산
                lblBorrowedBooks.Content = $"대출 중인 도서 수 : {totalCopies - totalAvailableCopies}권";

                int overdueBooksCount = dataManager.GetOverdueBookCount();
                lblOverdueBooks.Content = $"연체 중인 도서 수 : {overdueBooksCount}권";
            }
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("notepad.exe", "checkoutLogs.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일을 열지 못했습니다: {ex.Message}");
            }
        }

        private void CheckoutSearchButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = checkoutSearchByComboBox.SelectedItem as ComboBoxItem;
            string searchText = checkoutSearchTextBox.Text;

            if (selectedItem.Content.ToString() == "이름")
            {
                // 이름으로 검색
                var membersByName = dataManager.GetCheckoutsByName(searchText);

                // 결과를 DataGrid에 표시
                checkoutAllGrid.ItemsSource = null;
                checkoutAllGrid.ItemsSource = membersByName;
            }
            else if (selectedItem.Content.ToString() == "연락처")
            {
                // 연락처로 검색
                var membersByContact = dataManager.GetCheckoutsByContact(searchText);

                // 결과를 DataGrid에 표시
                checkoutAllGrid.ItemsSource = null;
                checkoutAllGrid.ItemsSource = membersByContact;
            }
        }
    }
}
