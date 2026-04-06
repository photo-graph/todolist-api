using ToDoListApi.Repositories;

namespace ToDoListApi.Features.ToDoItems.UpdateToDoItem
{
    public class UpdateToDoItemHandler
    {
        private readonly ITaskRepository _repository;
        private readonly ILogger<UpdateToDoItemHandler> _logger;

        public UpdateToDoItemHandler(ITaskRepository repository, ILogger<UpdateToDoItemHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IResult> Handle(UpdateToDoItemCommand command)
        {
            var todo = await _repository.GetByIdAsync(command.Id);

            if (todo == null)
                return Results.NotFound();

            todo.Title = command.Title;
            todo.Description = command.Description;
            todo.IsCompleted = command.IsCompleted;
            todo.UpdatedAt = command.UpdatedAt;

            await _repository.SaveChangesAsync();

            _logger.LogInformation("Обновлена задача {Id} с заголовком '{Title}'", todo.Id, todo.Title);

            var response = new UpdateToDoItemResponse(
                todo.Id,
                todo.Title,
                todo.Description,
                todo.IsCompleted,
                todo.CreatedAt,
                todo.UpdatedAt
                );

            return Results.Ok(response);
        }

    }
}
