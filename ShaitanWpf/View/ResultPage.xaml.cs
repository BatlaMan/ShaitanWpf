using System.Windows.Controls;
using ShaitanWpf.ViewModel;
using SoundIdentification;

namespace ShaitanWpf.View
{
    /// <summary>
    /// Interaction logic for ResultPage.xaml
    /// </summary>
    public partial class ResultPage : Page
    {
        public ResultPage(QueryResult result)
        {
            InitializeComponent();
            DataContext = new ResultViewModel(result);
        }
    }
}
