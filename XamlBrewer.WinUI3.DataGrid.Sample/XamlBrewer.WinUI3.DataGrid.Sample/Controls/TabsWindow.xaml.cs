using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using XamlBrewer.WinUI3.DataGrid.Sample;

namespace XamlBrewer.WinUI.Controls
{
    public sealed partial class TabsWindow : Window
    {
        private const string DataIdentifier = "TabItem";

        public TabsWindow()
        {
            InitializeComponent();
        }

        public TabsWindow(ElementTheme requestedTheme) : this()
        {
            Root.RequestedTheme = requestedTheme;
        }

        public void AddTab(TabViewItem tab)
        {
            tabView.TabItems.Add(tab);
        }

        private void Tab_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            tabView.TabItems.Remove(args.Tab);
        }

        private void TabView_TabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args)
        {
            args.Data.Properties.Add(DataIdentifier, args.Tab);
            args.Data.RequestedOperation = DataPackageOperation.Move;
        }

        private void TabView_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {
            var tab = args.Tab;
            tabView.TabItems.Remove(tab);

            TabsWindow window = new(Root.ActualTheme) { Title = (Application.Current as App).Title };
            window.AddTab(tab);
            window.Activate();
        }

        private void TabView_TabStripDrop(object sender, DragEventArgs e)
        {
            // Never called.
        }

        private void TabView_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties[DataIdentifier] is TabViewItem)
            {
                e.AcceptedOperation = DataPackageOperation.Move;
            }
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

        // Event is not raised when the drop is handled by another Window.
        private void TabView_TabItemsChanged(TabView sender, IVectorChangedEventArgs args)
        {
            if (sender.TabItems.Count == 0)
            {
                Close();
            }
        }

        private void TabView_TabDragCompleted(TabView sender, TabViewTabDragCompletedEventArgs args)
        {
            if (sender.TabItems.Count == 0)
            {
                Close();
            }
        }
    }
}
