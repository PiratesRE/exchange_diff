using System;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission.Agents
{
	internal static class ModeratedTransportHandling
	{
		internal static bool ResubmitApprovedMessage(MessageItem messageItem, TransportMailItem transportMailItem, string approverAddress)
		{
			DateTime utcNow = DateTime.UtcNow;
			HeaderList headerList = (transportMailItem.RootPart != null) ? transportMailItem.RootPart.Headers : null;
			if (!ModerationHelper.RestoreOriginalMessage(messageItem, transportMailItem, TraceHelper.ModeratedTransportTracer, TraceHelper.MessageProbeActivityId))
			{
				return false;
			}
			transportMailItem.RootPart.Headers.RemoveAll("X-MS-Exchange-Organization-Approval-Approved");
			TextHeader textHeader = (TextHeader)Header.Create("X-MS-Exchange-Organization-Approval-Approved");
			textHeader.Value = (string)transportMailItem.Recipients[0].Email;
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients)
			{
				if (mailRecipient.Status == Status.Ready)
				{
					textHeader.Value = (string)mailRecipient.Email;
					break;
				}
			}
			transportMailItem.RootPart.Headers.AppendChild(textHeader);
			if (transportMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Moderation-SavedArrivalTime") == null)
			{
				Header header = transportMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-OriginalArrivalTime");
				string value = null;
				if (header != null)
				{
					value = header.Value;
				}
				if (!string.IsNullOrEmpty(value))
				{
					transportMailItem.RootPart.Headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-Moderation-SavedArrivalTime", value));
				}
			}
			TextHeader textHeader2 = (TextHeader)Header.Create("X-Moderation-Data");
			ExDateTime? valueAsNullable = messageItem.GetValueAsNullable<ExDateTime>(MessageItemSchema.ApprovalDecisionTime);
			if (valueAsNullable != null)
			{
				textHeader2.Value = valueAsNullable.ToString();
				transportMailItem.RootPart.Headers.AppendChild(textHeader2);
			}
			if (headerList != null)
			{
				Header[] array = headerList.FindAll(HeaderId.Received);
				foreach (Header header2 in array)
				{
					Header header3 = Header.Create(HeaderId.Received);
					header2.CopyTo(header3);
					transportMailItem.RootPart.Headers.PrependChild(header3);
				}
			}
			transportMailItem.RootPart.Headers.RemoveAll("X-MS-Exchange-Organization-OriginalArrivalTime");
			transportMailItem.RootPart.Headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-OriginalArrivalTime", Util.FormatOrganizationalMessageArrivalTime(utcNow)));
			transportMailItem.UpdateDirectionalityAndScopeHeaders();
			transportMailItem.UpdateCachedHeaders();
			MessageTrackingLog.TrackReceiveForApprovalRelease(transportMailItem, approverAddress, messageItem.InternetMessageId);
			return true;
		}
	}
}
