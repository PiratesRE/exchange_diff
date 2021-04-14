using System;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	internal static class TransactionScopeHelper
	{
		public static void Run(this DomainSession session, bool useTransaction, string transactionIdentifier, Action action)
		{
			if (useTransaction)
			{
				TransactionScopeHelper.RunWithTransaction(session, transactionIdentifier, action);
				return;
			}
			TransactionScopeHelper.RunWithoutTransaction(session, action);
		}

		private static void RunWithTransaction(DomainSession session, string transactionIdentifier, Action action)
		{
			session.TraceDebug("Running with application transaction:{0}", new object[]
			{
				transactionIdentifier
			});
			if (!(session.DefaultDataProvider is ITransactionProvider))
			{
				session.TraceError("Default data provider:{0} for Domain session does not implement ITransactionProvider", new object[]
				{
					session.DefaultDataProvider
				});
				throw new InvalidOperationException(string.Format(HygieneDataStrings.ErrorTransactionNotSupported, session.DefaultDataProvider.GetType().Name, session.GetType().Name));
			}
			bool flag = false;
			try
			{
				ITransactionProvider transactionProvider = session.DefaultDataProvider as ITransactionProvider;
				transactionProvider.InvokeWithTransaction(action, transactionIdentifier);
				flag = true;
			}
			finally
			{
				if (flag)
				{
					session.TraceDebug("Application transaction completed:{0}", new object[]
					{
						transactionIdentifier
					});
				}
				else
				{
					session.TraceWarning("Application transaction failed:{0}", new object[]
					{
						transactionIdentifier
					});
				}
			}
		}

		private static void RunWithoutTransaction(DomainSession session, Action action)
		{
			session.TraceDebug("Running without application transaction");
			action();
		}
	}
}
