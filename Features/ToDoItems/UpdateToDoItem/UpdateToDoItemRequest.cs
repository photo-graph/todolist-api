namespace ToDoListApi.Features.ToDoItems.UpdateToDoItem
{
    public record UpdateToDoItemRequest(int Id, string Title, string? Description, bool IsCompleted);
}
