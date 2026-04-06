namespace ToDoListApi.Features.ToDoItems.CreateToDoItem
{
    public record CreateToDoItemResponse(int Id, string Title, string Description, DateTime CreatedAt, bool IsCompleted);
}
