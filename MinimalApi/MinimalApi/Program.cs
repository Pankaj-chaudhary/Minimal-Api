using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using MinimalApi.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 
builder.Services.AddSingleton<IBookService, BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
// example 1
app.MapGet("/", () => "Hello, World!");

// example 2
app.MapGet("/books", (IBookService bookService) =>
    TypedResults.Ok(bookService.GetBooks()))
.WithName("GetBooks")
.WithOpenApi(x => new OpenApiOperation(x)
{
    Summary = "Get Library Books",
    Description = "Returns information about all the available books from the Amy's library.",
    Tags = new List<OpenApiTag> { new() { Name = "Amy's Library" } }
});

// example 3
app.MapGet("/books/{id}", Results<Ok<Book>, NotFound> (IBookService bookService, int id) =>
        bookService.GetBook(id) is { } book
            ? TypedResults.Ok(book)
            : TypedResults.NotFound()
    )
    .WithName("GetBookById")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get Library Book By Id",
        Description = "Returns information about selected book from the Amy's library.",
        Tags = new List<OpenApiTag> { new() { Name = "Amy's Library" } }
    });


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
