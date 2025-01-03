using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TransactionService.Data;
using TransactionService.RabbitMQ;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Connect Db
        builder.Services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        // Add services to the container.
        builder.Services.AddHostedService<RabbitMqListener>();

        builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
        builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();
        builder.Services.AddControllers().AddNewtonsoftJson();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //PrepDb.PrepPopulation(app);

        //app.UseHttpsRedirection();
        
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}