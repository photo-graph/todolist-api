using Microsoft.EntityFrameworkCore;
using ToDoListApi.Features.ToDoItems;
using ToDoListApi.Repositories;

namespace ToDoListApi.Data
{
    public class TaskRepository : ITaskRepository
    {

        private readonly AppDbContext _db;

        public TaskRepository(AppDbContext db) => _db = db;

        public void Add(ToDoItem entity)
        {
            _db.ToDoItems.Add(entity);
        }

        public async Task<List<ToDoItem>> GetAllAsync()
        {
            return await _db.ToDoItems
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<ToDoItem?> GetByIdAsync(int id)
        {
            return await _db.ToDoItems.FindAsync(id);
        }

        public void Remove(ToDoItem entity)
        {
            _db.ToDoItems.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public void Update(ToDoItem entity)
        {
            _db.ToDoItems.Update(entity);
        }
    }
}
