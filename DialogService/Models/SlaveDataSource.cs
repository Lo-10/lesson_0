using Npgsql;

namespace Dialogs.Models
{
    public class ReadDataSource
    {
        public NpgsqlDataSource DataSource { get; set; }
        public ReadDataSource(NpgsqlDataSource dataSource)
        {
            DataSource = dataSource;
        }
    }
}