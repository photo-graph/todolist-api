namespace ToDoListApi.Features.ToDoItems.GetToDoItemById
{
    public record GetToDoItemByIdResponse(int Id, string Title, string Description, bool IsCompleted, DateTime CreatedAt);
}
