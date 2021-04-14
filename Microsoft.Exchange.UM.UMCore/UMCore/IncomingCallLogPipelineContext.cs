using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class IncomingCallLogPipelineContext : CallLogPipelineContextBase
	{
		internal IncomingCallLogPipelineContext(SubmissionHelper helper) : base(helper)
		{
			base.MessageType = "IncomingCallLog";
		}

		internal IncomingCallLogPipelineContext(SubmissionHelper helper, UMRecipient recipient) : base(helper, recipient)
		{
			base.MessageType = "IncomingCallLog";
		}

		protected override string GetMessageSubject(MessageContentBuilder contentBuilder)
		{
			return contentBuilder.GetIncomingCallLogSubject(base.ContactInfo, base.CallerId);
		}

		protected override void AddMessageBody(MessageContentBuilder contentBuilder)
		{
			contentBuilder.AddIncomingCallLogBody(base.CallerId, base.ContactInfo);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IncomingCallLogPipelineContext>(this);
		}
	}
}
