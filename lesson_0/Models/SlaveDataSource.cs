using Npgsql;

namespace lesson_0.Models
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