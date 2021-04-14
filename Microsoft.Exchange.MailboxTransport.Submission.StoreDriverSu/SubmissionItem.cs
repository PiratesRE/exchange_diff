using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.MailboxTransport.Shared.SubmissionItem;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal abstract class SubmissionItem : SubmissionItemBase
	{
		public SubmissionItem(string mailProtocol, IStoreDriverTracer storeDriverTracer) : this(mailProtocol, null, null, storeDriverTracer)
		{
		}

		public SubmissionItem(string mailProtocol, MailItemSubmitter context, SubmissionInfo submissionInfo, IStoreDriverTracer storeDriverTracer) : base(mailProtocol, storeDriverTracer)
		{
			this.Context = context;
			this.Info = submissionInfo;
		}

		private protected SubmissionInfo Info { protected get; private set; }

		private protected MailItemSubmitter Context { protected get; private set; }

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private static readonly Trace diag = ExTraceGlobals.MapiStoreDriverSubmissionTracer;
	}
}
