namespace App;

using Microsoft.FSharp.Core;
using Pure.DI;
using static Domain.Models;
using static Domain.Models.FSharpFuncUtil;
using static Domain.PartialApplication;

static partial class CompositionRoot
{
    // Actually, this code never runs and the method might have any name or be a constructor for instance
    // because this is just a hint to set up an object graph.
    private static void Setup() => DI.Setup()
        .Default(Lifetime.Singleton)
        .Bind<FSharpFunc<Unit, DbConnection>>("Domain.Db.openConnection")
        .To<FSharpFunc<Unit, DbConnection>>(_ =>
        {
            var handler = (Unit _) => Db.openConnection();
            return handler.ToFSharpFunc();
        })
        
        .Bind<FSharpFunc<int, Person>>("Domain.PersonStorage.getPerson")
        .To<FSharpFunc<int, Person>>(ctx =>
        {
            var openConnection = ctx.Resolve<FSharpFunc<Unit, DbConnection>>("Domain.Db.openConnection");
            var handler = (int userId) => PersonStorage.getPerson(openConnection, userId);
            return handler.ToFSharpFunc();
        })
        
        .Bind<FSharpFunc<int, Person>>("Domain.PersonService.getPerson")
        .To<FSharpFunc<int, Person>>(ctx =>
        {
            var getPerson = ctx.Resolve<FSharpFunc<int, Person>>("Domain.PersonStorage.getPerson");
            var handler = (int userId) => PersonService.getPerson(getPerson, userId);
            return handler.ToFSharpFunc();
        })
        
        .Bind<FSharpFunc<int, Book[]>>("Domain.BookStorage.getBooksByPersonId")
        .To<FSharpFunc<int, Book[]>>(ctx =>
        {
            var openConnection = ctx.Resolve<FSharpFunc<Unit, DbConnection>>("Domain.Db.openConnection");
            var handler = (int userId) => BookStorage.getBooksByPersonId(openConnection, userId);
            return handler.ToFSharpFunc();
        })
        
        .Bind<FSharpFunc<int, Book[]>>("Domain.BookService.getBooksByPersonId")
        .To<FSharpFunc<int, Book[]>>(ctx =>
        {
            var getBooksByPersonId = ctx.Resolve<FSharpFunc<int, Book[]>>("Domain.BookStorage.getBooksByPersonId");
            var handler = (int userId) => BookService.getBooksByPersonId(getBooksByPersonId, userId);
            return handler.ToFSharpFunc();
        })
        
        .Bind<FSharpFunc<int, AuthorWithBooks>>("Domain.AuthorApi.getAuthorWithBooksById")
        .To<FSharpFunc<int, AuthorWithBooks>>(ctx =>
        {
            var getPerson = ctx.Resolve<FSharpFunc<int, Person>>("Domain.PersonService.getPerson");
            var getBooksByPersonId = ctx.Resolve<FSharpFunc<int, Book[]>>("Domain.BookService.getBooksByPersonId");
            var handler = (int userId) => AuthorApi.getAuthorWithBooksById(getPerson, getBooksByPersonId, userId);
            return handler.ToFSharpFunc();
        });
}