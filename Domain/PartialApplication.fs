namespace Domain.PartialApplication

open System
open Domain.Models

module Db =

    type OpenConnection = unit -> DbConnection

    let openConnection : OpenConnection =
        fun () ->
            let id = Guid.NewGuid()
            { Guid = id }

module PersonStorage =
    type GetPerson = int -> Person

    let getPerson (openConnection : Db.OpenConnection) : GetPerson =
        fun id ->
            use _connection = openConnection ()
            { Id = id; Name = "John Doe" }


module BookStorage =
    type GetBooksByPersonId = int -> Book []

    let getBooksByPersonId (openConnection : Db.OpenConnection) : GetBooksByPersonId =
        fun personId ->
            use _connection = openConnection ()
            Array.init 10 (fun id ->
                { Id = id
                  AuthorId = personId
                  Title = $"Title {id}" })


module PersonService =
    type GetPerson = int -> Person
    
    let getPerson (getPerson : PersonStorage.GetPerson) =
        fun id ->
            getPerson id

module BookService =
    type GetBooksByPersonId = int -> Book []
    
    let getBooksByPersonId (getBooksByPersonId : GetBooksByPersonId) =
        fun personId ->
            getBooksByPersonId personId

module AuthorApi =
    let getAuthorWithBooksById (getPerson : PersonService.GetPerson,
                                getBooksByPersonId : BookService.GetBooksByPersonId) =
        fun authorId ->
            let author = getPerson authorId
            let books = getBooksByPersonId authorId
            { Author = author; Books = books }