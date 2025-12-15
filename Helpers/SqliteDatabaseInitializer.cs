using Microsoft.Data.Sqlite;

namespace OwaspTop10Demo.Helpers
{
    public static class SqliteDatabaseInitializer
    {
        private const string Conn = "Data Source=SqlInjectionDemo.db";

        public static string GetConnectionString() => Conn;

        public static void Initialize()
        {
            using var conn = new SqliteConnection(Conn);
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users(
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserName TEXT NOT NULL,
                        Password TEXT NOT NULL
                    );";
                cmd.ExecuteNonQuery();
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Users;";
                long count = (long)(cmd.ExecuteScalar() ?? 0);

                if (count == 0)
                {
                    cmd.CommandText = @"
                        INSERT INTO Users(UserName, Password)
                        VALUES
                        ('alice', 'Password123'),
                        ('bob', 'Secret456'),
                        ('admin', 'AdminPass!');
                    ";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}