using System.Collections.Generic;
using DotNetNuke.Data;
using DotNetNuke.Framework;
using CRM.Modules.CRMProductDownload.Models;

namespace CRM.Modules.CRMProductDownload.Components
{
    interface ITransactionManager
    {

        void CreateTransaction(Transaction t);
        void DeleteTransaction(int transactionId, int moduleId);
        void DeleteTransaction(Transaction t);
        IEnumerable<Transaction> GetTransactions(int moduleId);
        Transaction GetTransaction(int TransactionId, int moduleId);
        void UpdateTransaction(Transaction t);

    }

    class TransactionManager : ServiceLocator<ITransactionManager, TransactionManager>, ITransactionManager
    {
        public void CreateTransaction(Transaction t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Transaction>();
                rep.Insert(t);
            }
        }

        public void DeleteTransaction(int transactionId, int moduleId)
        {
            var t = GetTransaction(transactionId, moduleId);
            DeleteTransaction(t);
        }

        public void DeleteTransaction(Transaction t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Transaction>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Transaction> GetTransactions(int moduleId)
        {
            IEnumerable<Transaction> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Transaction>();
                t = rep.Get(moduleId);
            }
            return t;
        }

        public Transaction GetTransaction(int transactionId, int moduleId)
        {
            Transaction t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Transaction>();
                t = rep.GetById(transactionId, moduleId);
            }
            return t;
        }

        public void UpdateTransaction(Transaction t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Transaction>();
                rep.Update(t);
            }
        }

        protected override System.Func<ITransactionManager> GetFactory()
        {
            return () => new TransactionManager();
        }
    }
}