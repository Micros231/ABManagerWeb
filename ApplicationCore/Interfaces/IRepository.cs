using ABManagerWeb.ApplicationCore.Entities;
using System;
using System.Collections.Generic;

namespace ABManagerWeb.ApplicationCore.Interfaces
{
    public interface IRepository<T> : IDisposable where T : BaseEntity
    {
        T GetById(string id);
        IReadOnlyList<T> ListAll();
        IReadOnlyList<T> List(ISpecification<T> spec);
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        int Count(ISpecification<T> spec);
    }
}
