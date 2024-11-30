using Npgsql;

namespace Dialogs.Models
{
    public class WriteDataSource
    {
        public NpgsqlDataSource DataSource { get; set; }
        public WriteDataSource(NpgsqlDataSource dataSource)
        {
            DataSource = dataSource;
        }
    }
}