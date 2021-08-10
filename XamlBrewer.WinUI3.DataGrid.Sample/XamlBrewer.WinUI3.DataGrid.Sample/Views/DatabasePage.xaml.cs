using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Navigation;
using XamlBrewer.WinUI3.DataGrid.Sample.Models;
using XamlBrewer.WinUI3.DataGrid.Sample.ViewModels;
using ctWinUI = CommunityToolkit.WinUI.UI.Controls;

namespace XamlBrewer.WinUI3.DataGrid.Sample.Views
{
    public sealed partial class DatabasePage : Page
    {
        private MountainViewModel _viewModel = new MountainViewModel();
        private long _token;

        private string _grouping;

        public DatabasePage()
        {
            this.InitializeComponent();
            this.Loaded += DatabasePage_Loaded;
            this.Unloaded += DatabasePage_Unloaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _token = DataGrid.RegisterPropertyChangedCallback(ctWinUI.DataGrid.ItemsSourceProperty, DataGridItemsSourceChangedCallback);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataGrid.UnregisterPropertyChangedCallback(ctWinUI.DataGrid.ItemsSourceProperty, _token);
            base.OnNavigatedFrom(e);
        }

        private async void DatabasePage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeAsync();
            DataGrid.ItemsSource = _viewModel.AllMountains();
            DataGrid.Columns[0].SortDirection = ctWinUI.DataGridSortDirection.Ascending;
            DataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }

        private void DatabasePage_Unloaded(object sender, RoutedEventArgs e)
        {
            DataGrid.SelectionChanged -= DataGrid_SelectionChanged;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid.RowDetailsVisibilityMode = ctWinUI.DataGridRowDetailsVisibilityMode.Collapsed;
            DetailsButton.IsEnabled = DataGrid.SelectedIndex >= 0;
        }

        private void DataGrid_Sorting(object sender, ctWinUI.DataGridColumnEventArgs e)
        {
            // Add sorting indicator, and sort
            var isAscending = e.Column.SortDirection == null || e.Column.SortDirection == ctWinUI.DataGridSortDirection.Descending;
            DataGrid.ItemsSource = _viewModel.SortedMountains(e.Column.Tag.ToString(), isAscending);
            e.Column.SortDirection = isAscending
                ? ctWinUI.DataGridSortDirection.Ascending
                : ctWinUI.DataGridSortDirection.Descending;
        }

        private void DataGrid_LoadingRowGroup(object sender, ctWinUI.DataGridRowGroupHeaderEventArgs e)
        {
            ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            Mountain item = group.GroupItems[0] as Mountain;
            if (_grouping == "ParentMountain")
            {
                e.RowGroupHeader.PropertyValue = item.ParentMountain;
            }
            else
            {
                e.RowGroupHeader.PropertyValue = item.Range;
            }
        }

        private void FilterRankLow_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            DataGrid.ItemsSource = _viewModel.FilterData(MountainViewModel.FilterOptions.Rank_Low);
        }

        private void FilterRankHigh_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            DataGrid.ItemsSource = _viewModel.FilterData(MountainViewModel.FilterOptions.Rank_High);
        }

        private void FilterHeightLow_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            DataGrid.ItemsSource = _viewModel.FilterData(MountainViewModel.FilterOptions.Height_Low);
        }

        private void FilterHeightHigh_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            DataGrid.ItemsSource = _viewModel.FilterData(MountainViewModel.FilterOptions.Height_High);
        }

        private void FilterClear_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            DataGrid.ItemsSource = _viewModel.FilterData(MountainViewModel.FilterOptions.All);
        }

        private void ApplyGrouping(string grouping)
        {
            SearchBox.Text = string.Empty;
            _grouping = grouping;
            DataGrid.RowGroupHeaderPropertyNameAlternative = _grouping;
            DataGrid.ItemsSource = _viewModel.GroupData(_grouping).View;
        }

        private void GroupByParentMountain_Click(object sender, RoutedEventArgs e)
        {
            ApplyGrouping("ParentMountain");
        }

        private void GroupByRange_Click(object sender, RoutedEventArgs e)
        {
            ApplyGrouping("Range");
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            DataGrid.ItemsSource = _viewModel.SearchMountainsByName(args.QueryText);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ItemsSource = _viewModel.SearchMountainsByName(SearchBox.Text);
        }

        private void DataGridItemsSourceChangedCallback(DependencyObject sender, DependencyProperty dp)
        {
            if (dp == ctWinUI.DataGrid.ItemsSourceProperty)
            {
                foreach (var column in (sender as ctWinUI.DataGrid).Columns)
                {
                    column.SortDirection = null;
                }
            }
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.RowDetailsVisibilityMode = 2 - DataGrid.RowDetailsVisibilityMode;
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.ResetAsync();
            DataGrid.ItemsSource = _viewModel.AllMountains();
        }
    }
}