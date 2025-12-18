using Npgsql;

var connectionString = "Host=localhost;Database=ecommerce;Username=postgres;Password=postgres";

using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

Console.WriteLine("Connected to database successfully!");

// Add MoMo columns
var sql = @"
ALTER TABLE ""Orders"" ADD COLUMN IF NOT EXISTS ""MoMoRequestId"" text;
ALTER TABLE ""Orders"" ADD COLUMN IF NOT EXISTS ""MoMoTransactionId"" text;
ALTER TABLE ""Orders"" ADD COLUMN IF NOT EXISTS ""PaymentDate"" timestamp with time zone;
";

using var command = new NpgsqlCommand(sql, connection);
await command.ExecuteNonQueryAsync();

Console.WriteLine("Successfully added MoMo payment columns to Orders table!");
