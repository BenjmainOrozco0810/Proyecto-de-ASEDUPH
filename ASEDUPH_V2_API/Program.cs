using ASEDUPH_V2_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;


var builder = WebApplication.CreateBuilder(args);

SqlConnection.ClearAllPools();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<AseduphDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConexionASEDUPH"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );

            sqlOptions.CommandTimeout(60);
        }
    ));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();