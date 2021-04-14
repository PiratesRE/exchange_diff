using System;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal interface IExecutor
	{
		ISearchPolicy Policy { get; }

		ExecutorContext Context { get; }

		void EnqueueNext(object item);

		void Fail(Exception ex);
	}
}
