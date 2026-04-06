namespace ToDoListApi.Features.ToDoItems.UpdateToDoItem
{

    public record UpdateToDoItemResponse(
        int Id, 
        string Title,
        string Description,
        bool IsCompleted,
        DateTime? CreatedAt,
        DateTime? UpdatedAt
        );


}
