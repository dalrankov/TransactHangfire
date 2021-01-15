using System.Transactions;

namespace TransactHangfire
{
    public interface ITransactionScopeFactory
    {
        TransactionScope CreateScope();
    }
}