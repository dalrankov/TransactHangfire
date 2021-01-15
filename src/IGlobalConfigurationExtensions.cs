using Hangfire;

namespace TransactHangfire
{
    public static class IGlobalConfigurationExtensions
    {
        /// <summary>
        /// Wrap all Hangfire jobs within individual transaction scopes.
        /// </summary>
        public static IGlobalConfiguration UseTransactionScope(
            this IGlobalConfiguration configuration)
        {
            return configuration.UseTransactionScope<TransactionScopeFactory>();
        }

        /// <summary>
        /// Wrap all Hangfire jobs within individual transaction scopes.
        /// </summary>
        /// <typeparam name="TFactory">Custom TransactionScope factory class with parameterless constructor and implementing <see cref="ITransactionScopeFactory"/>.</typeparam>
        public static IGlobalConfiguration UseTransactionScope<TFactory>(
            this IGlobalConfiguration configuration) where TFactory : ITransactionScopeFactory, new()
        {
            return configuration.UseFilter(new UseTransactionScopeAttribute(typeof(TFactory)));
        }
    }
}