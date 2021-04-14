using System;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal class SearchTaskContext
	{
		public IExecutor Executor { get; set; }

		public object Item { get; set; }

		public object TaskContext { get; set; }
	}
}
