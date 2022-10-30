using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel.DataTransfer;

namespace XamlBrewer.WinUI.Controls
{
    public sealed partial class TabsWindow : Window
    {
        private const string DataIdentifier = "MountainTabItem";

        public TabsWindow()
        {
            Title = "Tabs";

            InitializeComponent();
        }

        public void AddTab(TabViewItem tab)
        {
            tabView.TabItems.Add(tab);
        }

        private void TabView_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {

        }

        private void TabView_TabStripDrop(object sender, DragEventArgs e)
        {
            // Never called.
        }

        private void TabView_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        private void TabView_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties[DataIdentifier] is TabViewItem)
            {
                var tvi = e.DataView.Properties[DataIdentifier] as TabViewItem;
                var tvlv = tvi?.Parent as TabViewListView;
                if (tvlv is not null)
                {
                    tvlv.Items.Remove(tvi);
                    tabView.TabItems.Add(tvi);
                }
            }
        }
    }
}
