using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace XamlBrewer.WinUI3.DataGrid.Sample.Views
{
    public sealed partial class DetailsPage : Page
    {
        public DetailsPage()
        {
            InitializeComponent();
        }

        public void ShowTabs()
        {
            tvMountains.Visibility = Visibility.Visible;
        }

        public void AddTab(TabViewItem tab)
        {
            tvMountains.TabItems.Add(tab);
        }
    }
}
