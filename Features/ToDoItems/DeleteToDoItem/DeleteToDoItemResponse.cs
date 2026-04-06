namespace ToDoListApi.Features.ToDoItems.DeleteToDoItem
{
    public record DeleteToDoItemResponse(int Id, string Message, DateTime DeletedAt);
}
