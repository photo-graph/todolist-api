namespace ToDoListApi.Features.ToDoItems.UpdateToDoItem
{
    public record UpdateToDoItemCommand(
        int Id, 
        string Title, 
        string Description, 
        bool IsCompleted, 
        DateTime UpdatedAt
        );
}
