using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using BookManager_wpf.Models;


namespace BookManager_wpf
{

    internal class DataManager
    {
        private string connectionString = "Data Source=bookmanager.db";

        public DataManager()
        {
            InitializeDatabase();
        }

        /// <summary>
        /// 데이터베이스 초기화
        /// </summary>
        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string createBooksTable = @"
                    CREATE TABLE IF NOT EXISTS books (
                        book_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        title TEXT NOT NULL,
                        category TEXT,
                        author TEXT,
                        description TEXT,
                        publisher TEXT,
                        publication_date TEXT,
                        quantity TEXT,
                        registered_date DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";

                string createMembersTable = @"
                    CREATE TABLE IF NOT EXISTS members (
                        member_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
                        mobile_number TEXT,
                        createdDate DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";

                string createCheckoutsTable = @"
                    CREATE TABLE IF NOT EXISTS checkouts (
                        checkout_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        book_id INTEGER,
                        member_id INTEGER,
                        checkout_date DATETIME DEFAULT CURRENT_TIMESTAMP,
                        return_date DATETIME,
                        title TEXT,
                        name TEXT,
                        FOREIGN KEY (book_id) REFERENCES books (book_id),
                        FOREIGN KEY (member_id) REFERENCES members (member_id)
                    )";

                SqliteCommand cmd = new SqliteCommand(createBooksTable, connection);
                cmd.ExecuteNonQuery();

                cmd = new SqliteCommand(createMembersTable, connection);
                cmd.ExecuteNonQuery();

                cmd = new SqliteCommand(createCheckoutsTable, connection);
                cmd.ExecuteNonQuery();

                InsertSampleData(connection);
            }
        }

        /// <summary>
        /// 샘플 데이터 삽입
        /// </summary>
        private void InsertSampleData(SqliteConnection connection)
        {
            // 기존 데이터가 있는지 확인
            string checkDataQuery = "SELECT COUNT(*) FROM books";
            SqliteCommand checkCmd = new SqliteCommand(checkDataQuery, connection);
            int bookCount = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (bookCount > 0) return; // 이미 데이터가 있으면 실행하지 않음

            // 샘플 도서 데이터 삽입
            string[] sampleBooks = {
                "INSERT INTO books (title, category, author, description, publisher, publication_date, quantity, registered_date) VALUES " +
                "('해리 포터와 마법사의 돌', '판타지', 'J.K. 롤링', '마법사 해리 포터의 모험이 시작되는 첫 번째 이야기', '문학수첩', '1997-06-26', '5', datetime('now'))",

                "INSERT INTO books (title, category, author, description, publisher, publication_date, quantity, registered_date) VALUES " +
                "('클린 코드', 'IT/프로그래밍', '로버트 C. 마틴', '애자일 소프트웨어 장인 정신', '인사이트', '2008-08-01', '3', datetime('now'))",

                "INSERT INTO books (title, category, author, description, publisher, publication_date, quantity, registered_date) VALUES " +
                "('1984', '소설', '조지 오웰', '전체주의 사회를 그린 디스토피아 소설', '민음사', '1949-06-08', '4', datetime('now'))",

                "INSERT INTO books (title, category, author, description, publisher, publication_date, quantity, registered_date) VALUES " +
                "('코스모스', '과학', '칼 세이건', '우주에 대한 경이로운 탐험', '사이언스북스', '1980-09-28', '2', datetime('now'))",

                "INSERT INTO books (title, category, author, description, publisher, publication_date, quantity, registered_date) VALUES " +
                "('데미안', '문학', '헤르만 헤세', '청년의 성장과 자아 발견을 그린 성장소설', '민음사', '1919-01-01', '3', datetime('now'))"
            };

            foreach (string bookQuery in sampleBooks)
            {
                SqliteCommand bookCmd = new SqliteCommand(bookQuery, connection);
                bookCmd.ExecuteNonQuery();
            }

            // 샘플 회원 데이터 삽입
            string[] sampleMembers = {
                "INSERT INTO members (name, mobile_number, createdDate) VALUES ('김철수', '010-1234-5678', datetime('now', '-30 days'))",
                "INSERT INTO members (name, mobile_number, createdDate) VALUES ('박영희', '010-2345-6789', datetime('now', '-25 days'))",
                "INSERT INTO members (name, mobile_number, createdDate) VALUES ('이민수', '010-3456-7890', datetime('now', '-20 days'))",
                "INSERT INTO members (name, mobile_number, createdDate) VALUES ('정수현', '010-4567-8901', datetime('now', '-15 days'))",
                "INSERT INTO members (name, mobile_number, createdDate) VALUES ('최지영', '010-5678-9012', datetime('now', '-10 days'))"
            };

            foreach (string memberQuery in sampleMembers)
            {
                SqliteCommand memberCmd = new SqliteCommand(memberQuery, connection);
                memberCmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 책 정보 가져오기
        /// </summary>
        /// <returns>Books 객체의 리스트</returns>
        public List<Books> LoadBooks()
        {
            var books = new List<Books>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM books";

                SqliteCommand cmd = new SqliteCommand(query, connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var book = new Books
                        {
                            BookId = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Category = reader.GetString(2),
                            Author = reader.GetString(3),
                            Description = reader.GetString(4),
                            Publisher = reader.GetString(5),
                            PublicationDate = reader.GetString(6),  // 문자열로 처리됨.
                            QuantityAvailable = GetAvailableCopies(reader.GetInt32(0)),
                            Quantity = reader.GetString(7),
                            RegisteredDate = reader.GetDateTime(8)
                        };
                        books.Add(book);
                    }
                }
                return books;
            }
        }

        /// <summary>
        /// 유저 정보 가져오기
        /// </summary>
        public List<Members> LoadMembers()
        {
            var members = new List<Members>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM members";

                SqliteCommand cmd = new SqliteCommand(query, connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var member = new Members
                        {
                            MemberId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            MobileNumber = reader.GetString(2),
                            CreatedDate = reader.GetDateTime(3),
                            AvailableBookCount = GetAvailableBookCountForMember(reader.GetInt32(0))
                        };
                        members.Add(member);
                    }
                }

                return members;
            }
        }

        /// <summary>
        /// 유저 정보 추가
        /// </summary>
        public void AddNewMember(Members newMember)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO members(name, mobile_number, createdDate) VALUES(@name, @mobileNumber, @createdDate)";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", newMember.Name);
                cmd.Parameters.AddWithValue("@mobileNumber", newMember.MobileNumber);
                cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 아이디를 통해 유저 정보 가져오기
        /// </summary>
        public Members GetMemberById(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM members WHERE member_id = @id";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Members
                        {
                            MemberId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            MobileNumber = reader.GetString(2),
                            CreatedDate = reader.GetDateTime(3),
                            AvailableBookCount = GetAvailableBookCountForMember(id)
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 유저 정보 업데이트
        /// </summary>
        public void UpdateMember(Members updatedMember)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE members SET name = @name, mobile_number = @mobileNumber WHERE member_id = @id";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", updatedMember.Name);
                cmd.Parameters.AddWithValue("@mobileNumber", updatedMember.MobileNumber);
                cmd.Parameters.AddWithValue("@id", updatedMember.MemberId);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 유저 정보 삭제하기
        /// </summary>
        public void DeleteMember(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE FROM members WHERE member_id = @id";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 책 정보 추가하기
        /// </summary>
        public void AddNewBook(Books newBook)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO books(title, category, author, description, publisher, publication_date, quantity, registered_date) VALUES(@title, @category, @author, @description, @publisher, @publicationDate, @quantity, @registeredDate)";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@title", newBook.Title);
                cmd.Parameters.AddWithValue("@category", newBook.Category);
                cmd.Parameters.AddWithValue("@author", newBook.Author);
                cmd.Parameters.AddWithValue("@description", newBook.Description);
                cmd.Parameters.AddWithValue("@publisher", newBook.Publisher);
                cmd.Parameters.AddWithValue("@publicationDate", newBook.PublicationDate);
                cmd.Parameters.AddWithValue("@quantity", newBook.Quantity);
                cmd.Parameters.AddWithValue("@registeredDate", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 책 아이디로 책 정보 가져오기
        /// </summary>
        public Books GetBookById(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM books WHERE book_id = @bookId";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Books
                        {
                            BookId = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Category = reader.GetString(2),
                            Author = reader.GetString(3),
                            Description = reader.GetString(4),
                            Publisher = reader.GetString(5),
                            PublicationDate = reader.GetString(6),
                            QuantityAvailable = GetAvailableCopies(reader.GetInt32(0)),
                            Quantity = reader.GetString(7),
                            RegisteredDate = reader.GetDateTime(8)
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 책 정보 수정하기
        /// </summary>
        public void UpdateBook(Books updatedBook)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE books SET title = @title, category = @category, author = @author, description = @description, publisher = @publisher, publication_date = @publicationDate, quantity=@quantity WHERE book_id= @bookId";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@title", updatedBook.Title);
                cmd.Parameters.AddWithValue("@category", updatedBook.Category);
                cmd.Parameters.AddWithValue("@author", updatedBook.Author);
                cmd.Parameters.AddWithValue("@description", updatedBook.Description);
                cmd.Parameters.AddWithValue("@publisher", updatedBook.Publisher);
                cmd.Parameters.AddWithValue("@publicationDate", updatedBook.PublicationDate);
                cmd.Parameters.AddWithValue("@quantity", updatedBook.Quantity);
                cmd.Parameters.AddWithValue("@bookId", updatedBook.BookId);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 책 정보 삭제하기
        /// </summary>
        public void DeleteBook(int bookId)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE FROM books WHERE book_id = @bookId";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", bookId);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 책 정보 검색하기
        /// </summary>
        public List<Books> SearchBooksByTitle(string title)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM books WHERE title LIKE @title";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@title", $"%{title}%");

                List<Books> books = new List<Books>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Books book = new Books
                        {
                            BookId = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Category = reader.GetString(2),
                            Author = reader.GetString(3),
                            Description = reader.GetString(4),
                            Publisher = reader.GetString(5),
                            PublicationDate = reader.GetString(6),
                            Quantity = reader.GetString(7),
                            RegisteredDate = reader.GetDateTime(8)
                        };

                        books.Add(book);
                    }
                }

                return books;
            }
        }

        /// <summary>
        /// 책 분류 검색하기
        /// </summary>
        public List<Books> SearchBooksByCategory(string category)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM books WHERE category LIKE @category";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@category", $"%{category}%");

                List<Books> books = new List<Books>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Books book = new Books
                        {
                            BookId = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Category = reader.GetString(2),
                            Author = reader.GetString(3),
                            Description = reader.GetString(4),
                            Publisher = reader.GetString(5),
                            PublicationDate = reader.GetString(6),
                            Quantity = reader.GetString(7),
                            RegisteredDate = reader.GetDateTime(8)
                        };

                        books.Add(book);
                    }
                }

                return books;
            }
        }

        /// <summary>
        /// 책 저자 검색하기
        /// </summary>
        public List<Books> SearchBooksByPublisher(string publisher)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM books WHERE publisher LIKE @publisher";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@publisher", $"%{publisher}%");

                List<Books> books = new List<Books>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Books book = new Books
                        {
                            BookId = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Category = reader.GetString(2),
                            Author = reader.GetString(3),
                            Description = reader.GetString(4),
                            Publisher = reader.GetString(5),
                            PublicationDate = reader.GetString(6),
                            Quantity = reader.GetString(7),
                            RegisteredDate = reader.GetDateTime(8)
                        };

                        books.Add(book);
                    }
                }

                return books;
            }
        }

        public int GetAvailableCopies(int bookId)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM checkouts WHERE book_id = @bookId AND return_date IS NULL";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", bookId);

                int checkedOutCopiesCount = Convert.ToInt32(cmd.ExecuteScalar());

                int quantityOfBook = GetQuantityOfBook(bookId);

                return quantityOfBook - checkedOutCopiesCount;
            }
        }

        public int GetQuantityOfBook(int bookId)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT quantity FROM books WHERE book_id = @bookId";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", bookId);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool RentBook(int bookId, int memberId, string bookTitle, string memberName)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // 첫 번째로, 해당 회원이 이미 해당 도서를 대여하고 있는지 확인합니다.
                string checkQuery = "SELECT COUNT(*) FROM checkouts WHERE book_id=@bookId AND member_id=@memberId AND return_date IS NULL";

                SqliteCommand checkCmd = new SqliteCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@bookId", bookId);
                checkCmd.Parameters.AddWithValue("@memberId", memberId);

                object resultObj = checkCmd.ExecuteScalar();

                // 만약 결과가 1 이상이면 (즉, 같은 책을 빌린 기록이 있다면) false 반환
                if (Convert.ToInt32(resultObj) > 0)
                {
                    return false;
                }

                // 만약 위의 검사에서 문제가 없다면(즉 같은 책을 빌린 기록이 없다면), 실제로 대출 처리 진행
                string query = "INSERT INTO checkouts(book_id, member_id, checkout_date, title,name) VALUES(@bookId2,@memberId2,@checkoutDate,@bookTitle,@memberName)";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId2", bookId);
                cmd.Parameters.AddWithValue("@memberId2", memberId);
                cmd.Parameters.AddWithValue("@checkoutDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@bookTitle", bookTitle); // 책 제목 추가
                cmd.Parameters.AddWithValue("@memberName", memberName); // 회원 이름 추가

                var resultInsertion = cmd.ExecuteNonQuery();

                return resultInsertion > 0;
            }
        }

        public bool IsBookCheckedOutByMember(int bookId, int memberId)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM checkouts WHERE book_id = @bookId AND member_id= @memberId AND return_date IS NULL";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", bookId);
                cmd.Parameters.AddWithValue("@memberId", memberId);

                var result = Convert.ToInt32(cmd.ExecuteScalar());

                return result > 0;
            }
        }

        public bool ReturnBook(int bookId, int memberId)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE checkouts SET return_date = @returnDate WHERE book_id = @bookId AND member_id= @memberId AND return_date IS NULL";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@returnDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@bookId", bookId);
                cmd.Parameters.AddWithValue("@memberId", memberId);

                var result = cmd.ExecuteNonQuery();

                return result > 0;
            }
        }

        public List<Checkouts> LoadCheckouts()
        {
            var checkouts = new List<Checkouts>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // 모든 레코드를 선택하는 쿼리로 변경
                string query = "SELECT * FROM checkouts";

                SqliteCommand cmd = new SqliteCommand(query, connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var checkout = new Checkouts
                        {
                            CheckoutId = reader.GetInt32(0),
                            MemberId = reader.GetInt32(2),
                            BookId = reader.GetInt32(1),
                            CheckoutDate = reader.GetDateTime(3),
                            ReturnDate = !reader.IsDBNull(4) ? reader.GetDateTime(4) : (DateTime?)null, // null을 처리하기 위해 사용합니다.
                            Name = reader.GetString(5),
                            Title = reader.GetString(6)
                        };
                        checkouts.Add(checkout);
                    }
                }
            }

            return checkouts;
        }



        public int GetOverdueBookCount()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM checkouts WHERE checkout_date < @sevenDaysAgo AND return_date IS NULL";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@sevenDaysAgo", DateTime.Now.AddDays(-7));

                var result = Convert.ToInt32(cmd.ExecuteScalar());

                return result;
            }
        }

        public int GetAvailableBookCountForMember(int memberId)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM checkouts WHERE member_id = @memberId AND return_date IS NULL";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@memberId", memberId);

                var checkedOutBooksCount = Convert.ToInt32(cmd.ExecuteScalar());

                return Math.Max(0, 3 - checkedOutBooksCount); // 최대 3권에서 현재 대출 중인 도서수를 뺀다.
            }
        }

        public Checkouts GetCheckoutByBookAndMemberId(int bookId, int memberId)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM checkouts WHERE book_id = @bookId AND member_id= @memberId AND return_date IS NULL";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", bookId);
                cmd.Parameters.AddWithValue("@memberId", memberId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Checkouts
                        {
                            CheckoutId = reader.GetInt32(0),
                            MemberId = reader.GetInt32(2),
                            BookId = reader.GetInt32(1),
                            CheckoutDate = reader.GetDateTime(3),
                            ReturnDate = !reader.IsDBNull(4) ? reader.GetDateTime(4) : (DateTime?)null,
                            Name = reader.GetString(5),
                            Title = reader.GetString(6)
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        public List<Checkouts> GetCheckoutsByName(string name)
        {
            var checkouts = new List<Checkouts>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM checkouts WHERE name LIKE @name";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", $"%{name}%");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var checkout = new Checkouts
                        {
                            CheckoutId = reader.GetInt32(0),
                            MemberId = reader.GetInt32(2),
                            BookId = reader.GetInt32(1),
                            CheckoutDate = reader.GetDateTime(3),
                            ReturnDate = !reader.IsDBNull(4) ? reader.GetDateTime(4) : (DateTime?)null,
                            Name = reader.GetString(5),
                            Title = reader.GetString(6)
                        };
                        checkouts.Add(checkout);
                    }
                }

                return checkouts;
            }
        }

        public List<Checkouts> GetCheckoutsByContact(string contact)
        {
            var checkouts = new List<Checkouts>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT checkouts.* 
                         FROM checkouts 
                         JOIN members ON checkouts.member_id = members.member_id 
                         WHERE members.mobile_number LIKE @contact";

                SqliteCommand cmd = new SqliteCommand(query, connection);
                cmd.Parameters.AddWithValue("@contact", $"%{contact}%");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var checkout = new Checkouts
                        {
                            CheckoutId = reader.GetInt32(0),
                            MemberId = reader.GetInt32(2),
                            BookId = reader.GetInt32(1),
                            CheckoutDate = reader.GetDateTime(3),
                            ReturnDate = !reader.IsDBNull(4) ? reader.GetDateTime(4) : (DateTime?)null,
                            Name = reader.GetString(5),
                            Title = reader.GetString(6)
                        };
                        checkouts.Add(checkout);
                    }
                }

                return checkouts;
            }
        }
    }
}