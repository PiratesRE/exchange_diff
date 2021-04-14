using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal sealed class SubmissionDatabaseThreadMap : SynchronizedThreadMap<string>
	{
		public SubmissionDatabaseThreadMap(Trace tracer) : base(SubmissionDatabaseThreadMap.EstimatedCapacity, tracer, "Submitting mailbox database", 100, Environment.ProcessorCount * SubmissionConfiguration.Instance.App.MaxConcurrentSubmissions, "Too many concurrent submissions from mailbox database.  The limit is {1}.", true)
		{
		}

		private const string KeyDisplayName = "Submitting mailbox database";

		private const int EstimatedEntrySize = 100;

		private static readonly int EstimatedCapacity = Components.Configuration.LocalServer.MaxConcurrentMailboxSubmissions;
	}
}
