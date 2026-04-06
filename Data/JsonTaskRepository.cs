using System.Text.Json;
using ToDoListApi.Features.ToDoItems;
using ToDoListApi.Repositories;

namespace ToDoListApi.Data
{
    public class JsonTaskRepository : ITaskRepository
    {
        private readonly string _filePath = "todos.json";
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private List<ToDoItem> _todos;
        private int _nextId;

        public JsonTaskRepository()
        {
            LoadData().GetAwaiter().GetResult();
        }

        private async Task LoadData()
        {
            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath);
                _todos = JsonSerializer.Deserialize<List<ToDoItem>>(json, _jsonOptions) ?? new List<ToDoItem>();
            }
            else
            {
                _todos = new List<ToDoItem>();
            }

            _nextId = _todos.Any() ? _todos.Max(t => t.Id) + 1 : 1;
        }

        private async Task SaveData()
        {
            var json = JsonSerializer.Serialize(_todos, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public void Add(ToDoItem entity)
        {
            entity.Id = _nextId++;
            _todos.Add(entity);
        }

        public async Task<List<ToDoItem>> GetAllAsync()
        {
            return await Task.FromResult(_todos.OrderByDescending(x => x.CreatedAt).ToList());
        }

        public async Task<ToDoItem?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_todos.FirstOrDefault(t => t.Id == id));
        }

        public void Remove(ToDoItem entity)
        {
            _todos.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            await SaveData();
            return 1;
        }

        public void Update(ToDoItem entity)
        {
            var existing = _todos.FirstOrDefault(t => t.Id == entity.Id);
            if (existing != null)
            {
                existing.Title = entity.Title;
                existing.Description = entity.Description;
                existing.IsCompleted = entity.IsCompleted;
            }
        }
    }
}
