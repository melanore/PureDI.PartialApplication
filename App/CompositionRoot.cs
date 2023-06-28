namespace App;

using Microsoft.FSharp.Core;
using Pure.DI;

static partial class Composition
{
    // Actually, this code never runs and the method might have any name or be a constructor for instance
    // because this is just a hint to set up an object graph.
    private static void Setup() => DI.Setup("CompositionRoot")
        .DefaultLifetime(Lifetime.Singleton)
        .Bind<FSharpFunc<Unit, DbConnection>>("Domain.Db.openConnection")
        .To<FSharpFunc<Unit, DbConnection>>(_ =>
        {
            var handler = (Unit _) => Db.openConnection();
            return handler.ToFSharpFunc();
        })
        
        .Bind<FSharpFunc<int, Person>>("Domain.PersonStorage.getPerson")
        .To<FSharpFunc<int, Person>>(ctx =>
        {
            ctx.Inject<FSharpFunc<Unit, DbConnection>>("Domain.Db.openConnection", out var openConnection);
            var handler = (int userId) => PersonStorage.getPerson(openConnection, userId);
            return handler.ToFSharpFunc();
        })
        
        .Bind<FSharpFunc<int, Person>>("Domain.PersonService.getPerson")
        .To<FSharpFunc<int, Person>>(ctx =>
        {
            ctx.Inject<FSharpFunc<int, Person>>("Domain.PersonStorage.getPerson", out var getPerson);
            var handler = (int userId) => PersonService.getPerson(getPerson, userId);
            return handler.ToFSharpFunc();
        })
        
        .Bind<FSharpFunc<int, Book[]>>("Domain.BookStorage.getBooksByPersonId")
        .To<FSharpFunc<int, Book[]>>(ctx =>
        {
            ctx.Inject<FSharpFunc<Unit, DbConnection>>("Domain.Db.openConnection", out var openConnection);
            var handler = (int userId) => BookStorage.getBooksByPersonId(openConnection, userId);
            return handler.ToFSharpFunc();
        })
        
        .Bind<FSharpFunc<int, Book[]>>("Domain.BookService.getBooksByPersonId")
        .To<FSharpFunc<int, Book[]>>(ctx =>
        {
            ctx.Inject<FSharpFunc<int, Book[]>>("Domain.BookStorage.getBooksByPersonId", out var getBooksByPersonId);
            var handler = (int userId) => BookService.getBooksByPersonId(getBooksByPersonId, userId);
            return handler.ToFSharpFunc();
        })
        
        .Bind<FSharpFunc<int, AuthorWithBooks>>("Domain.AuthorApi.getAuthorWithBooksById")
        .To<FSharpFunc<int, AuthorWithBooks>>(ctx =>
        {
            ctx.Inject<FSharpFunc<int, Person>>("Domain.PersonService.getPerson", out var getPerson);
            ctx.Inject<FSharpFunc<int, Book[]>>("Domain.BookService.getBooksByPersonId", out var getBooksByPersonId);
            var handler = (int userId) => AuthorApi.getAuthorWithBooksById(getPerson, getBooksByPersonId, userId);
            return handler.ToFSharpFunc();
        })
    
        .Root<FSharpFunc<int, AuthorWithBooks>>("getAuthorWithBooksById", "Domain.AuthorApi.getAuthorWithBooksById");
}