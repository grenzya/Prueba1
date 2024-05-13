using ebooks_dotnet7_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("ebooks"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var ebooks = app.MapGroup("api/ebook");

ebooks.MapPost("/", CreateEBook);
ebooks.MapPut("/{id}", UpdateBook);

ebooks.MapPut("/{id}/increment-stock", IncrementStock);


ebooks.MapDelete("/{id}", DeleteEBook);

app.Run();

Task<IResult> CreateEBook(DataContext context, [FromBody] EBook ebook)
{
    var existEBookOne = context.EBooks.Where(e => e.Title == e.Title).FirstOrDefault();
    var existEBookTwo = context.EBooks.Where(e => e.Author == e.Author).FirstOrDefault();
    if (existEBookOne != null || existEBookTwo != null)
    {
        return Task.FromResult<IResult>(TypedResults.BadRequest("Ya existe este libro"));
    }

    context.EBooks.Add(ebook);
    context.SaveChanges();
    return Task.FromResult<IResult>(TypedResults.CreatedAtRoute());
}



Task<IResult> UpdateBook(DataContext context, [FromBody] EBook ebook)
{
    var existEBookOne = context.EBooks.Where(e => e.Id == e.Id).FirstOrDefault();
    if (existEBookOne == null)
    {
        return Task.FromResult<IResult>(TypedResults.NotFound("No existe este libro"));
    }

    existEBookOne.Title = ebook.Title;
    existEBookOne.Author = ebook.Author;
    existEBookOne.Genre = ebook.Genre;
    existEBookOne.Format = ebook.Format;
    existEBookOne.Price = ebook.Price;

    context.SaveChanges();
    return Task.FromResult<IResult>(TypedResults.Ok());
}

Task<IResult> IncrementStock(DataContext context, [FromBody] EBook ebook)
{
    var existEBookOne = context.EBooks.Where(e => e.Id == e.Id).FirstOrDefault();
    if (existEBookOne == null)
    {
        return Task.FromResult<IResult>(TypedResults.BadRequest("No existe este libro"));
    }

    if (existEBookOne.Stock <= ebook.Stock)
    {
        return Task.FromResult<IResult>(TypedResults.BadRequest("Tiene que ser mayor que 0"));
    }


    context.SaveChanges();
    return Task.FromResult<IResult>(TypedResults.Ok());
}

Task<IResult> DeleteEBook(DataContext context, int id)
{
    var existEBook = context.EBooks.Where(e => e.Id == id).FirstOrDefault();

    if (existEBook == null)
    {
        return Task.FromResult<IResult>(TypedResults.NotFound("El EBook no existe"));
    }

    context.EBooks.Remove(existEBook);
    context.SaveChanges();

    return Task.FromResult(Results.NoContent());
}
