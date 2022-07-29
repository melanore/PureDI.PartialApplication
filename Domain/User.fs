module Domain.User

open System
open System.Threading.Tasks
open Microsoft.Extensions.Logging

type ConnectionString = ConnectionString of string
type UserId = UserId of int with member x.Value = match x with UserId userId -> userId

type User = { Id: UserId }

type PipelineContext =
    { UserId : UserId
      // some expensive data to fetch and pass between services
      State : Guid }
    
let connectionString = ConnectionString "test"
    
type GetPipelineContext = GetPipelineContext of Lazy<Task<PipelineContext>>
let getPipelineContext (ConnectionString connectionString)
                       (logger : ILogger<GetPipelineContext>) =
    GetPipelineContext <| lazy task {
        let data =
            { UserId = UserId 5
              State = Guid.NewGuid() }
        logger.LogInformation "getPipelineContext:"
        logger.LogInformation $"    connectionString=%s{connectionString}"
        logger.LogInformation $"    data={{ UserId=%i{data.UserId.Value}; State=%A{data.State} }}"
        return data
    }

type GetUserById = GetUserById of (UserId -> Task<User>)
let getUserById (ConnectionString connectionString)
                (GetPipelineContext getPipelineContext)
                (logger : ILogger<GetUserById>) =
    GetUserById <| fun (UserId userId) -> task {
        let! context = getPipelineContext.Value
        logger.LogInformation "getUserById:"
        logger.LogInformation $"    connectionString=%s{connectionString}; userId=%i{userId}"
        logger.LogInformation $"    userId=%i{userId}"
        logger.LogInformation $"    context={{ UserId=%i{context.UserId.Value}; State=%A{context.State} }}"
        return { Id = UserId userId }
    }

let getUserByIdDecorator (GetUserById getUserById)
                         (logger : ILogger<GetUserById>) =
    GetUserById <| fun userId ->
        logger.LogInformation "getUserByIdDecorator:"
        logger.LogInformation "    DECORATED BEFORE"
        let result = getUserById userId
        logger.LogInformation "    DECORATED AFTER"
        result