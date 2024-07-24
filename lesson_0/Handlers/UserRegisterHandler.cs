namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Accession;
    using lesson_0.Models.Requests;
    using MediatR;
    using Npgsql;
    using System.Data;
    using System.Net.Security;
    using System.Security.Cryptography;

    public partial class UserRegisterHandler : IRequestHandler<UserRegisterRequest, UserRegisterResponse>
    {
        private readonly IMediator _mediator;

        public UserRegisterHandler(ILifetimeScope scope)
        {

        }

        public async Task<UserRegisterResponse> Handle(UserRegisterRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var pgServer = Environment.GetEnvironmentVariables()["pgsql_server"];
                var pgPort = Environment.GetEnvironmentVariables()["pgsql_port"];
                var pgDb = Environment.GetEnvironmentVariables()["pgsql_db"];
                var pgUser = Environment.GetEnvironmentVariables()["pgsql_user"];
                var pgPassord = Environment.GetEnvironmentVariables()["pgsql_password"];
                var connectionString = $"Server={pgServer};Port={pgPort};Username={pgUser};Password={pgPassord};Database={pgDb}";

                await using var dataSource = NpgsqlDataSource.Create(connectionString);

                var dt = ConvertCSVtoDataTable("users.csv");
                var userId = Guid.NewGuid();
                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (i == 208688) break;
                    await using var cmd = dataSource.CreateCommand("SELECT * FROM users");

                    userId = Guid.NewGuid();
                    cmd.CommandText = $"INSERT INTO public.users (UserId, UserName, FirstName, SecondName, BirthDate, Biography, City, Password)" +
                                      $" VALUES (@UserId, @UserName, @FirstName, @SecondName,@BirthDate, @Biography, @City, @Password)";
                    cmd.Parameters.AddWithValue("UserId", userId);
                    cmd.Parameters.AddWithValue("UserName", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("FirstName", row["FirstName"]);
                    cmd.Parameters.AddWithValue("SecondName", row["SecondName"]);
                    cmd.Parameters.AddWithValue("BirthDate", row["BirthDate"]);
                    cmd.Parameters.AddWithValue("Biography", "Biography");
                    cmd.Parameters.AddWithValue("City", row["City"]);
                    cmd.Parameters.AddWithValue("Password", Encryption.HashPassword(request.Password));

                    try
                    {
                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                        i++;
                    }
                    catch (Exception ex)
                    {

                        
                    }
                }
                return new UserRegisterResponse() { UserId = userId.ToString() };
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicate key value violates unique constraint"))
                {
                    return new UserRegisterResponse() { UserId = null };
                }
                else
                {
                    return null;
                }
            }
            finally
            {

            }
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }
}
