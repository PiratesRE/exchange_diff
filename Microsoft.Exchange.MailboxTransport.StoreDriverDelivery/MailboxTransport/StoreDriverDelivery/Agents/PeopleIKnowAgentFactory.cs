using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PeopleIKnowAgentFactory : StoreDriverDeliveryAgentFactory
	{
		public PeopleIKnowAgentFactory()
		{
			this.isAgentEnabled = this.ReadAgentEnabledValueFromConfigFile();
		}

		private bool ReadAgentEnabledValueFromConfigFile()
		{
			return TransportAppConfig.GetConfigBool("PeopleIKnowAgentEnabled", true);
		}

		public override StoreDriverDeliveryAgent CreateAgent(SmtpServer server)
		{
			return new PeopleIKnowAgent(this.isAgentEnabled);
		}

		private const string PeopleIKnowAgentEnabled = "PeopleIKnowAgentEnabled";

		private readonly bool isAgentEnabled;
	}
}
