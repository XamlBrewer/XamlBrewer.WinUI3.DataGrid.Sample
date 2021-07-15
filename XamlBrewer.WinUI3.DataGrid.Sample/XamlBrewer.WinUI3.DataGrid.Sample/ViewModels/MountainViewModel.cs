using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.SampleApp.Data;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XamlBrewer.WinUI3.DataGrid.Sample.DataAccessLayer;
using XamlBrewer.WinUI3.DataGrid.Sample.Models;

namespace XamlBrewer.WinUI3.DataGrid.Sample.ViewModels
{
    public class MountainViewModel
    {
        public IEnumerable<Mountain> AllMountains()
        {
            using (var dbContext = new MountainDbContext())
            {
                return dbContext.Mountains.ToList();
            }
        }

        public IEnumerable<Mountain> SortedMountains(string sortBy, bool ascending)
        {
            using (var dbContext = new MountainDbContext())
            {
                return dbContext.Mountains.OrderBy(sortBy, !ascending).ToList();
            }
        }

        public IEnumerable<Mountain> SearchMountainsByName(string queryText)
        {
            using (var dbContext = new MountainDbContext())
            {
                return (from item in dbContext.Mountains
                        where EF.Functions.Like(item.Name, $"%{queryText}%")
                        select item).ToList();
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

        public IEnumerable<Mountain> FilterData(FilterOptions filterBy)
        {
            using var dbContext = new MountainDbContext();

            switch (filterBy)
            {
                case FilterOptions.All:
                    return dbContext.Mountains.ToList();

                case FilterOptions.Rank_Low:
                    return dbContext.Mountains.Where(m => m.Rank < 50).ToList();

                case FilterOptions.Rank_High:
                    return dbContext.Mountains.Where(m => m.Rank > 50).ToList();

                case FilterOptions.Height_High:
                    return dbContext.Mountains.Where(m => m.Height > 8000).ToList();

                case FilterOptions.Height_Low:
                    return dbContext.Mountains.Where(m => m.Height < 8000).ToList();
            }

            return dbContext.Mountains.ToList();
        }

        public CollectionViewSource GroupData(string groupBy = "Range")
        {
            using var dbContext = new MountainDbContext();

            ObservableCollection<GroupInfoCollection<Mountain>> groups = new();
            var query = from item in dbContext.Mountains.ToList()
                        orderby item.Range
                        group item by item.Range into g
                        select new { GroupName = g.Key, Items = g };
            if (groupBy == "ParentMountain")
            {
                query = from item in dbContext.Mountains.ToList()
                        orderby item.ParentMountain
                        group item by item.ParentMountain into g
                        select new { GroupName = g.Key, Items = g };
            }
            foreach (var g in query)
            {
                GroupInfoCollection<Mountain> info = new();
                info.Key = g.GroupName;
                var mountains = g.Items;
                foreach (var item in g.Items)
                {
                    info.Add(item);
                }

                groups.Add(info);
            }

            var groupedItems = new CollectionViewSource();
            groupedItems.IsSourceGrouped = true;
            groupedItems.Source = groups;

            return groupedItems;
        }

        public class GroupInfoCollection<T> : ObservableCollection<T>
        {
            public object Key { get; set; }

            public new IEnumerator<T> GetEnumerator()
            {
                return (IEnumerator<T>)base.GetEnumerator();
            }
        }

        public async Task InitializeAsync()
        {
            using (var dbContext = new MountainDbContext())
            {
                // Ensure database is created
                dbContext.Database.EnsureCreated();

                // Ensure table is populated
                if (!dbContext.Mountains.Any())
                {
                    var items = await new DataGridDataSource().GetDataAsync();
                    dbContext.AddRange(items.Select(i => new Mountain
                    {
                        Rank = i.Rank,
                        Name = i.Mountain,
                        Height = i.Height_m,
                        Range = i.Range,
                        ParentMountain = i.Parent_mountain
                    }));

                    dbContext.SaveChanges(); // Raaaaah, do not forget this.
                }
            }
        }
    }

    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc)
        {
            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExpression));
            return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}
