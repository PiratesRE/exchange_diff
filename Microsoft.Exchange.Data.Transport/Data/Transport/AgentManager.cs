using System;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class AgentManager
	{
		internal string AgentName
		{
			get
			{
				return this.agentName;
			}
			set
			{
				this.agentName = value;
			}
		}

		private string agentName;
	}
}
