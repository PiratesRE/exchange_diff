using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.MailboxTransport.StoreDriver
{
	internal class MemorySubmissionItem : SubmissionItem
	{
		public MemorySubmissionItem(MessageItem item, OrganizationId organizationId) : base("Microsoft SMTP Server")
		{
			base.Item = item;
			this.organizationId = organizationId;
			this.submissionTime = DateTime.UtcNow;
		}

		public override string SourceServerFqdn
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.Fqdn;
			}
		}

		public override IPAddress SourceServerNetworkAddress
		{
			get
			{
				return StoreDriver.LocalIPAddress;
			}
		}

		public override DateTime OriginalCreateTime
		{
			get
			{
				return this.submissionTime;
			}
		}

		public override bool HasMessageItem
		{
			get
			{
				return base.Item != null;
			}
		}

		public override TenantPartitionHint TenantPartitionHint
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public void Submit(MessageTrackingSource messageTrackingSource, MemorySubmissionItem.OnConvertedToTransportMailItemDelegate transportMailItemHandler, RoutedMailItem relatedMailItem)
		{
			TransportMailItem transportMailItem;
			if (relatedMailItem == null)
			{
				transportMailItem = TransportMailItem.NewMailItem(this.organizationId, LatencyComponent.StoreDriverSubmit, MailDirectionality.Undefined, default(Guid));
			}
			else
			{
				transportMailItem = TransportMailItem.NewSideEffectMailItem(relatedMailItem, this.organizationId, LatencyComponent.StoreDriverSubmit, MailDirectionality.Undefined, default(Guid));
			}
			base.CopyContentTo(transportMailItem);
			base.DecorateMessage(transportMailItem);
			base.ApplySecurityAttributesTo(transportMailItem);
			if (relatedMailItem != null)
			{
				transportMailItem.PrioritizationReason = relatedMailItem.PrioritizationReason;
				transportMailItem.Priority = relatedMailItem.Priority;
			}
			MailItemSubmitter.CopySenderTo(this, transportMailItem);
			List<string> invalidRecipients = null;
			List<string> list = null;
			MailItemSubmitter.CopyRecipientsTo(this, transportMailItem, null, ref invalidRecipients, ref list);
			ClassificationUtils.PromoteStoreClassifications(transportMailItem.RootPart.Headers);
			MailItemSubmitter.PatchQuarantineSender(transportMailItem, base.QuarantineOriginalSender);
			bool flag = transportMailItem.Recipients.Count > 0;
			if (relatedMailItem != null)
			{
				MimeInternalHelpers.CopyHeaderBetweenList(relatedMailItem.RootPart.Headers, transportMailItem.RootPart.Headers, "X-MS-Exchange-Moderation-Loop");
			}
			bool flag2 = transportMailItemHandler(transportMailItem, flag);
			if (flag && flag2)
			{
				transportMailItem.CommitLazy();
				MsgTrackReceiveInfo msgTrackInfo = new MsgTrackReceiveInfo(StoreDriver.LocalIPAddress, (relatedMailItem != null) ? new long?(relatedMailItem.RecordId) : null, transportMailItem.MessageTrackingSecurityInfo, invalidRecipients);
				MessageTrackingLog.TrackReceive(messageTrackingSource, transportMailItem, msgTrackInfo);
				Components.CategorizerComponent.EnqueueSubmittedMessage(transportMailItem);
			}
		}

		private OrganizationId organizationId;

		private DateTime submissionTime;

		public delegate bool OnConvertedToTransportMailItemDelegate(TransportMailItem mailItem, bool isValid);
	}
}
