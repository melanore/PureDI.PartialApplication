module App.Program


open Composition
open Domain.User

module App =

    open Microsoft.Extensions.Logging
    open Microsoft.Extensions.DependencyInjection
        
    let serviceCollection =
        ServiceCollection()
            .AddLogging(fun o -> o.AddConsole() |> ignore)
            .AddCompositionRoot()
            
    let serviceProvider =
        serviceCollection
            .BuildServiceProvider()

    let (GetUserById getUserById) = serviceProvider.GetService<GetUserById>()
    let (GetPipelineContext pipelineContext) = serviceProvider.GetService<GetPipelineContext>()

module Test =
    open Microsoft.Extensions.DependencyInjection
    
    let mockedUserId = UserId 123
    let private mockedGetUserById (UserId testUserId) : GetUserById =
        GetUserById <| fun (UserId passedUserId) -> task {
            printfn $"GetUserById MOCKED: passed {passedUserId}, returns mocked {testUserId}"
            return { Id = UserId testUserId }
        }

    let serviceProvider =
        App.serviceCollection
            .AddTransient<GetUserById>(fun ctx -> mockedGetUserById mockedUserId)
            .BuildServiceProvider()

    let (GetUserById getUserById) = serviceProvider.GetService<GetUserById>()

let pipeline () =
    task {
        let userId = UserId 999
        let! pipelineContext1 = App.pipelineContext.Value
        let! pipelineContext2 = App.pipelineContext.Value

        assert (pipelineContext1.State = pipelineContext2.State)

        let! appUser = App.getUserById userId
        assert(appUser.Id = userId)
        
        let! testUser = Test.getUserById userId
        assert(testUser.Id = Test.mockedUserId)
    }

[<EntryPoint>]
let main _arv =
    pipeline ()
    |> Async.AwaitTask
    |> Async.RunSynchronously
    0