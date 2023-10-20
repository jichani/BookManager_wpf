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

            // Connect to database and execute query (in a separate task)
            Task.Run(() =>
            {
                bool isConnected = dataManager.ConnectToDatabase();

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (isConnected)
                        MessageBox.Show("Connected to database successfully.");
                    else
                        MessageBox.Show("Failed to connect to database.");
                }));

            });
        }
    }
}
