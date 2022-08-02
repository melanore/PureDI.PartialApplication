module Domain.Functions

open System
open Domain.Models

module Db =
    let openConnection () =
        let id = Guid.NewGuid()
        { Guid = id }

module PersonStorage =
    let getPerson id =
        use _connection = Db.openConnection ()
        { Id = id; Name = "John Doe" }
        

module BookStorage =
    let getBooksByPersonId personId =
        use _connection = Db.openConnection ()
        Array.init 10 (fun id ->
            { Id = id
              AuthorId = personId
              Title = $"Title {id}" })


module PersonService =
    let getPerson id =
        PersonStorage.getPerson id

module BookService =
    let getBooksByPersonId personId =
        BookStorage.getBooksByPersonId personId

module AuthorApi =
    let getAuthorWithBooksById authorId =
        let author = PersonService.getPerson authorId
        let books = BookService.getBooksByPersonId authorId
        { Author = author; Books = books }

module Composition =

    let inline invoke authorId =
        AuthorApi.getAuthorWithBooksById authorId