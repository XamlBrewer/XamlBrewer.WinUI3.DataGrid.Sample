using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Windows.Storage;
using XamlBrewer.WinUI3.DataGrid.Sample.Models;

namespace XamlBrewer.WinUI3.DataGrid.Sample.DataAccessLayer
{
    public class MountainDbContext : DbContext
    {
        public DbSet<Mountain> Mountains { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mountains.db");

            // File.Delete(path); // Reset to factory settings.

            string connectionStringBuilder = new
                SqliteConnectionStringBuilder()
            {
                DataSource = path
            }
             .ToString();

            SqliteConnection connection = new SqliteConnection(connectionStringBuilder);
            optionsBuilder.UseSqlite(connection);
        }
    }
}
