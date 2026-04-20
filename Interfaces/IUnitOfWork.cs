using HR_Management_System.Repositories.Interface;

namespace HR_Management_System.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employee { get; }

        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> CompleteAsync();

        Task ExecuteRawSqlAsync(string query);
    }
}