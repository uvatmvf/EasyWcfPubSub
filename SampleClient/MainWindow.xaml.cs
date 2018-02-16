using System;
using System.Windows;

namespace TestPubSub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            (DataContext as IDisposable)?.Dispose();
        }
    }
}
