using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task
{
	internal class CompleteSearchMailbox : SearchTask<SearchMailboxesResults>
	{
		public CompleteSearchMailbox.CompleteSearchMailboxContext TaskContext
		{
			get
			{
				return (CompleteSearchMailbox.CompleteSearchMailboxContext)base.Context.TaskContext;
			}
		}

		public override void Process(SearchMailboxesResults item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "CompleteSearchMailbox.Process Item:", item);
			this.TaskContext.Results.MergeResults(item);
			base.Executor.EnqueueNext(this.TaskContext.Results);
		}

		internal class CompleteSearchMailboxContext
		{
			public CompleteSearchMailboxContext()
			{
				this.Results = new SearchMailboxesResults(null);
			}

			public SearchMailboxesResults Results { get; private set; }
		}
	}
}
