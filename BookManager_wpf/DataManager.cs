using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BookManager_wpf.Models;


namespace BookManager_wpf
{

    internal class DataManager
    {
        private string connectionString = "Server=localhost;" +
                                          "Database=bookmanager;" +
                                          "Uid=root;" + "Pwd=7640;";

        /// <summary>
        /// 데이터베이스로부터 책 정보를 불러오는 메서드.
        /// 이 메서드는 Books 객체의 리스트를 반환한다.
        /// 각 객체는 books 테이블의 한 행에 해당한다.
        /// </summary>
        /// <returns>Books 객체의 리스트</returns>
        public List<Books> LoadBooks()
        {
            var books = new List<Books>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM books";

                MySqlCommand cmd = new MySqlCommand(query, connection);

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

        public List<Members> LoadMembers()
        {
            var members = new List<Members>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM members";

                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var member = new Members
                        {
                            MemberId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            MobileNumber = reader.GetString(2),
                            CreatedDate = reader.GetDateTime(3)
                        };
                        members.Add(member);
                    }
                }

                return members;
            }
        }

        public void AddNewMember(Members newMember)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO members(name, mobile_number, createdDate) VALUES(@name, @mobileNumber, @createdDate)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", newMember.Name);
                cmd.Parameters.AddWithValue("@mobileNumber", newMember.MobileNumber);
                cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
        }

        public Members GetMemberById(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM members WHERE member_id = @id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
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
                            CreatedDate = reader.GetDateTime(3)
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }


        public void UpdateMember(Members updatedMember)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE members SET name = @name, mobile_number = @mobileNumber WHERE member_id = @id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@name", updatedMember.Name);
                cmd.Parameters.AddWithValue("@mobileNumber", updatedMember.MobileNumber);
                cmd.Parameters.AddWithValue("@id", updatedMember.MemberId);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteMember(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE FROM members WHERE member_id = @id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
        }



        public void AddNewBook(Books newBook)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO books(title, category, author, description, publisher, publication_date, quantity, registered_date) VALUES(@title, @category, @author, @description, @publisher, @publicationDate, @quantity, @registeredDate)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
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

        public Books GetBookById(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM books WHERE book_id = @bookId";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Books
                        {
                            BookId = reader.GetInt32("book_id"),
                            Title = reader.GetString("title"),
                            Category = reader.GetString("category"),
                            Author = reader.GetString("author"),
                            Description = reader.GetString("description"),
                            Publisher = reader.GetString("publisher"),
                            PublicationDate = reader.GetString("publication_date"),
                            Quantity = reader.GetString("quantity"),
                            RegisteredDate = reader.GetDateTime("registered_date")
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        public void UpdateBook(Books updatedBook)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE books SET title = @title, category = @category, author = @author, description = @description, publisher = @publisher, publication_date = @publicationDate, quantity=@quantity WHERE book_id= @bookId";

                MySqlCommand cmd = new MySqlCommand(query, connection);
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

        public void DeleteBook(int bookId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE FROM books WHERE book_id = @bookId";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", bookId);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Books> SearchBooksByTitle(string title)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM books WHERE title LIKE @title";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@title", $"%{title}%");

                List<Books> books = new List<Books>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Books book = new Books
                        {
                            BookId = reader.GetInt32("book_id"),
                            Title = reader.GetString("title"),
                            Category = reader.GetString("category"),
                            Author = reader.GetString("author"),
                            Description = reader.GetString("description"),
                            Publisher = reader.GetString("publisher"),
                            PublicationDate = reader.GetString("publication_date"),
                            Quantity = reader.GetString("quantity"),
                            RegisteredDate = reader.GetDateTime("registered_date")
                        };

                        books.Add(book);
                    }
                }

                return books;
            }
        }
        public List<Books> SearchBooksByCategory(string category)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM books WHERE category LIKE @category";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@category", $"%{category}%");

                List<Books> books = new List<Books>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Books book = new Books
                        {
                            BookId = reader.GetInt32("book_id"),
                            Title = reader.GetString("title"),
                            Category = reader.GetString("category"),
                            Author = reader.GetString("author"),
                            Description = reader.GetString("description"),
                            Publisher = reader.GetString("publisher"),
                            PublicationDate = reader.GetString("publication_date"),
                            Quantity = reader.GetString("quantity"),
                            RegisteredDate = reader.GetDateTime("registered_date")
                        };

                        books.Add(book);
                    }
                }

                return books;
            }
        }
        public List<Books> SearchBooksByPublisher(string publisher)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM books WHERE publisher LIKE @publisher";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@publisher", $"%{publisher}%");

                List<Books> books = new List<Books>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Books book = new Books
                        {
                            BookId = reader.GetInt32("book_id"),
                            Title = reader.GetString("title"),
                            Category = reader.GetString("category"),
                            Author = reader.GetString("author"),
                            Description = reader.GetString("description"),
                            Publisher = reader.GetString("publisher"),
                            PublicationDate = reader.GetString("publication_date"),
                            Quantity = reader.GetString("quantity"),
                            RegisteredDate = reader.GetDateTime("registered_date")
                        };

                        books.Add(book);
                    }
                }

                return books;
            }
        }

        public int GetAvailableCopies(int bookId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM checkouts WHERE book_id = @bookId AND return_date IS NULL";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", bookId);

                // ExecuteScalar()은 쿼리의 결과로 반환된 첫 번째 행의 첫 번째 열을 반환합니다.
                // 이 경우, COUNT(*) 결과가 됩니다.
                int checkedOutCopiesCount = Convert.ToInt32(cmd.ExecuteScalar());

                Books book = GetBookById(bookId);

                return Convert.ToInt32(book.Quantity) - checkedOutCopiesCount;
            }
        }

        public bool RentBook(int bookId, int memberId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO checkouts(book_id, member_id, checkout_date) VALUES(@bookId, @memberId, @checkoutDate)";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", bookId);
                cmd.Parameters.AddWithValue("@memberId", memberId);
                cmd.Parameters.AddWithValue("@checkoutDate", DateTime.Now);

                var result = cmd.ExecuteNonQuery();

                return result > 0;
            }
        }
        public bool IsBookCheckedOutByMember(int bookId, int memberId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM checkouts WHERE book_id = @bookId AND member_id= @memberId AND return_date IS NULL";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@bookId", bookId);
                cmd.Parameters.AddWithValue("@memberId", memberId);

                var result = Convert.ToInt32(cmd.ExecuteScalar());

                return result > 0;
            }
        }

        public bool ReturnBook(int bookId, int memberId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE checkouts SET return_date = @returnDate WHERE book_id = @bookId AND member_id= @memberId AND return_date IS NULL";

                MySqlCommand cmd = new MySqlCommand(query, connection);
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

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM checkouts";

                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var checkout = new Checkouts
                        {
                            CheckoutId = reader.GetInt32(0),
                            BookId = reader.GetInt32(1),
                            MemberId = reader.GetInt32(2),
                            CheckoutDate = reader.GetDateTime(3),
                            ReturnDate = !reader.IsDBNull(4) ? reader.GetDateTime(4) : (DateTime?)null // null을 처리하기 위해 사용합니다.
                        };
                        checkouts.Add(checkout);
                    }
                }
            }

            return checkouts;
        }

        public int GetOverdueBookCount()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM checkouts WHERE checkout_date < @sevenDaysAgo AND return_date IS NULL";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@sevenDaysAgo", DateTime.Now.AddDays(-7));

                var result = Convert.ToInt32(cmd.ExecuteScalar());

                return result;
            }
        }

    }
}