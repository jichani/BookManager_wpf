using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace BookManager_wpf
{
    public class Books
    {
        public int BookId { get; set; } // 책 ID
        public string Title { get; set; } // 책 제목
        public string Category { get; set; } // 카테고리 
        public string Author { get; set; } // 저자 
        public string Description { get; set; }  // 설명 
        public string Publisher { get; set; }  // 출판사 
        public string PublicationDate { get; set; }  // 출판일 (문자열)
        public int Quantity { get; set; }  // 수량 
        public DateTime RegisteredDate { get; set; }  // 등록일  
        public override string ToString()
        {
            return $"ID: {BookId}, Title: {Title}, Category: {Category}, Author: {Author}, Description: {Description}, Publisher: {Publisher}, PublicationDate: {PublicationDate}, Quantity: {Quantity}, RegisteredDate:{RegisteredDate}";
        }
    }

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
        public List<Books> Load()
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
    }
}