using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Update
{
	internal sealed class StsUpdateAgentFactory
	{
		public StsUpdateAgentFactory()
		{
			Database.Attach();
		}

		public void CreateAgent(SmtpServer server)
		{
			lock (this.syncObject)
			{
				if (this.stsUpdateAgent == null)
				{
					this.stsUpdateAgent = new StsUpdateAgent();
					this.stsUpdateAgent.Startup();
					this.isCreated = true;
				}
			}
		}

		public void Close()
		{
			lock (this.syncObject)
			{
				if (this.stsUpdateAgent != null)
				{
					this.stsUpdateAgent.Shutdown();
					this.stsUpdateAgent = null;
					StsUpdateAgent.PerformanceCounters.RemoveCounters();
				}
				else if (this.isCreated)
				{
					throw new InvalidOperationException("Trying to release Update Agent more than once");
				}
			}
		}

		private StsUpdateAgent stsUpdateAgent;

		private bool isCreated;

		private object syncObject = new object();
	}
}
