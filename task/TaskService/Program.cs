using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskService.Data;
using TaskService.DTO;
using TaskService.Models;
using TaskService.Utils;
using TaskService.Utils.Retry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
});

builder.Services.AddHttpClient("NotificationService", client =>
{
    client.BaseAddress = new Uri("http://127.0.0.1:8000");
});

builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["Jwt:Issuer"],
//             ValidAudience = builder.Configuration["Jwt:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(
//                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
//         };
//     });

// builder.Services.AddAuthorization();


var app = builder.Build();

// app.UseAuthentication();
// app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/task/", async(AppDbContext db) => await db.Tasks.ToArrayAsync());
app.MapPost("/api/task/", async (CreateTask createTask, AppDbContext db, IHttpClientFactory httpFactory, JsonSerializerOptions jsonOptions) =>
{
    if (createTask.Title is null)
    {
        return Results.BadRequest("Title is null");
    } 
    var NewTask = new TaskItem { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Title = createTask.Title, Description = createTask.Description, Status = TaskService.Models.TaskStatus.New };
    db.Tasks.Add(NewTask);
    await db.SaveChangesAsync();
    _ = Retry.SendWebHook(httpFactory, jsonOptions, NewTask);
    return Results.Created($"/api/task/{NewTask.Id}", NewTask);
});

app.MapGet("/api/task/{id}", async (Guid id, AppDbContext db) => 
{
   var task = await db.Tasks.FindAsync(id);
   return task is null? Results.NotFound(): Results.Ok(task);
});

app.MapDelete("/api/task/{id}", async (Guid id, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null)
    {
        return Results.NotFound();
    }
    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.Run();
