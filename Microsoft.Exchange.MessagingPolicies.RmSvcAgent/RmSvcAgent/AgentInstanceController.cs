using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal sealed class AgentInstanceController
	{
		private AgentInstanceController(int maxActiveAgents, bool multiTenancyEnabled)
		{
			this.totalSlots = maxActiveAgents;
			this.freeSlots = maxActiveAgents;
			this.multiTenancyEnabled = multiTenancyEnabled;
			if (multiTenancyEnabled)
			{
				this.agents = new Dictionary<Guid, int>(100);
			}
			else
			{
				this.agents = new Dictionary<Guid, int>(1);
			}
			RmSvcAgentPerfCounters.CurrentActiveAgents.RawValue = 0L;
			RmSvcAgentPerfCounters.TotalSuccessfulActiveRequests.RawValue = 0L;
			RmSvcAgentPerfCounters.TotalUnsuccessfulActiveRequests.RawValue = 0L;
		}

		public static AgentInstanceController Instance
		{
			get
			{
				AgentInstanceController result;
				lock (AgentInstanceController.syncRoot)
				{
					if (AgentInstanceController.instance == null)
					{
						throw new InvalidOperationException("AgentInstanceController.Instance is not initialized yet!");
					}
					result = AgentInstanceController.instance;
				}
				return result;
			}
		}

		public static void Initialize()
		{
			lock (AgentInstanceController.syncRoot)
			{
				if (AgentInstanceController.instance == null)
				{
					bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
					AgentInstanceController.instance = new AgentInstanceController(Utils.GetMaxActiveAgents(), enabled);
				}
			}
		}

		public bool TryMakeActive(Guid tenantId)
		{
			bool flag = false;
			lock (this.agents)
			{
				if (this.freeSlots > 0)
				{
					int num = 0;
					bool flag3 = this.agents.TryGetValue(tenantId, out num);
					if ((this.multiTenancyEnabled && this.freeSlots > num) || (!this.multiTenancyEnabled && this.freeSlots > 0))
					{
						if (flag3)
						{
							this.agents[tenantId] = num + 1;
						}
						else
						{
							this.agents.Add(tenantId, 1);
						}
						this.freeSlots--;
						flag = true;
						RmSvcAgentPerfCounters.CurrentActiveAgents.RawValue = (long)(this.totalSlots - this.freeSlots);
					}
				}
			}
			if (flag)
			{
				RmSvcAgentPerfCounters.TotalSuccessfulActiveRequests.Increment();
			}
			else
			{
				RmSvcAgentPerfCounters.TotalUnsuccessfulActiveRequests.Increment();
			}
			return flag;
		}

		public void MakeInactive(Guid tenantId)
		{
			lock (this.agents)
			{
				int num = 0;
				bool flag2 = this.agents.TryGetValue(tenantId, out num);
				if (flag2)
				{
					this.freeSlots++;
					if (num > 1)
					{
						this.agents[tenantId] = num - 1;
					}
					else
					{
						this.agents.Remove(tenantId);
					}
					RmSvcAgentPerfCounters.CurrentActiveAgents.RawValue = (long)(this.totalSlots - this.freeSlots);
				}
			}
		}

		public override string ToString()
		{
			string text = string.Format("Total slots: {0}, Free slots: {1}", this.totalSlots, this.freeSlots);
			if (this.multiTenancyEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder(text);
				lock (this.agents)
				{
					foreach (Guid guid in this.agents.Keys)
					{
						int num = this.agents[guid];
						if (num > 0)
						{
							stringBuilder.AppendFormat("{0}Tenant: {1} - Used Slots: {2}", Environment.NewLine, guid, num);
						}
					}
				}
				text = stringBuilder.ToString();
			}
			return text;
		}

		internal static void ReInitializeForUnitTest(int maxActiveAgents, bool multiTenancyEnabled)
		{
			lock (AgentInstanceController.syncRoot)
			{
				AgentInstanceController.instance = new AgentInstanceController(maxActiveAgents, multiTenancyEnabled);
			}
		}

		private static readonly object syncRoot = new object();

		private static AgentInstanceController instance;

		private readonly int totalSlots;

		private int freeSlots;

		private Dictionary<Guid, int> agents;

		private readonly bool multiTenancyEnabled;
	}
}
