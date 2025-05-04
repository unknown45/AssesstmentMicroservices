using AssesstmentMicroservices.Application.Interfaces;
using AssesstmentMicroservices.Domain.Entities;
using AssesstmentMicroservices.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesstmentMicroservices.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public IGenericRepository<Customer> Customers { get; }
        public IGenericRepository<Order> Orders { get; }
        public IGenericRepository<Product> Products { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Customers = new GenericRepository<Customer>(_context);
            Orders = new GenericRepository<Order>(_context);
            Products = new GenericRepository<Product>(_context);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
