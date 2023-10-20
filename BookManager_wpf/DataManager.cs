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
                            Quantity = reader.GetInt32(7),
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
    }
}