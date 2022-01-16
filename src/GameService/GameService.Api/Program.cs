var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddCustomActors();
builder.AddCustomUserIdProvider();

builder.Services.AddDaprClient();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
app.MapActorsHandlers();
app.MapControllers();
app.MapHubs();

app.Run();