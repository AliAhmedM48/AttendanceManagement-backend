using Core.Models;
using System.Linq.Expressions;

namespace Core.Interfaces;
public interface IRepository<TModel> where TModel : BaseModel
{
    Task AddAsync(TModel entity);
    void SaveInclude(TModel entity, params Expression<Func<TModel, object>>[] propertyExpressions);
    void SaveIncludeRange(IEnumerable<TModel> entities, params Expression<Func<TModel, object>>[] propertyExpressions);
    IQueryable<TModel> GetAll();
    IQueryable<TModel> GetAll(Expression<Func<TModel, bool>> expression);

    Task<TModel?> GetByIdAsync(int id);
    void Delete(TModel entity);

    Task<bool> DoesEntityExistAsync(int id);
    Task<bool> AnyAsync(Expression<Func<TModel, bool>> expression);
}