using ToDoListApi.Features.ToDoItems;

namespace ToDoListApi.Repositories
{
    public interface ITaskRepository
    {
        Task<ToDoItem?> GetByIdAsync(int id);
        Task<List<ToDoItem>> GetAllAsync();
        void Add(ToDoItem entity);
        void Update(ToDoItem entity);
        void Remove(ToDoItem entity);
        Task<int> SaveChangesAsync();
    }
}
