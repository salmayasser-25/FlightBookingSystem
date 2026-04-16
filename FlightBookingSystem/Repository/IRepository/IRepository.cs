using System.Linq.Expressions;

namespace FlightBookingSystem.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<List<T>> GetAsync(Expression<Func<T, bool>>? expression = null,
            bool tracked = false, Expression<Func<T, object>>[]? includes = null);

        Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, bool tracked = false, Expression<Func<T, object>>[]? includes = null);

        Task<int> CommitAsync();
    }

}
