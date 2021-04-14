using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class OutgoingCallLogPipelineContext : CallLogPipelineContextBase
	{
		internal OutgoingCallLogPipelineContext(SubmissionHelper helper) : base(helper)
		{
			base.MessageType = "OutgoingCallLog";
		}

		internal OutgoingCallLogPipelineContext(SubmissionHelper helper, ContactInfo targetContact, UMRecipient recipient) : base(helper, recipient)
		{
			base.ContactInfo = targetContact;
			base.MessageType = "OutgoingCallLog";
		}

		protected override string GetMessageSubject(MessageContentBuilder contentBuilder)
		{
			return contentBuilder.GetOutgoingCallLogSubject(base.ContactInfo, base.CallerId);
		}

		protected override void AddMessageBody(MessageContentBuilder contentBuilder)
		{
			contentBuilder.AddOutgoingCallLogBody(base.CallerId, base.ContactInfo);
		}

		protected override void SetMessageProperties()
		{
			base.SetMessageProperties();
			base.MessageToSubmit.From = new Participant(base.CAMessageRecipient.ADRecipient);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OutgoingCallLogPipelineContext>(this);
		}
	}
}
