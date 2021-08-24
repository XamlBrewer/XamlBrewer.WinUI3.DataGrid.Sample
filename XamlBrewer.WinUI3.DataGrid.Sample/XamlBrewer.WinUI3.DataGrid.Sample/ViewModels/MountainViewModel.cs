using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.SampleApp.Data;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using XamlBrewer.WinUI3.DataGrid.Sample.DataAccessLayer;
using XamlBrewer.WinUI3.DataGrid.Sample.Models;

namespace XamlBrewer.WinUI3.DataGrid.Sample.ViewModels
{
    public class MountainViewModel
    {
        public async Task<IEnumerable<Mountain>> AllMountainsAsync()
        {
            using (MountainDbContext dbContext = new())
            {
                return await dbContext.Mountains.OrderBy(m => m.Rank).AsNoTracking().ToListAsync();
            }
        }

        public async Task<IEnumerable<Mountain>> SortedMountainsAsync(string sortBy, bool ascending)
        {
            using (MountainDbContext dbContext = new())
            {
                return await dbContext.Mountains.OrderBy(sortBy, !ascending).AsNoTracking().ToListAsync();
            }
        }

        public async Task<IEnumerable<Mountain>> SearchMountainsByNameAsync(string queryText)
        {
            using (MountainDbContext dbContext = new())
            {
                return await dbContext.Mountains
                            .Where(m => EF.Functions.Like(m.Name, $"%{queryText}%"))
                            .OrderBy(m => m.Rank)
                            .AsNoTracking()
                            .ToListAsync();
            }
        }

        public enum FilterOptions
        {
            All = -1,
            Rank_Low = 0,
            Rank_High = 1,
            Height_Low = 2,
            Height_High = 3
        }

        public async Task<IEnumerable<Mountain>> FilteredMountainsAsync(FilterOptions filterBy)
        {
            using MountainDbContext dbContext = new();

            switch (filterBy)
            {
                case FilterOptions.All:
                    return await dbContext.Mountains.OrderBy(m => m.Rank).AsNoTracking().ToListAsync();

                case FilterOptions.Rank_Low:
                    return await dbContext.Mountains.Where(m => m.Rank < 50).OrderBy(m => m.Rank).AsNoTracking().ToListAsync();

                case FilterOptions.Rank_High:
                    return await dbContext.Mountains.Where(m => m.Rank > 50).OrderBy(m => m.Rank).AsNoTracking().ToListAsync();

                case FilterOptions.Height_High:
                    return await dbContext.Mountains.Where(m => m.Height > 8000).OrderBy(m => m.Rank).AsNoTracking().ToListAsync();

                case FilterOptions.Height_Low:
                    return await dbContext.Mountains.Where(m => m.Height < 8000).OrderBy(m => m.Rank).AsNoTracking().ToListAsync();

                default:
                    break;
            }

            return await dbContext.Mountains.AsNoTracking().ToListAsync();
        }

        public CollectionViewSource GroupedMountains(string groupBy = "Range")
        {
            using MountainDbContext dbContext = new();

            // No ToListAsync() here, since we bail out of Entity Framework to do the Grouping.

            IEnumerable<GroupInfoCollection<string, Mountain>> query = dbContext.Mountains
                            .OrderBy(m => m.Range)
                            .ThenBy(m => m.Rank)
                            .AsNoTracking()
                            .ToList()
                            .GroupBy(m => m.Range, (key, list) => new GroupInfoCollection<string, Mountain>(key, list));
            if (groupBy == "ParentMountain")
            {
                query = dbContext.Mountains
                            .OrderBy(m => m.ParentMountain)
                            .ThenBy(m => m.Rank)
                            .AsNoTracking()
                            .ToList()
                            .GroupBy(m => m.ParentMountain, (key, list) => new GroupInfoCollection<string, Mountain>(key, list));
            }

            CollectionViewSource groupedItems = new()
            {
                IsSourceGrouped = true,
                Source = query
            };

            return groupedItems;
        }

        public class GroupInfoCollection<K, T> : IGrouping<K, T>
        {
            private readonly IEnumerable<T> _items;

            public GroupInfoCollection(K key, IEnumerable<T> items)
            {
                Key = key;
                _items = items;
            }

            public K Key { get; }

            public IEnumerator<T> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _items.GetEnumerator();
            }
        }

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

        public async Task ResetAsync()
        {
            using (MountainDbContext dbContext = new())
            {
                // Ensure database is removed
                dbContext.Database.EnsureDeleted();
            }

            await InitializeAsync();
        }
    }

    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(
            this IQueryable<TEntity> source,
            string orderByProperty,
            bool desc)
        {
            string command = desc ? "OrderByDescending" : "OrderBy";
            Type type = typeof(TEntity);
            PropertyInfo property = type.GetProperty(orderByProperty);
            ParameterExpression parameter = Expression.Parameter(
                    type,
                    "p");
            MemberExpression propertyAccess = Expression.MakeMemberAccess(
                    parameter,
                    property);
            LambdaExpression orderByExpression = Expression.Lambda(
                    propertyAccess,
                    parameter);
            MethodCallExpression resultExpression = Expression.Call(
                    typeof(Queryable),
                    command,
                    new Type[] { type, property.PropertyType },
                    source.Expression, Expression.Quote(orderByExpression));
            return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}
