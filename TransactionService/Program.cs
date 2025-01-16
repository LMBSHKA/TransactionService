using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TransactionService.Data;
using TransactionService.RabbitMQ;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.SetIsOriginAllowed(origin => true)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        //Connect Db
        builder.Services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Transaction API",
                Description = $"Создание и просмотор транзакций ссылка - {new Uri("https://transactionservice-h089.onrender.com")}",
            });
        });

        // Add services to the container.
        builder.Services.AddHostedService<RabbitMqListener>();

        builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
        builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();
        builder.Services.AddControllers().AddNewtonsoftJson();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var basePath = AppContext.BaseDirectory;

            var xmlPath = Path.Combine(basePath, "TransactionService.xml");
            options.IncludeXmlComments(xmlPath);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.yaml", "v1");
            });
        }

        app.UseCors();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}