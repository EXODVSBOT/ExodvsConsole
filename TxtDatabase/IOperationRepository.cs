using System;
using System.Collections.Generic;

namespace TxtDatabase
{
    public interface IOperationRepository<T> where T : class
    {
        void EnsureFileExists();
        void Create(T entity);
        IEnumerable<T> ReadAll();
        T ReadById(Func<T, bool> predicate);
        void Update(Func<T, bool> predicate, T newEntity);
        void Delete(Func<T, bool> predicate);
        void ClearAll();
    }
}