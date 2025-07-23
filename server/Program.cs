using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using App.Data;
using System.Text.Json.Serialization;
using Ai.Service;

var builder = WebApplication.CreateBuilder(args);
//取得appsettings裡面的環境變數
var frontendUrl = builder.Configuration["FrontendUrl"];



// 註冊 Controller 服務
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 把 enum 顯示為字串，而不是數字
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
//cors設定
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendClient", policy =>
        policy.WithOrigins(frontendUrl ?? "http://localhost:5173") // 允許的網站
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // 可選
});
//註冊資料庫
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 啟用 Swagger 產生 OpenAPI 文件
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PizzaStore API",
        Description = "Making the Pizzas you love",
        Version = "v1"
    });
});
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddHttpClient<IAIService, DeepSeekService>();
builder.Services.AddSingleton<AIServiceFactory>();


var app = builder.Build();

// 測試資料庫連線
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        bool canConnect = db.Database.CanConnect();
        Console.WriteLine($"Database connection success? {canConnect}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection failed: {ex.Message}");
    }
}


if (app.Environment.IsDevelopment())
{
    // 啟用 Swagger 中介軟體與 UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
    });
}

// 使用順序要正確：Routing → Cors → Authorization → Controller
app.UseRouting();
app.UseCors("FrontendClient");
app.UseAuthentication();
app.UseAuthorization();
// app.UseHttpsRedirection(); // 可以先註解

// 把 Controller 路由映射起來
// 會掃描你專案中所有加了 [ApiController] 和 [Route(...)] 的 
// controller 類別，自動把它們的路由對應起來。
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
