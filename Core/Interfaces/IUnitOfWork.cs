using Core.Models;

namespace Core.Interfaces;
public interface IUnitOfWork
{
    IRepository<TModel> GetRepository<TModel>() where TModel : BaseModel;
    Task<int> SaveChangesAsync();
}