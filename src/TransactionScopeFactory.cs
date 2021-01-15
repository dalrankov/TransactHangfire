using System;
using System.Transactions;

namespace TransactHangfire
{
    class TransactionScopeFactory
        : ITransactionScopeFactory
    {
        static readonly TransactionOptions _transactionOptions =
            new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(1)
            };

        public TransactionScope CreateScope()
        {
            return new TransactionScope(
                TransactionScopeOption.Required,
                _transactionOptions,
                TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}