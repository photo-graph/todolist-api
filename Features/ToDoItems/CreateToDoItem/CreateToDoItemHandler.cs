using ToDoListApi.Repositories;

namespace ToDoListApi.Features.ToDoItems.CreateToDoItem
{
    public class CreateToDoItemHandler
    {

        private readonly ITaskRepository _repository;
        private readonly ILogger<CreateToDoItemHandler> _logger;

        public CreateToDoItemHandler(ITaskRepository repository, ILogger<CreateToDoItemHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IResult> Handle(CreateToDoItemCommand command)
        {
            var todo = new ToDoItem
            {
                Title = command.Title,
                Description = command.Description,
                CreatedAt = command.CreatedAt,
                IsCompleted = command.IsCompleted
            };

            _repository.Add(todo);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Создана задача {Id} с заголовком '{Title}'", todo.Id,todo.Title);

            var response = new CreateToDoItemResponse(
                    todo.Id,
                    todo.Title,
                    todo.Description,
                    todo.CreatedAt,
                    todo.IsCompleted
                );

            return Results.Created($"/api/todos/{todo.Id}",response);
        } 

    }
}
