var AllowSpecificOrigins = "allowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(hc => new HttpClient { BaseAddress = new Uri("https://api-web.nhle.com/v1") });
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5145",
                                             "https://localhost:5145")
                          .SetIsOriginAllowed((host) => true);
                      });
});

var app = builder.Build();

app.UseCors(AllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/schedule-now", async () =>
{
    var proxyClient = new HttpClient();
    var stream = await proxyClient.GetStreamAsync("https://api-web.nhle.com/v1/schedule/now");
    // Proxy the response as JSON
    return Results.Stream(stream, "application/json");
}).RequireCors(AllowSpecificOrigins);

app.Run();
