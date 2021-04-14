using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal class AgentAsyncState
	{
		internal AgentAsyncState()
		{
		}

		public AgentAsyncState(AgentAsyncContext agentAsyncContext)
		{
			if (agentAsyncContext == null)
			{
				throw new ArgumentException("AgentAsyncContext");
			}
			this.agentAsyncContext = agentAsyncContext;
		}

		public virtual void Resume()
		{
			this.agentAsyncContext.Resume();
		}

		public virtual void Complete()
		{
			this.agentAsyncContext.Complete();
			this.agentAsyncContext = null;
		}

		private AgentAsyncContext agentAsyncContext;
	}
}
