using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class ApprovalProcessingAgentFactory : StoreDriverDeliveryAgentFactory
	{
		public override StoreDriverDeliveryAgent CreateAgent(SmtpServer server)
		{
			if (this.counterInstance == null)
			{
				this.counterInstance = MSExchangeTransportApproval.GetInstance("_Total");
			}
			return new ApprovalProcessingAgent(server, this.counterInstance);
		}

		private const string CounterInstanceName = "_Total";

		private MSExchangeTransportApprovalInstance counterInstance;
	}
}
