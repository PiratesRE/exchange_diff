using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal sealed class ProtocolAnalysisBgAgentFactory
	{
		public ProtocolAnalysisBgAgentFactory()
		{
			Database.Attach();
		}

		public void CreateAgent(SmtpServer server)
		{
			lock (this.syncObject)
			{
				if (this.PaBgAgent == null)
				{
					this.PaBgAgent = new ProtocolAnalysisBgAgent(server);
					this.isCreated = true;
				}
			}
		}

		public void Close()
		{
			lock (this.syncObject)
			{
				if (this.PaBgAgent != null)
				{
					this.PaBgAgent.Shutdown();
					this.PaBgAgent = null;
					ProtocolAnalysisBgAgent.PerformanceCounters.RemoveCounters();
				}
				else if (this.isCreated)
				{
					throw new InvalidOperationException("Trying to destroy ProtocolAnalysisBgAgent more than once.");
				}
			}
		}

		internal ProtocolAnalysisBgAgent PaBgAgent;

		private bool isCreated;

		private object syncObject = new object();
	}
}
