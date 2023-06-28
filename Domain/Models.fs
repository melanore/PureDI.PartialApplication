namespace Domain.Models

open System
open System.Runtime.CompilerServices

[<Struct>]
type DbConnection =
    { Guid : Guid }
    interface IDisposable with
        member this.Dispose() = ()

[<Struct>]
type Person =
    { Id : int
      Name : string }
    
[<Struct>]
type Book =
    { Id : int
      AuthorId : int
      Title : string }
    
[<Struct>]
type AuthorWithBooks =
    { Author : Person
      Books : Book [] }
    
[<Extension>]
type public FSharpFuncUtil =
    [<Extension>]
    static member ToFSharpFunc<'a,'b> (func:System.Func<'a,'b>) = func.Invoke