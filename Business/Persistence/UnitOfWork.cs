using System;
using System.Threading.Tasks;
using Business.Core;
using DataAccess.Data;

namespace Business.Persistence
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly CoreDbContext _dbContext;

        public UnitOfWork(CoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._dbContext.Dispose();
                }
            }

            this._disposed = true;
        }
    }
}
