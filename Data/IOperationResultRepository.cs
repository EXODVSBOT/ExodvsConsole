using Domain.Class;

namespace Data;

public interface IOperationResultRepository
{
    void EnsureFileExists();
    void Create(OperationResultDomain entity);
    IEnumerable<OperationResultDomain> ReadAll();
    OperationResultDomain ReadById(Func<OperationResultDomain, bool> predicate);
    void Update(Func<OperationResultDomain, bool> predicate, OperationResultDomain newEntity);
    void Delete(Func<OperationResultDomain, bool> predicate);
    void ClearAll();
}