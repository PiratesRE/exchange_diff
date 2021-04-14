using System;
using System.Configuration;

namespace Microsoft.Exchange.Data.Transport.StoreDriverDelivery
{
	internal abstract class StoreDriverDeliveryAgentFactory : AgentFactory
	{
		public abstract StoreDriverDeliveryAgent CreateAgent(SmtpServer server);

		internal override Agent CreateAgent(string typeName, object state)
		{
			if (typeName != typeof(StoreDriverDeliveryAgent).FullName || (state != null && !(state is SmtpServer)))
			{
				throw new ConfigurationErrorsException(string.Format("The supplied agent factory doesn't match the agent type found in the supplied assembly. typeName=={0} state=={1}", typeName, (state == null) ? "null" : state.GetType().ToString()));
			}
			return this.CreateAgent((SmtpServer)state);
		}
	}
}
