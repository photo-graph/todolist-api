namespace ToDoListApi.Features.ToDoItems.CreateToDoItem
{
    public record CreateToDoItemCommand(string Title, string Description, DateTime CreatedAt, bool IsCompleted);
}
