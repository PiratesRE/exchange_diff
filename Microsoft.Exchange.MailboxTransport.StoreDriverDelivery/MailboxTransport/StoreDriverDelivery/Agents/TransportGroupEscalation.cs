using System;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.GroupMailbox.Escalation;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TransportGroupEscalation : GroupEscalation
	{
		public TransportGroupEscalation(MbxTransportMailItem mbxTransportMailItem, IXSOFactory xsoFactory, IGroupEscalationFlightInfo groupEscalationFlightInfo, IMailboxUrls mailboxUrls) : base(xsoFactory, groupEscalationFlightInfo, mailboxUrls)
		{
			this.mbxTransportMailItem = mbxTransportMailItem;
		}

		protected override void SendEscalateMessage(IMessageItem escalatedMessage)
		{
			escalatedMessage.CommitReplyTo();
			using (MemorySubmissionItem memorySubmissionItem = new MemorySubmissionItem((MessageItem)escalatedMessage, this.mbxTransportMailItem.OrganizationId))
			{
				memorySubmissionItem.Submit(MessageTrackingSource.AGENT, new MemorySubmissionItem.OnConvertedToTransportMailItemDelegate(this.TransportMailItemHandler), this.mbxTransportMailItem);
			}
		}

		private bool TransportMailItemHandler(TransportMailItem mailItem, bool isValid)
		{
			return isValid;
		}

		private readonly MbxTransportMailItem mbxTransportMailItem;
	}
}
