using Microsoft.Toolkit.Uwp.SampleApp.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using ctWinUI = CommunityToolkit.WinUI.UI.Controls;

namespace XamlBrewer.WinUI3.DataGrid.Sample.Views
{
    public sealed partial class HomePage : Page
    {
        private DataGridDataSource _dataSource = new DataGridDataSource();
        private string _grouping;

        public HomePage()
        {
            this.InitializeComponent();
            this.Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid.ItemsSource = await _dataSource.GetDataAsync();
            DataGrid.Columns[0].SortDirection = ctWinUI.DataGridSortDirection.Ascending;
        }

        private void DataGrid_Sorting(object sender, ctWinUI.DataGridColumnEventArgs e)
        {
            // Add sorting indicator, and sort
            var isAscending = e.Column.SortDirection == null || e.Column.SortDirection == ctWinUI.DataGridSortDirection.Descending;
            DataGrid.ItemsSource = _dataSource.SortData(e.Column.Tag.ToString(), isAscending);
            e.Column.SortDirection = isAscending
                ? ctWinUI.DataGridSortDirection.Ascending
                : ctWinUI.DataGridSortDirection.Descending;

            // Remove sorting indicators from other columns
            foreach (var column in DataGrid.Columns)
            {
                if (column.Tag.ToString() != e.Column.Tag.ToString())
                {
                    column.SortDirection = null;
                }
            }
        }

        private void DataGrid_LoadingRowGroup(object sender, ctWinUI.DataGridRowGroupHeaderEventArgs e)
        {
            ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            DataGridDataItem item = group.GroupItems[0] as DataGridDataItem;
            if (_grouping == "Parent_Mountain")
            {
                e.RowGroupHeader.PropertyValue = item.Parent_mountain;
            }
            else
            {
                e.RowGroupHeader.PropertyValue = item.Range;
            }
        }

        private void FilterRankLow_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ItemsSource = _dataSource.FilterData(DataGridDataSource.FilterOptions.Rank_Low);
        }

        private void FilterRankHigh_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ItemsSource = _dataSource.FilterData(DataGridDataSource.FilterOptions.Rank_High);
        }

        private void FilterHeightLow_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ItemsSource = _dataSource.FilterData(DataGridDataSource.FilterOptions.Height_Low);
        }

        private void FilterHeightHigh_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ItemsSource = _dataSource.FilterData(DataGridDataSource.FilterOptions.Height_High);
        }

        private void FilterClear_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ItemsSource = _dataSource.FilterData(DataGridDataSource.FilterOptions.All);
        }

        private void ApplyGrouping(string grouping)
        {
            _grouping = grouping;
            DataGrid.RowGroupHeaderPropertyNameAlternative = _grouping;
            DataGrid.ItemsSource = _dataSource.GroupData(_grouping).View;
        }

        private void GroupByParentMountain_Click(object sender, RoutedEventArgs e)
        {
            ApplyGrouping("Parent_Mountain");
        }

        private void GroupByRange_Click(object sender, RoutedEventArgs e)
        {
            ApplyGrouping("Range");
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            DataGrid.ItemsSource = _dataSource.SearchData(args.QueryText);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ItemsSource = _dataSource.SearchData(SearchBox.Text);
        }
    }
}
