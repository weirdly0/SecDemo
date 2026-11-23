using System.Data.SqlClient;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 1) Hard-coded credential (fake, demo only)
const string ConnectionString =
    "Server=tcp:demo.database.windows.net;Database=shop;User Id=admin;Password=P@ssw0rd123!;";

app.MapGet("/", () => "Dependabot + CodeQL demo");

// 2) SQL injection: user input concatenated into the query
app.MapGet("/products", (string name) =>
{
    var results = new List<string>();
    using var conn = new SqlConnection(ConnectionString);
    conn.Open();
    var cmd = new SqlCommand(
        "SELECT Name FROM Products WHERE Name = '" + name + "'", conn);
    using var reader = cmd.ExecuteReader();
    while (reader.Read()) results.Add(reader.GetString(0));
    return JsonConvert.SerializeObject(results);
});

// 3) Path traversal: user input used in a file path
app.MapGet("/file", (string fileName) =>
    File.ReadAllText(Path.Combine("data", fileName)));

// 4) Reflected XSS: user input written into HTML
app.MapGet("/hello", (string user) =>
    Results.Content($"<h1>Hello {user}</h1>", "text/html"));

app.Run();
