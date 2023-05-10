using DataLayer;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BusinessLayer.Implementations
{
    public class EFRepository<T> : IAsyncDisposable where T : Entity //ограничения на представления из папки DataLayer/Entities
    {
        private protected readonly VkApiContext vkApiContext;

        public EFRepository(VkApiContext vkApiContext)
        {
            this.vkApiContext = vkApiContext;

        }

        public async Task<int> InsertAsync(T entry) // добавление записи в базу данных
        {
            if (entry == null) { throw new ArgumentNullException(nameof(entry)); }
            await vkApiContext.AddAsync(entry);
            return await vkApiContext.SaveChangesAsync(); // возвращает число добавленных записей (0 или 1)
        }

        public async Task<int> UpdateAsync(T entry)
        {
            if (entry == null) { throw new ArgumentNullException(nameof(entry)); }
            vkApiContext.Update(entry);
            return await vkApiContext.SaveChangesAsync(); // возвращает число  записей (0 или 1)
        }

        public async Task<int> DeleteAsync(T entry) // возвращает число удаленных записей
        {
            if (entry == null) { throw new ArgumentNullException(nameof(entry)); }
            vkApiContext.Remove(entry);
            return await vkApiContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? predicate = null) => 
            predicate is null ? await vkApiContext.Set<T>().ToListAsync(): await vkApiContext.Set<T>().Where(predicate).ToListAsync();

        public async ValueTask DisposeAsync()
        {
            await vkApiContext.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
