using System;
using System.Threading;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class RuntimeSettings : IRuntimeSettings
	{
		public RuntimeSettings(MExConfiguration config, string agentGroup, FactoryInitializer factoryInitializer)
		{
			AgentInfo[] enabledAgentsByType = config.GetEnabledAgentsByType(agentGroup);
			this.factoryTable = new FactoryTable(enabledAgentsByType, factoryInitializer);
			this.agentsInDefaultOrder = new AgentRecord[enabledAgentsByType.Length];
			this.monitoringOptions = config.MonitoringOptions;
			for (int i = 0; i < this.agentsInDefaultOrder.Length; i++)
			{
				AgentInfo agentInfo = enabledAgentsByType[i];
				this.agentsInDefaultOrder[i] = new AgentRecord(agentInfo.Id, agentInfo.AgentName, agentInfo.BaseTypeName, i, agentInfo.IsInternal);
			}
			string[] agents;
			string[][] eventTopics;
			AgentRecord[] array;
			RuntimeSettings.InitializeAgentsAndSubscriptions(config, agentGroup, false, out agents, out eventTopics, out array);
			this.agentSubscription = new AgentSubscription(agentGroup, agents, eventTopics);
			RuntimeSettings.InitializeAgentsAndSubscriptions(config, agentGroup, true, out agents, out eventTopics, out this.publicAgentsInDefaultOrder);
			this.disposeAgents = config.DisposeAgents;
		}

		public AgentRecord[] PublicAgentsInDefaultOrder
		{
			get
			{
				return this.publicAgentsInDefaultOrder;
			}
		}

		public bool DisposeAgents
		{
			get
			{
				return this.disposeAgents;
			}
		}

		public MonitoringOptions MonitoringOptions
		{
			get
			{
				return this.monitoringOptions;
			}
		}

		public FactoryTable AgentFactories
		{
			get
			{
				return this.factoryTable;
			}
		}

		public AgentSubscription AgentSubscription
		{
			get
			{
				return this.agentSubscription;
			}
		}

		public int AgentCount
		{
			get
			{
				return this.agentsInDefaultOrder.Length;
			}
		}

		public AgentRecord[] CreateDefaultAgentOrder()
		{
			AgentRecord[] array = new AgentRecord[this.agentsInDefaultOrder.Length];
			for (int i = 0; i < this.agentsInDefaultOrder.Length; i++)
			{
				array[i] = new AgentRecord(this.agentsInDefaultOrder[i].Id, this.agentsInDefaultOrder[i].Name, this.agentsInDefaultOrder[i].Type, this.agentsInDefaultOrder[i].SequenceNumber, this.agentsInDefaultOrder[i].IsInternal);
			}
			return array;
		}

		public void SaveAgentSubscription(AgentRecord[] agentRecords)
		{
			long ticks = DateTime.UtcNow.Ticks;
			long num = this.timeToSaveAgentSubscription;
			if (ticks <= num)
			{
				return;
			}
			long num2 = Interlocked.CompareExchange(ref this.timeToSaveAgentSubscription, DateTime.MaxValue.Ticks, num);
			if (num != num2)
			{
				return;
			}
			string[][] array = new string[agentRecords.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new string[agentRecords[i].Instance.Handlers.Count];
				int num3 = 0;
				foreach (object obj in agentRecords[i].Instance.Handlers.Keys)
				{
					string text = (string)obj;
					array[i][num3++] = text;
				}
			}
			this.agentSubscription.Update(array);
			this.agentSubscription.Save();
		}

		public void Shutdown()
		{
			if (this.sessionCount > 0)
			{
				throw new InvalidOperationException(MExRuntimeStrings.InvalidState);
			}
			this.factoryTable.Shutdown();
			if (this.agentSubscription != null)
			{
				this.agentSubscription.Dispose();
			}
		}

		public void AddSessionRef()
		{
			Interlocked.Increment(ref this.sessionCount);
		}

		public void ReleaseSessionRef()
		{
			Interlocked.Decrement(ref this.sessionCount);
		}

		public string GetAgentName(int agentSequenceNumber)
		{
			return this.agentsInDefaultOrder[agentSequenceNumber].Name;
		}

		private static void InitializeAgentsAndSubscriptions(MExConfiguration config, string agentGroup, bool publicAgentsOnly, out string[] agents, out string[][] subscriptions, out AgentRecord[] agentsInDefaultOrder)
		{
			AgentInfo[] array = publicAgentsOnly ? config.GetEnaledPublicAgentsByType(agentGroup) : config.GetEnabledAgentsByType(agentGroup);
			agents = new string[array.Length];
			agentsInDefaultOrder = new AgentRecord[array.Length];
			subscriptions = new string[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				AgentInfo agentInfo = array[i];
				agentsInDefaultOrder[i] = new AgentRecord(agentInfo.Id, agentInfo.AgentName, agentInfo.BaseTypeName, i, agentInfo.IsInternal);
				agents[i] = agentInfo.AgentName;
				subscriptions[i] = new string[0];
			}
		}

		private readonly MonitoringOptions monitoringOptions;

		private readonly FactoryTable factoryTable;

		private readonly AgentRecord[] agentsInDefaultOrder;

		private readonly AgentSubscription agentSubscription;

		private readonly bool disposeAgents;

		private readonly AgentRecord[] publicAgentsInDefaultOrder;

		private int sessionCount;

		private long timeToSaveAgentSubscription = DateTime.MinValue.Ticks;
	}
}
