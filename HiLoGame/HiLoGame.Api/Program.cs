using FluentValidation;
using HiLoGame.Api;
using HiLoGame.Api.Validators;
using HiLoGame.Application.Interfaces;
using HiLoGame.Application.UseCases;
using HiLoGame.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
builder.Services.AddScoped<CreateGameUseCase>();
builder.Services.AddScoped<JoinGameUseCase>();
builder.Services.AddScoped<MakeGuessUseCase>();
builder.Services.AddScoped<GuessHandler>();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(IValidator));
builder.Services.AddScoped<IValidator<CreateGameRequest>, CreateGameRequestValidator>();
builder.Services.AddScoped<IValidator<GuessRequest>, GuessRequestValidator>();
builder.Services.AddScoped<IValidator<JoinGameRequest>, JoinGameRequestValidator>();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", config =>
    {
        config.Window = TimeSpan.FromSeconds(10);
        config.PermitLimit = 5;
        config.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 2;
    });
});

var app = builder.Build();

app.UseRateLimiter();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapOpenApi();

app.MapPost("/games", async ([FromServices]IValidator<CreateGameRequest> validator,[FromServices] CreateGameUseCase useCase, [FromBody] CreateGameRequest req) =>
{
    var validationResult = await validator.ValidateAsync(req);
    if (!validationResult.IsValid) 
        return Results.ValidationProblem(validationResult.ToDictionary());
    
    var game = await useCase.ExecuteAsync(req.Min, req.Max);
    return Results.Ok(new { game.Id, game.Min, game.Max });
}).WithName("games");

app.MapPost("/games/{gameId:guid}/join",
    async ([FromServices]IValidator<JoinGameRequest> validator,[FromServices] JoinGameUseCase useCase, Guid gameId, [FromBody] JoinGameRequest req) =>
    {
        var validationResult = await validator.ValidateAsync(req);
        if (!validationResult.IsValid) 
            return Results.ValidationProblem(validationResult.ToDictionary());
        
        var player = await useCase.ExecuteAsync(gameId, req.PlayerName);
        return Results.Ok(new { player.Id, player.Name });
    }).WithName("games-join");

app.MapPost("/games/{gameId:guid}/guess",
        async ([FromServices]IValidator<GuessRequest> validator,[FromServices] GuessHandler handler, Guid gameId, [FromBody] GuessRequest req) =>
        {
            var validationResult = await validator.ValidateAsync(req);
            if (!validationResult.IsValid) 
                return Results.ValidationProblem(validationResult.ToDictionary());
            
            try
            {
                var result = await handler.ExecuteAsync(gameId, req.PlayerId, req.Guess);
                return Results.Ok(new { Result = result.ToString() });
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        })
    .RequireRateLimiting("fixed")
    .WithName("games-guess");

app.MapHub<GameHub>("/hub/game");

app.Run();

public partial class Program
{
}