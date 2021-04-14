using System;
using System.Configuration;

namespace Microsoft.Exchange.Data.Transport.Storage
{
	public abstract class StorageAgentFactory : AgentFactory
	{
		internal abstract StorageAgent CreateAgent(SmtpServer server);

		internal override Agent CreateAgent(string typeName, object state)
		{
			if (typeName != typeof(StorageAgent).FullName)
			{
				throw new ConfigurationErrorsException(string.Format("The supplied agent factory doesn't match the agent type found in the supplied assembly. typeName=={0}", typeName));
			}
			return this.CreateAgent(null);
		}
	}
}
