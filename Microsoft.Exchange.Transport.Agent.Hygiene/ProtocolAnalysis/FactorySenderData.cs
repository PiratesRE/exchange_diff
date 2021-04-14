using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	internal class FactorySenderData : SenderData
	{
		public FactorySenderData(DateTime tsCreate) : base(tsCreate)
		{
			this.helloDomains = new HybridDictionary(10);
			this.reverseDns = string.Empty;
		}

		public IDictionaryEnumerator HelloDomainEnumerator
		{
			get
			{
				return this.helloDomains.GetEnumerator();
			}
		}

		public DateTime LastUpdateTime
		{
			get
			{
				return this.lastUpdate;
			}
		}

		public string ReverseDns
		{
			get
			{
				return this.reverseDns;
			}
		}

		public void Merge(AgentSenderData agentData)
		{
			this.lastUpdate = DateTime.UtcNow;
			base.Merge(agentData);
			int num = 0;
			if (this.helloDomains.Contains(agentData.HelloDomain))
			{
				num = (int)this.helloDomains[agentData.HelloDomain];
			}
			num += agentData.NumMsgs;
			this.helloDomains[agentData.HelloDomain] = num;
			if (!string.IsNullOrEmpty(agentData.ReverseDns))
			{
				this.reverseDns = agentData.ReverseDns;
			}
		}

		private HybridDictionary helloDomains;

		private DateTime lastUpdate;

		private string reverseDns;
	}
}
