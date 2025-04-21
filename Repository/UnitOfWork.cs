using Core.Interfaces;
using Core.Models;
using Repository.Data;
using Repository.Repositories;
using System.Collections;

namespace Repository;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _appDbContext;
    private readonly Hashtable _repositories;

    public UnitOfWork(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
        _repositories = new Hashtable();
    }
    public IRepository<TModel> GetRepository<TModel>() where TModel : BaseModel
    {
        var type = typeof(TModel).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repository = new Repository<TModel>(_appDbContext);
            _repositories.Add(type, repository);
        }

        return (IRepository<TModel>)_repositories[type];

    }

    public async Task<int> SaveChangesAsync()
    {
        return await _appDbContext.SaveChangesAsync();
    }
}