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

            // Create DataManager instance
            dataManager = new DataManager();

            Task.Run(() =>
            {
                var books = dataManager.Load();  // Load books from database

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (books.Count > 0)  // If there are any books...
                    {
                        foreach (var book in books)
                        {
                            MessageBox.Show(book.ToString());  // Display each book's information in a message box.
                        }
                    }
                    else
                    {
                        MessageBox.Show("No books found.");
                    }
                }));
            });
        }
    }

}
