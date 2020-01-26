using System.Windows;
using System.Windows.Controls;

namespace RagiFiler.Views.Controls
{
    public partial class SearchResultView : UserControl
    {
        public SearchResultView()
        {
            InitializeComponent();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
