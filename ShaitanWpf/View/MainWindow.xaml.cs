using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ShaitanWpf.ViewModel;

namespace ShaitanWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new RecognizedPagesViewModel();
            

        }

        
    }
}
