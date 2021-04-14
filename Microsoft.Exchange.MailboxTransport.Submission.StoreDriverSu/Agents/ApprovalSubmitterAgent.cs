using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission.Agents
{
	internal class ApprovalSubmitterAgent : SubmissionAgent
	{
		public ApprovalSubmitterAgent()
		{
			base.OnDemotedMessage += this.OnDemotedMessageHandler;
		}

		public void OnDemotedMessageHandler(StoreDriverEventSource source, StoreDriverSubmissionEventArgs args)
		{
			MailItem mailItem = args.MailItem;
			StoreDriverSubmissionEventArgsImpl storeDriverSubmissionEventArgsImpl = (StoreDriverSubmissionEventArgsImpl)args;
			MessageItem item = storeDriverSubmissionEventArgsImpl.SubmissionItem.Item;
			if (ApprovalInitiation.IsArbitrationMailbox((ADRecipientCache<TransportMiniRecipient>)mailItem.RecipientCache, mailItem.FromAddress))
			{
				item.Load(ApprovalSubmitterAgent.ModeratedTransportProperties);
				ApprovalStatus? valueAsNullable = item.GetValueAsNullable<ApprovalStatus>(MessageItemSchema.ApprovalStatus);
				string valueOrDefault = item.GetValueOrDefault<string>(MessageItemSchema.ApprovalDecisionMaker, string.Empty);
				if (valueAsNullable != null && (valueAsNullable.Value & ApprovalStatus.Approved) == ApprovalStatus.Approved)
				{
					TransportMailItem transportMailItem = ((TransportMailItemWrapper)mailItem).TransportMailItem;
					ModeratedTransportHandling.ResubmitApprovedMessage(item, transportMailItem, valueOrDefault);
					return;
				}
				if (ObjectClass.IsOfClass(item.ClassName, "IPM.Microsoft.Approval.Initiation") || ObjectClass.IsOfClass(item.ClassName, "IPM.Note.Microsoft.Approval.Request.Recall"))
				{
					Header header = (TextHeader)Header.Create("X-MS-Exchange-Organization-Do-Not-Journal");
					header.Value = "ArbitrationMailboxSubmission";
					mailItem.MimeDocument.RootPart.Headers.AppendChild(header);
					Header newChild = new AsciiTextHeader("X-MS-Exchange-Organization-Approval-Initiator", "mapi");
					mailItem.MimeDocument.RootPart.Headers.AppendChild(newChild);
				}
			}
		}

		private const string DoNotJournalIdentifier = "ArbitrationMailboxSubmission";

		private const string MapiApprovalInitiator = "mapi";

		internal static readonly PropertyDefinition[] ModeratedTransportProperties = new PropertyDefinition[]
		{
			MessageItemSchema.ApprovalDecisionMaker,
			MessageItemSchema.ApprovalStatus
		};
	}
}
