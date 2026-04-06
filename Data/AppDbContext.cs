using Microsoft.EntityFrameworkCore;
using ToDoListApi.Features.ToDoItems;

namespace ToDoListApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        //Таблицы БД
        public DbSet<ToDoItem> ToDoItems { get; set; }

    }
}
