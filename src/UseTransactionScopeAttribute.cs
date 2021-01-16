using Hangfire.Common;
using Hangfire.Server;
using System;
using System.Collections.Concurrent;
using System.Transactions;

namespace TransactHangfire
{
    /// <summary>
    /// Wrap Hangfire job within transaction scope. 
    /// If it's placed on a class level, it is going to wrap all invoked methods inside individual transaction scopes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class UseTransactionScopeAttribute
        : JobFilterAttribute, IServerFilter
    {
        static readonly ITransactionScopeFactory _defaultFactory = new TransactionScopeFactory();
        static readonly ConcurrentDictionary<string, TransactionScope> _transactionScopes =
            new ConcurrentDictionary<string, TransactionScope>();

        readonly ITransactionScopeFactory _factory;

        public UseTransactionScopeAttribute()
        {
            _factory = _defaultFactory;
        }

        /// <param name="factoryType">Custom TransactionScope factory class type with parameterless constructor and implementing <see cref="ITransactionScopeFactory"/>.</param>
        public UseTransactionScopeAttribute(
           Type factoryType)
        {
            if (!(typeof(ITransactionScopeFactory).IsAssignableFrom(factoryType)
                && factoryType.IsClass
                && factoryType.GetConstructor(Type.EmptyTypes) != null))
            {
                throw new ArgumentException($"{factoryType.Name} is not a class with parameterless constructor and implementing {nameof(ITransactionScopeFactory)}!");
            }

            _factory = Activator.CreateInstance(factoryType) as ITransactionScopeFactory;
        }

        public void OnPerformed(
            PerformedContext context)
        {
            if (_transactionScopes.TryRemove(context.BackgroundJob.Id, out TransactionScope scope))
            {
                if (!(context.Canceled || (context.Exception != null && !context.ExceptionHandled)))
                {
                    scope.Complete();
                }

                scope.Dispose();
            }
        }

        public void OnPerforming(
            PerformingContext context)
        {
            _transactionScopes.TryAdd(context.BackgroundJob.Id, _factory.CreateScope());
        }
    }
}