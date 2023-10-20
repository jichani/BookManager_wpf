using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace BookManager_wpf
{
    internal class DataManager
    {
        private string connectionString = "Server=localhost;Database=bookmanager;Uid=root;Pwd=7640;";

        public bool ConnectToDatabase()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM books";

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader[0].ToString());
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return false;
            }
        }
    }
}
