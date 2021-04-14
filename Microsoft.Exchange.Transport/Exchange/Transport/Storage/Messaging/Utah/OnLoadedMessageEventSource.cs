using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class OnLoadedMessageEventSource : StorageEventSource
	{
		public OnLoadedMessageEventSource(TransportMailItem mailItem)
		{
			this.mailItem = mailItem;
		}

		public override void Delete(string sourceContext)
		{
			LatencyFormatter latencyFormatter = new LatencyFormatter(this.mailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, true);
			foreach (MailRecipient mailRecipient in this.mailItem.Recipients)
			{
				MessageTrackingLog.TrackRecipientFail(MessageTrackingSource.AGENT, this.mailItem, mailRecipient.Email, mailRecipient.SmtpResponse, sourceContext, latencyFormatter);
			}
			this.mailItem.ReleaseFromActive();
			this.mailItem.CommitLazy();
		}

		public override void DeleteWithNdr(SmtpResponse response, string sourceContext)
		{
			if (this.mailItem.ADRecipientCache == null)
			{
				ADOperationResult adoperationResult = MultiTenantTransport.TryCreateADRecipientCache(this.mailItem);
				if (!adoperationResult.Succeeded)
				{
					MultiTenantTransport.TraceAttributionError(string.Format("Error {0} when creating recipient cache for message {1}. Falling back to first org", adoperationResult.Exception, MultiTenantTransport.ToString(this.mailItem)), new object[0]);
					MultiTenantTransport.UpdateADRecipientCacheAndOrganizationScope(this.mailItem, OrganizationId.ForestWideOrgId);
				}
			}
			TransportMailItemWrapper transportMailItemWrapper = new TransportMailItemWrapper(this.mailItem, true);
			EnvelopeRecipientCollection recipients = transportMailItemWrapper.Recipients;
			if (recipients == null || recipients.Count == 0)
			{
				return;
			}
			for (int i = recipients.Count - 1; i >= 0; i--)
			{
				transportMailItemWrapper.Recipients.Remove(recipients[i], DsnType.Failure, response, sourceContext);
			}
			Components.DsnGenerator.GenerateDSNs(this.mailItem);
			this.mailItem.ReleaseFromActive();
			this.mailItem.CommitLazy();
		}

		private TransportMailItem mailItem;
	}
}
