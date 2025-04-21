using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Repository.Data;
using System.Linq.Expressions;

namespace Repository.Repositories;
public class Repository<TModel> : IRepository<TModel> where TModel : BaseModel
{
    private readonly AppDbContext _appDbContext;
    private readonly DbSet<TModel> _dbSet;

    public Repository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
        _dbSet = _appDbContext.Set<TModel>();
    }

    public IQueryable<TModel> GetAll() => _dbSet.AsNoTracking();

    public async Task<TModel?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task AddAsync(TModel entity)
    {
        entity.CreatedAt = DateTime.Now;
        await _dbSet.AddAsync(entity);
    }


    public void SaveInclude(TModel entity, params Expression<Func<TModel, object>>[] propertyExpressions)
    {
        var local = _dbSet.Local.FirstOrDefault(x => x.Id == entity.Id);

        EntityEntry<TModel> entityEntry = null;

        if (local is null)
            entityEntry = _appDbContext.Entry(entity);
        else
            entityEntry = _appDbContext.ChangeTracker.Entries<TModel>()
                .First(x => x.Entity.Id == entity.Id);

        foreach (var propertyExpression in propertyExpressions)
        {
            var propertyToUpdate = entityEntry.Property(propertyExpression);
            var propertyName = propertyToUpdate.Metadata.Name;
            propertyToUpdate.CurrentValue = entity.GetType().GetProperty(propertyName)?.GetValue(entity);
            propertyToUpdate.IsModified = true;
        }
    }


    public void Delete(TModel entity) => _dbSet.Remove(entity);




    public IQueryable<TModel> GetAll(Expression<Func<TModel, bool>> expression) => _dbSet.AsNoTracking().Where(expression);


    public void SaveIncludeRange(IEnumerable<TModel> entities, params Expression<Func<TModel, object>>[] propertyExpressions)
    {
        foreach (var entity in entities)
        {
            SaveInclude(entity, propertyExpressions);
        }
    }

    public async Task<bool> DoesEntityExistAsync(int id) => await _dbSet.AsNoTracking().AnyAsync(e => e.Id == id);

    public async Task<bool> AnyAsync(Expression<Func<TModel, bool>> expression)
    {
        return await _dbSet.AsNoTracking().AnyAsync(expression);
    }
}