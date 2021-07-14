using Microsoft.Toolkit.Uwp.SampleApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
