using gps_jamming_classifier_be.Data;
using gps_jamming_classifier_be.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue; // Set to a very high value or specific limit
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = null; // Allow unlimited request body size
});

builder.Services.AddScoped<SignalProcessingService>();

// Configure MongoDB
//var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDB"));
//var mongoDatabase = mongoClient.GetDatabase(builder.Configuration["DatabaseName"]);
//builder.Services.AddSingleton(mongoDatabase);


//SQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Jammer")));


// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
    builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = null; // Allow unlimited request body size
    serverOptions.Limits.MaxResponseBufferSize = null; // Allow unlimited response buffer size
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
app.UseCors("AllowAllOrigins");
app.MapControllers();
app.Run();
