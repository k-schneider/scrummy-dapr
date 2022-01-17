var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddCustomActors();
builder.AddCustomApplicationServices();


builder.Services.AddDaprClient();
builder.Services.AddControllers();
builder.Services.AddSignalR().AddJsonProtocol();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudEvents();
app.UseAuthorization();

app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
app.MapActorsHandlers();
app.MapControllers();
app.MapHubs();
app.MapSubscribeHandler();

app.Run();
