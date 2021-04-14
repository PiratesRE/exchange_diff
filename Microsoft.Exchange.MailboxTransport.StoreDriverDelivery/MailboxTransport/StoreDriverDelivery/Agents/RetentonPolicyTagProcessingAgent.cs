using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.InfoWorker.Common.ELC;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class RetentonPolicyTagProcessingAgent : StoreDriverDeliveryAgent
	{
		public RetentonPolicyTagProcessingAgent()
		{
			base.OnCreatedMessage += this.OnCreatedMessageHandler;
		}

		public void OnCreatedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			if (RetentonPolicyTagProcessingAgent.IsRetentionPolicyEnabled(storeDriverDeliveryEventArgsImpl.ADRecipientCache, storeDriverDeliveryEventArgsImpl.MailRecipient.Email))
			{
				RetentionTagHelper.ApplyPolicy(storeDriverDeliveryEventArgsImpl.MailboxSession, storeDriverDeliveryEventArgsImpl.RetentionPolicyTag, storeDriverDeliveryEventArgsImpl.RetentionFlags, storeDriverDeliveryEventArgsImpl.RetentionPeriod, storeDriverDeliveryEventArgsImpl.ArchiveTag, storeDriverDeliveryEventArgsImpl.ArchivePeriod, storeDriverDeliveryEventArgsImpl.CompactDefaultRetentionPolicy, storeDriverDeliveryEventArgsImpl.MessageItem);
			}
		}

		internal static bool IsRetentionPolicyEnabled(ADRecipientCache<TransportMiniRecipient> cache, RoutingAddress address)
		{
			ProxyAddress proxyAddress = new SmtpProxyAddress((string)address, true);
			TransportMiniRecipient data = cache.FindAndCacheRecipient(proxyAddress).Data;
			if (data == null)
			{
				return false;
			}
			ElcMailboxFlags elcMailboxFlags = data.ElcMailboxFlags;
			ADObjectId elcPolicyTemplate = data.ElcPolicyTemplate;
			return ((elcMailboxFlags & ElcMailboxFlags.ElcV2) != ElcMailboxFlags.None && elcPolicyTemplate != null) || ((elcMailboxFlags & ElcMailboxFlags.ShouldUseDefaultRetentionPolicy) != ElcMailboxFlags.None && elcPolicyTemplate == null);
		}
	}
}
