﻿using Microsoft.UI.Xaml;
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
        private DataGridDisplayMode _displayMode = DataGridDisplayMode.Default;
        private long _token;
        private string _grouping;

        public DatabasePage()
        {
            InitializeComponent();
            Loaded += DatabasePage_Loaded;
            Unloaded += DatabasePage_Unloaded;
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
            _displayMode = DataGridDisplayMode.Default;
            DataGrid.ItemsSource = await _viewModel.AllMountainsAsync();
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

        private async void DataGrid_Sorting(object sender, ctWinUI.DataGridColumnEventArgs e)
        {
            _displayMode = DataGridDisplayMode.UserSorted;

            // Sort, and add sorting indicator
            bool isAscending = e.Column.SortDirection is null or (ctWinUI.DataGridSortDirection?)ctWinUI.DataGridSortDirection.Descending;
            DataGrid.ItemsSource = await _viewModel.SortedMountainsAsync(e.Column.Tag.ToString(), isAscending);
            e.Column.SortDirection = isAscending
                ? ctWinUI.DataGridSortDirection.Ascending
                : ctWinUI.DataGridSortDirection.Descending;
        }

        private void DataGrid_LoadingRowGroup(object sender, ctWinUI.DataGridRowGroupHeaderEventArgs e)
        {
            ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            Mountain item = group.GroupItems[0] as Mountain;
            e.RowGroupHeader.PropertyValue = _grouping == "ParentMountain" ? item.ParentMountain : item.Range;
        }

        private async void FilterRankLow_Click(object sender, RoutedEventArgs e)
        {
            _displayMode = DataGridDisplayMode.Filtered;
            DataGrid.ItemsSource = await _viewModel.FilteredMountainsAsync(MountainViewModel.FilterOptions.Rank_Low);
        }

        private async void FilterRankHigh_Click(object sender, RoutedEventArgs e)
        {
            _displayMode = DataGridDisplayMode.Filtered;
            DataGrid.ItemsSource = await _viewModel.FilteredMountainsAsync(MountainViewModel.FilterOptions.Rank_High);
        }

        private async void FilterHeightLow_Click(object sender, RoutedEventArgs e)
        {
            _displayMode = DataGridDisplayMode.Filtered;
            DataGrid.ItemsSource = await _viewModel.FilteredMountainsAsync(MountainViewModel.FilterOptions.Height_Low);
        }

        private async void FilterHeightHigh_Click(object sender, RoutedEventArgs e)
        {
            _displayMode = DataGridDisplayMode.Filtered;
            DataGrid.ItemsSource = await _viewModel.FilteredMountainsAsync(MountainViewModel.FilterOptions.Height_High);
        }

        private async void FilterClear_Click(object sender, RoutedEventArgs e)
        {
            _displayMode = DataGridDisplayMode.Default;
            DataGrid.ItemsSource = await _viewModel.FilteredMountainsAsync(MountainViewModel.FilterOptions.All);
        }

        private void ApplyGrouping(string grouping)
        {
            _displayMode = DataGridDisplayMode.Grouped;
            _grouping = grouping;
            DataGrid.RowGroupHeaderPropertyNameAlternative = _grouping;
            DataGrid.ItemsSource = _viewModel.GroupedMountains(_grouping).View;
        }

        private void GroupByParentMountain_Click(object sender, RoutedEventArgs e)
        {
            ApplyGrouping("ParentMountain");
        }

        private void GroupByRange_Click(object sender, RoutedEventArgs e)
        {
            ApplyGrouping("Range");
        }

        private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _displayMode = DataGridDisplayMode.Search;
            DataGrid.ItemsSource = await _viewModel.SearchMountainsByNameAsync(args.QueryText);
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _displayMode = DataGridDisplayMode.Search;
            DataGrid.ItemsSource = await _viewModel.SearchMountainsByNameAsync(SearchBox.Text);
        }

        private void DataGridItemsSourceChangedCallback(DependencyObject sender, DependencyProperty dp)
        {
            // Binding could do most of this ...

            // Remove Display Mode Indicators;
            FilterIndicator.Visibility = Visibility.Collapsed;
            GroupIndicator.Visibility = Visibility.Collapsed;
            SearchIndicator.Visibility = Visibility.Collapsed;

            // Remove Sort Indicators.
            if (dp == ctWinUI.DataGrid.ItemsSourceProperty)
            {
                foreach (var column in (sender as ctWinUI.DataGrid).Columns)
                {
                    column.SortDirection = null;
                }
            }

            if (_displayMode == DataGridDisplayMode.Filtered)
            {
                FilterIndicator.Visibility = Visibility.Visible;
            }

            if (_displayMode == DataGridDisplayMode.Grouped)
            {
                GroupIndicator.Visibility = Visibility.Visible;
            }

            if (_displayMode == DataGridDisplayMode.Search)
            {
                SearchIndicator.Visibility = Visibility.Visible;
            }
            else
            {
                SearchBox.Text = string.Empty;
            }
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.RowDetailsVisibilityMode = 2 - DataGrid.RowDetailsVisibilityMode;
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.ResetAsync();
            DataGrid.ItemsSource = await _viewModel.AllMountainsAsync();
        }

        private enum DataGridDisplayMode
        {
            Default,
            UserSorted,
            Filtered,
            Grouped,
            Search
        }
    }
}