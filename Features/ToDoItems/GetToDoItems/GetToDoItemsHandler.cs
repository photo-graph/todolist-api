using ToDoListApi.Repositories;

namespace ToDoListApi.Features.ToDoItems.GetToDoItems
{
    public class GetToDoItemsHandler
    {
        private readonly ITaskRepository _repository;
        private readonly ILogger<GetToDoItemsHandler> _logger;


        public GetToDoItemsHandler(ITaskRepository repository, ILogger<GetToDoItemsHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IResult> Handle()
        {
            var todos = await _repository.GetAllAsync();

            if (!todos.Any())
                return Results.NotFound("Задачи не найдены");
            
            _logger.LogInformation("Возвращено {Count} заметок", todos.Count);

            var response = todos.Select(t => new GetToDoItemsResponse(
                t.Id,
                t.Title,
                t.Description,
                t.IsCompleted,
                t.CreatedAt,
                t.UpdatedAt
                )).ToList();

            return Results.Ok(response);
        }
    }
}
