using Npgsql;

namespace lesson_0.Models
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