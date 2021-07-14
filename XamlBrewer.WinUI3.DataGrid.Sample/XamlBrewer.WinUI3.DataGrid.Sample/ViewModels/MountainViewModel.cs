using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.SampleApp.Data;
using System;
using System.Collections.Generic;
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
                return dbContext.Mountains.Select(i => i).ToList();
            }
        }

        public IEnumerable<Mountain> SortedMountains(string sortBy, bool ascending)
        {
            using (var dbContext = new MountainDbContext())
            {
                return dbContext.Mountains.Select(i => i).OrderBy(sortBy, !ascending).ToList();
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
