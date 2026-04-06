using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security.Claims;
using ToDoListApi.Data;
using ToDoListApi.Features.ToDoItems.CreateToDoItem;
using ToDoListApi.Features.ToDoItems.DeleteToDoItem;
using ToDoListApi.Features.ToDoItems.GetToDoItemById;
using ToDoListApi.Features.ToDoItems.GetToDoItems;
using ToDoListApi.Features.ToDoItems.UpdateToDoItem;
using ToDoListApi.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJS", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",   // Next.js dev server
            "https://localhost:3000")  // на всякий случай если используете HTTPS
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();  // если используете куки/авторизацию
    });
});


//Аутентификация и Авторизация
builder.Services.AddAuthentication("Cookies")
        .AddCookie("Cookies", options =>
        {
            // Настройки cookie находятся здесь, внутри options.Cookie
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // или CookieSecurePolicy.Always
            options.Cookie.IsEssential = true;

            // Другие настройки
            //options.Cookie.Domain = "localhost";  // ← явно указываем домен
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;

            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401; // не аутентифицирован
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403; // нет прав
                return Task.CompletedTask;
            };
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        policy => policy.RequireRole("Admin"));
});

//// Регистрация DbContext (SQLite)
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlite("Data Source=app.db"));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Репозитории(Data Layer)
//builder.Services.AddScoped<ITaskRepository, TaskRepository>();


// JSON (Data Layer)
builder.Services.AddSingleton<ITaskRepository, JsonTaskRepository>();

// Регистрация Handler'ов (обработчики)
builder.Services.AddScoped<CreateToDoItemHandler>();
builder.Services.AddScoped<GetToDoItemsHandler>();
builder.Services.AddScoped<GetToDoItemByIdHandler>();
builder.Services.AddScoped<DeleteToDoItemHandler>();
builder.Services.AddScoped<UpdateToDoItemHandler>();

var app = builder.Build();

// Swagger только в разработке
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//middleware - автоматически перенаправляет все HTTP-запросы на HTTPS.
//app.UseHttpsRedirection();
app.UseCors("AllowNextJS");

app.UseAuthentication();  // кто ты?
app.UseAuthorization();   // что тебе можно?

// Группа для всех эндпоинтов
var api = app.MapGroup("/api")
             .WithTags("API").RequireAuthorization("AdminOnly");

// Редирект с корня на Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Здесь добавляй свои эндпоинты:
// login(вход)
app.MapPost("/login", async (HttpContext context, string name, string password) =>
{
    Console.WriteLine($"Запрос к /login: {name}, {password}");
    var users = new List<User>()
    {
        new User{Id=1,Name="Alex",Password="123",Role="Admin"},
        new User{Id=2,Name="Bob",Password="333",Role="User"},
        new User{Id=3,Name="Tom",Password="222",Role="User"},
        new User{Id=4,Name="Joe",Password="111",Role="User"}

    };

    var user = users.FirstOrDefault(u => u.Name == name);



    if (user == null || user.Password != password)
    {
        return Results.Unauthorized();
    }
       
    // 1. Данные пользователя (имя, роль и др.)
    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };
    // 2. Паспорт пользователя
    var identity = new ClaimsIdentity(claims, "Cookies");
    // 3. Пользователь с паспортом
    var principal = new ClaimsPrincipal(identity);
    // 4. Выдаём куку
    await context.SignInAsync("Cookies", principal);

    return Results.Ok($"Авторизован {user.Name} с ролью: {user.Role}");
    

    
});

// logout(выход)
app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync("Cookies");
    return Results.Ok("Вышел из системы");
});

// Проверка состояния App 
api.MapGet("/health", () => Results.Ok("Healthy"))
    .WithName("HealthCheck")
    .WithSummary("Проверка состояния приложения")
    .WithDescription("Возвращает 'Healthy' при нормальной работе API");

//Create ToDo Item
api.MapPost("/todos", async (CreateToDoItemRequest request, CreateToDoItemHandler handler) =>
{
    if (string.IsNullOrWhiteSpace(request.Title)) return Results.BadRequest("Title обязателен");

    var command = new CreateToDoItemCommand(
        request.Title,
        request.Description,
        DateTime.UtcNow,
        false);

    return await handler.Handle(command);
})
    .WithName("CreateToDoItem")
    .WithSummary("Создаёт задачу")
    .WithDescription("Создаёт задачу. Title обязателен. Возвращает 201 Created с Location новой задачи.");

//GetAll List ToDo
api.MapGet("/todos", async (GetToDoItemsHandler handler) =>
    await handler.Handle())
    .WithName("GetAllToDoItems")
    .WithSummary("Получить все задачи")
    .WithDescription("Возвращает список всех задач. Если задач нет — 404.");

//Get todo item by id
api.MapGet("/todos/{id:int}", async (int id, GetToDoItemByIdHandler handler) =>
{
    if(id <= 0)
        return Results.BadRequest("Id должно быть больше 0");

    var request = new GetToDoItemByIdRequest(id);

    return await handler.Handle(request);
})
    .WithName("GetToDoItemById")
    .WithSummary("Получить задачу по ID")
    .WithDescription("Возвращает задачу по указанному ID. Если задачи нет — 404.");

//Delete todo by id
api.MapDelete("/todos/{id:int}", async (int id, DeleteToDoItemHandler handler) =>
{
    if (id <= 0)
        return Results.BadRequest("Id должно быть больше 0");

    // Id из пути маршрута передаём в объект запроса и вызываем обработчик удаления
    var request = new DeleteToDoItemRequest(id);

    return await handler.Handle(request);
})
.WithName("DeleteToDoItem")
.WithSummary("Удалить задачу по ID")
.WithDescription("Удаляет задачу по указанному ID. Если задачи нет — 404.");

//Update todo by id
api.MapPut("/todos/{id:int}", async (int id, UpdateToDoItemRequest request, UpdateToDoItemHandler handler) =>
{
    // Важно: Id из маршрута должен совпадать с Id в теле запроса
    if (id != request.Id)
        return Results.BadRequest("ID в маршруте и теле запроса не совпадают");

    if (string.IsNullOrWhiteSpace(request.Title))
        return Results.BadRequest("Title обязателен");

    var command = new UpdateToDoItemCommand
    (
        request.Id,
        request.Title,
        request.Description,
        request.IsCompleted,
        DateTime.UtcNow
    );

    return await handler.Handle(command);
})
.WithName("UpdateToDoItem")
.WithSummary("Обновить задачу по ID")
.WithDescription("Обновляет задачу. ID из пути и тела должны совпадать. Title обязателен.");


app.Urls.Add("http://0.0.0.0:8080");

await app.RunAsync();

class User
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public string Password { get; set; }
    public string Role { get; set; }
}