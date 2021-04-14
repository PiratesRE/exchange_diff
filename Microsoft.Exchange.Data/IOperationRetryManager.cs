using System;

namespace Microsoft.Exchange.Data
{
	internal interface IOperationRetryManager
	{
		OperationRetryManagerResult TryRun(Action operation);

		void Run(Action operation);
	}
}
