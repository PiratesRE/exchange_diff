using System;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal delegate void SearchCompletedCallback(ISearchMailboxTask task, ISearchTaskResult result);
}
