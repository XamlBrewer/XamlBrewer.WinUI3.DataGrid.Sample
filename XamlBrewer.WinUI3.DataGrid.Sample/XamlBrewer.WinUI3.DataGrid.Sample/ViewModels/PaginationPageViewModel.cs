using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.SampleApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XamlBrewer.WinUI3.DataGrid.Sample.DataAccessLayer;
using XamlBrewer.WinUI3.DataGrid.Sample.Models;
using XamlBrewer.WinUI3.DataGrid.Sample.Services;

namespace XamlBrewer.WinUI3.DataGrid.Sample.ViewModels
{
    public class PaginationPageViewModel : ObservableObject
    {
        private int _pageSize = 10;
        private int _pageNumber;
        private int _pageCount;
        private List<Mountain> _mountains;

        public PaginationPageViewModel()
        {
            FirstAsyncCommand = new AsyncRelayCommand(
                async () => await GetMountains(1, _pageSize),
                () => _pageNumber != 1
              );
            PreviousAsyncCommand = new AsyncRelayCommand(
                async () => await GetMountains(_pageNumber - 1, _pageSize),
                () => _pageNumber > 1
              );
            NextAsyncCommand = new AsyncRelayCommand(
                async () => await GetMountains(_pageNumber + 1, _pageSize),
                () => _pageNumber < _pageCount
              );
            LastAsyncCommand = new AsyncRelayCommand(
                async () => await GetMountains(_pageCount, _pageSize),
                () => _pageNumber != _pageCount
              );

            Refresh();
        }

        public List<int> PageSizes => new() { 5, 10, 20, 50, 100 };

        public int PageSize
        {
            get => _pageSize;
            set
            {
                SetProperty(ref _pageSize, value);
                Refresh();
            }
        }

        public int PageNumber
        {
            get => _pageNumber;
            private set => SetProperty(ref _pageNumber, value);
        }

        public int PageCount
        {
            get => _pageCount;
            private set => SetProperty(ref _pageCount, value);
        }

        public List<Mountain> Mountains
        {
            get => _mountains;
            private set => SetProperty(ref _mountains, value);
        }

        public IAsyncRelayCommand FirstAsyncCommand { get; }

        public IAsyncRelayCommand PreviousAsyncCommand { get; }

        public IAsyncRelayCommand NextAsyncCommand { get; }

        public IAsyncRelayCommand LastAsyncCommand { get; }

        public async Task InitializeAsync()
        {
            using (MountainDbContext dbContext = new())
            {
                // Ensure database is created
                dbContext.Database.EnsureCreated();

                // Ensure table is populated
                if (!dbContext.Mountains.Any())
                {
                    IEnumerable<DataGridDataItem> items = await new DataGridDataSource().GetDataAsync();
                    dbContext.AddRange(items.Select(i => new Mountain
                    {
                        Rank = i.Rank,
                        Name = i.Mountain,
                        Height = i.Height_m,
                        Range = i.Range,
                        ParentMountain = i.Parent_mountain,
                        Coordinates = i.Coordinates,
                        Prominence = i.Prominence,
                        FirstAscent = i.First_ascent,
                        Ascents = i.Ascents
                    }));

                    dbContext.SaveChanges(); // Raaaaah, do not forget this.
                }
            }
        }

        private async Task GetMountains(int pageIndex, int pageSize)
        {
            using MountainDbContext dbContext = new();
            PaginatedList<Mountain> pagedMountains = await PaginatedList<Mountain>.CreateAsync(
                dbContext.Mountains.OrderBy(m => m.Rank),
                pageIndex,
                pageSize);
            PageNumber = pagedMountains.PageIndex;
            PageCount = pagedMountains.PageCount;
            Mountains = pagedMountains;
            FirstAsyncCommand.NotifyCanExecuteChanged();
            PreviousAsyncCommand.NotifyCanExecuteChanged();
            NextAsyncCommand.NotifyCanExecuteChanged();
            LastAsyncCommand.NotifyCanExecuteChanged();
        }

        private void Refresh()
        {
            _pageNumber = 0;
            FirstAsyncCommand.Execute(null);
        }
    }
}
