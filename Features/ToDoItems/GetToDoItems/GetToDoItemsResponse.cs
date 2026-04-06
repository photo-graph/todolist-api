namespace ToDoListApi.Features.ToDoItems.GetToDoItems
{
    public record GetToDoItemsResponse(int Id, string Title, string Description,bool IsCompleted, DateTime CreatedAt, DateTime? UpdatedAt);
}
