using Npgsql;

namespace Counters.Models
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