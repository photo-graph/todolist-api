using ToDoListApi.Repositories;

namespace ToDoListApi.Features.ToDoItems.DeleteToDoItem
{
    public class DeleteToDoItemHandler
    {
        private readonly ITaskRepository _repository;
        private readonly ILogger<DeleteToDoItemHandler> _logger;

        public DeleteToDoItemHandler(ITaskRepository repository, ILogger<DeleteToDoItemHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IResult> Handle(DeleteToDoItemRequest request)
        { 
            var todo = await _repository.GetByIdAsync(request.Id);

            if (todo == null)
                return Results.NotFound(new {error = "Задача не найдена" });

            _repository.Remove(todo);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Задача {Id} удалена", todo.Id);

            var response = new DeleteToDoItemResponse(todo.Id, "Заметка удалена", DateTime.UtcNow);

            //return Results.NoContent();
            return Results.Ok(response);
        }

    }
}
