using System;
using System.Configuration;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public abstract class SmtpReceiveAgentFactory : AgentFactory
	{
		public abstract SmtpReceiveAgent CreateAgent(SmtpServer server);

		internal override Agent CreateAgent(string typeName, object state)
		{
			if (typeName != typeof(SmtpReceiveAgent).FullName || (state != null && !(state is SmtpServer)))
			{
				throw new ConfigurationErrorsException(string.Format("The supplied agent factory doesn't match the agent type found in the supplied assembly. typeName=={0}, state=={1}", typeName, state));
			}
			return this.CreateAgent((SmtpServer)state);
		}
	}
}
