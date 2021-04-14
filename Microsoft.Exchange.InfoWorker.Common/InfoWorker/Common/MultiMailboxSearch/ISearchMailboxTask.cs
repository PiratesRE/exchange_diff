using System;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface ISearchMailboxTask
	{
		MailboxInfo Mailbox { get; }

		SearchType Type { get; }

		void Execute(SearchCompletedCallback callback);

		bool ShouldRetry(ISearchTaskResult result);

		ISearchTaskResult GetErrorResult(Exception ex);

		void Abort();
	}
}
