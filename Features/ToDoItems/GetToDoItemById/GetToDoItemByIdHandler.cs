using ToDoListApi.Repositories;

namespace ToDoListApi.Features.ToDoItems.GetToDoItemById
{
    public class GetToDoItemByIdHandler
    {
        private readonly ITaskRepository _repository;
        private readonly ILogger<GetToDoItemByIdHandler> _logger;
        public GetToDoItemByIdHandler(ITaskRepository repository, ILogger<GetToDoItemByIdHandler> logger)
        {
            _logger = logger;
            _repository = repository;
        }
        public async Task<IResult> Handle(GetToDoItemByIdRequest request)
        {
            var todo = await _repository.GetByIdAsync(request.Id);

            if (todo == null)
                return Results.Problem(detail:"Запись не найдена", statusCode:404);

            _logger.LogInformation("Запись {Id} {Title} найдена", todo.Id, todo.Title);

            var response = new GetToDoItemByIdResponse(
                    todo.Id,
                    todo.Title,
                    todo.Description,
                    todo.IsCompleted,
                    todo.CreatedAt
                );

            return Results.Ok(response);

        }
    }
}