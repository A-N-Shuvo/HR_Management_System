using HR_Management_System.Models;

namespace HR_Management_System.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        //void Delete(T entity);
        //void Remove(Employee emp);
        //void Remove(Shift shift);
    }
}