using System;
using System.Threading.Tasks;

namespace Business.Core
{
    public interface IUnitOfWork: IDisposable
    {
        Task<int> CommitAsync();
        int Commit();
    }
}
