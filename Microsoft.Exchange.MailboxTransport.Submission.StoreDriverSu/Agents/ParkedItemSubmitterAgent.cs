using System;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission.Agents
{
	internal class ParkedItemSubmitterAgent : SubmissionAgent
	{
		public ParkedItemSubmitterAgent()
		{
			base.OnDemotedMessage += this.OnDemotedMessageHandler;
		}

		public void OnDemotedMessageHandler(StoreDriverEventSource source, StoreDriverSubmissionEventArgs args)
		{
			if (SubmissionConfiguration.Instance.App.EnableUnparkedMessageRestoring)
			{
				MailItem mailItem = args.MailItem;
				StoreDriverSubmissionEventArgsImpl storeDriverSubmissionEventArgsImpl = (StoreDriverSubmissionEventArgsImpl)args;
				MessageItem item = storeDriverSubmissionEventArgsImpl.SubmissionItem.Item;
				StoreSession session = storeDriverSubmissionEventArgsImpl.SubmissionItem.Session;
				if (ObjectClass.IsParkedMeetingMessage(storeDriverSubmissionEventArgsImpl.SubmissionItem.MessageClass))
				{
					DateTime utcNow = DateTime.UtcNow;
					TransportMailItem transportMailItem = ((TransportMailItemWrapper)mailItem).TransportMailItem;
					ModerationHelper.RestoreOriginalMessage(item, transportMailItem, TraceHelper.ParkedItemSubmitterAgentTracer, TraceHelper.MessageProbeActivityId);
					transportMailItem.RootPart.Headers.RemoveAll("X-MS-Exchange-Organization-OriginalArrivalTime");
					transportMailItem.RootPart.Headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-OriginalArrivalTime", Util.FormatOrganizationalMessageArrivalTime(utcNow)));
					transportMailItem.ExtendedProperties.Clear();
					transportMailItem.UpdateDirectionalityAndScopeHeaders();
					transportMailItem.UpdateCachedHeaders();
				}
			}
		}
	}
}
