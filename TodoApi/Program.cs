using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Add DI - AddService

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

//Configure pipeline -UseMethod

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/todoitems/{id}", async (Guid id, TodoDb db) =>
    await db.Todos.FindAsync(id));

app.MapPost("/todoitems", async(TodoItem todo, TodoDb db) =>
{
    todo.Id = Guid.NewGuid();
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"todoitems/{todo.Id}",todo);
});

app.MapPut("/todoitems/{id}", async (Guid id, TodoItem inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo == null) return Results.NotFound();
    todo.Name = inputTodo.Name;
    todo.isComplete = inputTodo.isComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("todoitems/{id}", async (Guid id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is TodoItem todo)
    {
        db.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();

});

app.Run();
 