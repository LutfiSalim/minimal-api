using Microsoft.AspNetCore.Authentication.JwtBearer;
using MinimalApiNpgSql;
using Npgsql;


string connectionString = string.Empty;
using (var configReader = new ConfigReader("configuration.json"))
connectionString = await configReader.ReadAsync();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseHttpsRedirection();
}

app.MapPost("/identity/users", async (UserDto request) =>
{
    using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    var insertCommand = new NpgsqlCommand("INSERT INTO users (name, email, password) VALUES (@name, @email, @password)", connection);
    insertCommand.Parameters.AddWithValue("name", request.Name!);
    insertCommand.Parameters.AddWithValue("email", request.Email!);
    insertCommand.Parameters.AddWithValue("password", request.Password!);

    try
    {
        await insertCommand.ExecuteNonQueryAsync();

        return Results.Ok(new
        {
            id = insertCommand, 
            name = request.Name,
            email = request.Email,
            password = request.Password
        });
    }
    catch (Exception)
    {
        return Results.BadRequest(new { error = "Data inserted is not correct" });
    }
}).WithTags("CRUD").WithOpenApi();

app.MapPut("/identity/users/{id}", async (Guid id, UserDto request) =>
{
    using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    var updateCommand = new NpgsqlCommand("UPDATE users SET name = @name, email = @email, password = @password WHERE user_id = @id", connection);
    updateCommand.Parameters.AddWithValue("name", request.Name);
    updateCommand.Parameters.AddWithValue("email", request.Email);
    updateCommand.Parameters.AddWithValue("password", request.Password);
    updateCommand.Parameters.AddWithValue("id", id);

    try
    {
        var rowsAffected = await updateCommand.ExecuteNonQueryAsync();
        if (rowsAffected > 0)
        {
            return Results.Ok(new
            {
                id,
                name = request.Name,
                email = request.Email,
                password = request.Password
            });
        }
        else
        {
            return Results.BadRequest(new { error = "UserID is not correct" });
        }
    }
    catch (Exception)
    {
        return Results.BadRequest(new { error = "An error occurred during the update operation" });
    }
}).WithTags("CRUD").WithOpenApi();


app.MapGet("/identity/users/{id}", async (Guid id) =>
{
    using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    using var command = new NpgsqlCommand("SELECT * FROM users WHERE user_id = @userId");
    command.Parameters.AddWithValue("@userId", id);
    command.Connection = connection;
    var reader = await command.ExecuteReaderAsync();

    var dataGet = new List<Dictionary<string, object>>();

    while (await reader.ReadAsync())
    {
        var dict = new Dictionary<string, object>();
        dict[reader.GetName(0)] = reader[0];
        dict[reader.GetName(1)] = reader[1];
        dict[reader.GetName(2)] = reader[2];
        dict[reader.GetName(3)] = reader[3]; // depends on how many columns ada dlm table

        dataGet.Add(dict);
    }
    return Results.Ok(dataGet);

}).WithTags("CRUD").WithOpenApi();


app.MapGet("/identity/users", async () =>
{
    using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    var selectCommand = new NpgsqlCommand("SELECT * FROM users", connection);

    using var reader = await selectCommand.ExecuteReaderAsync();
    var dataGet = new List<Dictionary<string, object>>();

    while (await reader.ReadAsync())
    {
        var dict = new Dictionary<string, object>();
        dict[reader.GetName(0)] = reader[0];
        dict[reader.GetName(1)] = reader[1];
        dict[reader.GetName(2)] = reader[2];
        dict[reader.GetName(3)] = reader[3]; // depends on how many columns ada dlm table

        dataGet.Add(dict);
    }

    return Results.Ok(dataGet);
}).WithTags("CRUD").WithOpenApi();


app.MapDelete("/identity/users/{id}", async (Guid id) =>
{
    using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    var deleteCommand = new NpgsqlCommand("DELETE FROM users WHERE user_id = @id", connection);
    deleteCommand.Parameters.AddWithValue("id", id);

    await deleteCommand.ExecuteNonQueryAsync();

    return Results.Ok(new { message = "User deleted" });
}).WithTags("CRUD").WithOpenApi();

app.Run();
