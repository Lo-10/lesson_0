using Npgsql;

namespace Counters.Models
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