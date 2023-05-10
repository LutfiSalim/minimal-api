using Microsoft.AspNetCore.Authentication.JwtBearer;
using minimalapi;
using minimalapi.Models;
using minimalapi.Models.Dto;

var configReader = new ConfigReader("configuration.json");
var (url, key) = await configReader.ReadAsync();

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
    var options = new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = true
    };

    var supabase = new Supabase.Client(url, key, options);
    await supabase.InitializeAsync();

    var user = new users
    {
        name = request.Name!,
        email = request.Email!,
        password = request.Password!
    };

    var response = await supabase.From<users>().Insert(user);

    if (response.Models != null && response.Models.Count > 0)
    {
        var found = response.Models[0];

        return Results.Ok(new
        {
            id = found.user_id,
            name = found.name,
            email = found.email,
            password = found.password
        });
    }

    return Results.BadRequest(new { error = "Data inserted is not correct" });

}).WithTags("CRUD").WithOpenApi();

app.MapPut("/identity/users/{id}", async (Guid id, UserDto request) =>
{
    var options = new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = true
    };

    var supabase = new Supabase.Client(url, key, options);
    await supabase.InitializeAsync();

    var user = new users
    {
        user_id = id,
        name = request.Name!,
        email = request.Email!,
        password = request.Password!
    };

    var response = await supabase.From<users>().Update(user);

    if (response.Models != null && response.Models.Count > 0)
    {
        var found = response.Models[0];

        return Results.Ok(new
        {
            id = found.user_id,
            name = found.name,
            email = found.email,
            password = found.password
        });
    }

    return Results.BadRequest(new { error = "UserID is not correct" });

}).WithTags("CRUD").WithOpenApi();


app.MapGet("/identity/users/{id}", async (Guid id) =>
{
    var options = new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = true
    };

    var supabase = new Supabase.Client(url, key, options);
    await supabase.InitializeAsync();

    var result = await supabase.From<users>().Where(x => x.user_id == id).Select("*").Get();
    if (result.Models != null && result.Models.Count > 0)
    {
        var found = result.Models[0];

        return Results.Ok(new
        {
            id = found.user_id,
            name = found.name,
            email = found.email,
            password = found.password
        });
    }

    return Results.BadRequest(new { error = "Data requested not found" });

}).WithTags("CRUD").WithOpenApi();

app.MapGet("/identity/users", async () =>
{
    var options = new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = true
    };

    var supabase = new Supabase.Client(url, key, options);
    await supabase.InitializeAsync();

    var result = await supabase.From<users>().Select("*").Get();

    if (result.Models != null && result.Models.Count > 0)
    {
        var users = new List<object>();
        foreach (var row in result.Models)
        {
            users.Add(new
            {
                id = row.user_id,
                name = row.name,
                email = row.email,
                password = row.password
            });
        }
        return Results.Ok(users);
    }
    else
    {
        return Results.Ok(new List<object>());
    }
});

app.MapDelete("/identity/users/{id}", async (Guid id) =>
{
    var options = new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = true
    };

    var supabase = new Supabase.Client(url, key, options);
    await supabase.InitializeAsync();

    await supabase.From<users>().Where(x => x.user_id == id).Delete();

    return Results.Ok(new { collum = "User deleted" });

}).WithTags("CRUD").WithOpenApi();

app.Run();