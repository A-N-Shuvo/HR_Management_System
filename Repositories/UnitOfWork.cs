using System.Collections;
using HR_Management_System.Data;
using HR_Management_System.Interfaces;
using HR_Management_System.Repositories.Interface;
using HR_Management_System.Repositories.Implementation; // নিশ্চিত করুন এখানে আপনার Implementation আছে
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private Hashtable _repositories;

        // ১. Employee রিপোজিটরির জন্য প্রপার্টি
        public IEmployeeRepository Employee { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            // ২. কাস্টম রিপোজিটরি ইনভোক করা
            // এটি করলে আপনার কাস্টম মেথডগুলো (যেমন: GetEmployeeListForReport) পাওয়া যাবে
            Employee = new EmployeeRepository(_context);
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null) _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task ExecuteRawSqlAsync(string query)
        {
            await _context.Database.ExecuteSqlRawAsync(query);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}