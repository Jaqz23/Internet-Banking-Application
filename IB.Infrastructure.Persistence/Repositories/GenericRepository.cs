﻿using Microsoft.EntityFrameworkCore;
using IB.Core.Application.Interfaces.Repositories;
using IB.Infrastructure.Persistence.Contexts;

namespace IB.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class
    {
        private readonly ApplicationContext _dbContext;

        public GenericRepository(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<Entity> AddAsync(Entity entity)
        {
            await _dbContext.Set<Entity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(Entity entity, int id)
        {
            var entry = await _dbContext.Set<Entity>().FindAsync(id);
            if (entry != null)
            {
                _dbContext.Entry(entry).CurrentValues.SetValues(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public virtual async Task DeleteAsync(Entity entity)
        {
            _dbContext.Set<Entity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<List<Entity>> GetAllAsync()
        {
            return await _dbContext.Set<Entity>().ToListAsync();
        }

        public virtual async Task<Entity> GetByIdAsync(int id)
        {
            return await _dbContext.Set<Entity>().FindAsync(id);
        }

        public virtual async Task<List<Entity>> GetAllWithIncludeAsync(List<string> properties)
        {
            var query = _dbContext.Set<Entity>().AsQueryable();
            foreach (string property in properties)
            {
                query = query.Include(property);
            }
            return await query.ToListAsync();
        }
    }
}
