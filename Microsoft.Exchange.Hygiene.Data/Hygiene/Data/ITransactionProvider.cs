using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal interface ITransactionProvider
	{
		void InvokeWithTransaction(Action action, string transactionIdentifier);
	}
}
