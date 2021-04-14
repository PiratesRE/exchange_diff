using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriver.Shared;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class StoreDriverDeliveryServer : StoreDriverServer
	{
		private StoreDriverDeliveryServer(OrganizationId organizationId) : base(organizationId)
		{
			this.agentLoopChecker = new AgentGeneratedMessageLoopChecker(new AgentGeneratedMessageLoopCheckerTransportConfig(Components.Configuration));
		}

		public new static StoreDriverDeliveryServer GetInstance(OrganizationId organizationId)
		{
			return new StoreDriverDeliveryServer(organizationId);
		}

		public override void SubmitMessage(EmailMessage message)
		{
			throw new NotImplementedException();
		}

		public override void SubmitMessage(IReadOnlyMailItem originalMailItem, EmailMessage message, OrganizationId organizationId, Guid externalOrganizationId, bool suppressDSNs)
		{
			try
			{
				TransportMailItem mailItem = SubmitHelper.CreateTransportMailItem(originalMailItem, message, this.Name, this.Version, (base.AssociatedAgent != null) ? base.AssociatedAgent.Name : "StoreDriverServer", null, null, organizationId, externalOrganizationId, false);
				this.SubmitMailItem(mailItem, suppressDSNs);
			}
			catch (Exception innerException)
			{
				throw new StoreDriverAgentTransientException(Strings.StoreDriverAgentTransientExceptionEmail, innerException);
			}
		}

		public override void SubmitMailItem(TransportMailItem mailItem, bool suppressDSNs)
		{
			StoreDriverDeliveryEventArgs storeDriverDeliveryEventArgs = null;
			if (base.AssociatedAgent != null && base.AssociatedAgent.Session != null && base.AssociatedAgent.Session.CurrentEventArgs != null)
			{
				storeDriverDeliveryEventArgs = (base.AssociatedAgent.Session.CurrentEventArgs as StoreDriverDeliveryEventArgs);
			}
			bool flag = this.agentLoopChecker.IsEnabledInSubmission();
			bool flag2 = false;
			if (storeDriverDeliveryEventArgs != null && !string.IsNullOrEmpty(base.AssociatedAgent.Name))
			{
				flag2 = this.agentLoopChecker.CheckAndStampInSubmission(storeDriverDeliveryEventArgs.MailItem.Message.RootPart.Headers, mailItem.RootPart.Headers, base.AssociatedAgent.Name);
				if (flag2)
				{
					MessageTrackingLog.TrackAgentGeneratedMessageRejected(MessageTrackingSource.STOREDRIVER, flag, mailItem);
				}
			}
			if (flag2 && flag)
			{
				using (IEnumerator<MailRecipient> enumerator = mailItem.Recipients.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MailRecipient mailRecipient = enumerator.Current;
						mailRecipient.Ack(AckStatus.Fail, SmtpResponse.AgentGeneratedMessageDepthExceeded);
					}
					return;
				}
			}
			Utils.SubmitMailItem(mailItem, suppressDSNs);
		}

		private AgentGeneratedMessageLoopChecker agentLoopChecker;
	}
}
