using System;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class AgentFactory
	{
		public virtual void Close()
		{
		}

		internal abstract Agent CreateAgent(string typeName, object state);
	}
}
