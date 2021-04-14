using System;
using Microsoft.Exchange.Diagnostics.Components.MExRuntime;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class Dispatcher : IDispatcher
	{
		public Dispatcher(IRuntimeSettings settings, AgentRecord[] agentsInDefaultOrder, string parentName)
		{
			string text = string.IsNullOrEmpty(parentName) ? null : (parentName + ".");
			text += this.GetHashCode().ToString();
			this.InstanceNameFormatted = string.Format("[{0}][{1}] ", "MExDispatcher", text);
			this.settings = settings;
			this.clonedExecutionEntry = -1;
			this.executionChain = Dispatcher.BuildAgentList(agentsInDefaultOrder);
			this.watchdog = new ExecutionMonitor(this.settings.MonitoringOptions.AgentExecutionLimitInMilliseconds, this);
			ExTraceGlobals.InitializeTracer.Information((long)this.GetHashCode(), this.InstanceNameFormatted + "created");
		}

		public event AgentInvokeStartHandler OnAgentInvokeStart;

		public event AgentInvokeReturnsHandler OnAgentInvokeReturns;

		public event AgentInvokeEndHandler OnAgentInvokeEnd;

		public event AgentInvokeScheduledHandler OnAgentInvokeScheduled;

		public event AgentInvokeResumedHandler OnAgentInvokeResumed;

		public void Invoke(MExSession session)
		{
			string eventTopic = session.EventTopic;
			AgentRecord agentRecord;
			if (session.CurrentAgent == null)
			{
				agentRecord = this.executionChain;
				if (this.clonedTopic != null && this.clonedExecutionEntry >= 0 && this.clonedTopic == eventTopic)
				{
					ExTraceGlobals.DispatchTracer.TraceDebug<string, int>((long)this.GetHashCode(), this.InstanceNameFormatted + "restore clone state for '{0}' at index {1}", eventTopic, this.clonedExecutionEntry);
					for (int i = 0; i < this.clonedExecutionEntry; i++)
					{
						if (agentRecord == null)
						{
							ExTraceGlobals.DispatchTracer.TraceError<int>((long)this.GetHashCode(), this.InstanceNameFormatted + "restore clone state failed at index {0}", i);
							break;
						}
						agentRecord = agentRecord.Next;
					}
				}
			}
			else
			{
				agentRecord = session.NextAgent;
			}
			session.IsAsyncAgent = false;
			while (agentRecord != null)
			{
				session.CurrentAgent = agentRecord;
				if (agentRecord.Enabled)
				{
					Agent instance = agentRecord.Instance;
					instance.Session = session;
					instance.Synchronous = true;
					instance.EventTopic = eventTopic;
					Delegate @delegate = (Delegate)instance.Handlers[eventTopic];
					if (@delegate != null)
					{
						ExTraceGlobals.DispatchTracer.Information<string, string, string>((long)this.GetHashCode(), this.InstanceNameFormatted + "dispatching event '{0}' to {1} ({2})", eventTopic, instance.GetType().FullName, instance.GetHashCode().ToString());
						session.IsAsyncAgent = false;
						if (this.OnAgentInvokeStart != null)
						{
							this.OnAgentInvokeStart(this, session);
						}
						bool flag = true;
						try
						{
							session.StartCpuTracking(instance.Name);
							instance.Invoke(eventTopic, session.CurrentEventSource, session.CurrentEventArgs);
							flag = false;
						}
						finally
						{
							session.StopCpuTracking();
							if (flag)
							{
								session.NextAgent = null;
								session.ExecutionCompleted();
								session.CleanupCpuTracker();
								ExTraceGlobals.DispatchTracer.TraceDebug<string, string>((long)this.GetHashCode(), this.InstanceNameFormatted + "agent {0} crashed during event '{1}'", instance.GetType().FullName, eventTopic);
							}
						}
						ExTraceGlobals.DispatchTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), this.InstanceNameFormatted + "event '{0}' handled by {1} {2}synchronously", eventTopic, instance.GetType().FullName, session.IsAsyncAgent ? "a" : string.Empty);
						if (this.OnAgentInvokeReturns != null)
						{
							this.OnAgentInvokeReturns(this, session);
						}
						session.NextAgent = agentRecord.Next;
						if (session.IsStatusHalt)
						{
							this.AgentInvokeCompleted(session);
							session.NextAgent = null;
							break;
						}
						if (session.IsAsyncAgent)
						{
							if (!session.IsSyncInvoke)
							{
								ExTraceGlobals.DispatchTracer.Information((long)this.GetHashCode(), this.InstanceNameFormatted + "async agent pending");
								break;
							}
							session.Wait();
							this.AgentInvokeCompleted(session);
						}
						else
						{
							this.AgentInvokeCompleted(session);
						}
					}
					else if (this.settings.MonitoringOptions.MessageSnapshotEnabled)
					{
						instance.Invoke(eventTopic, session.CurrentEventSource, session.CurrentEventArgs);
					}
				}
				agentRecord = agentRecord.Next;
			}
			if (agentRecord == null || (session.NextAgent == null && !session.IsAsyncAgent))
			{
				ExTraceGlobals.DispatchTracer.TraceDebug<string>((long)this.GetHashCode(), this.InstanceNameFormatted + "dispatching of event '{0}' completed", eventTopic);
				session.ExecutionCompleted();
			}
		}

		public void AgentInvokeCompleted(MExSession session)
		{
			if (this.OnAgentInvokeEnd != null)
			{
				this.OnAgentInvokeEnd(this, session);
			}
		}

		public void AgentInvokeScheduled(MExSession session)
		{
			if (this.OnAgentInvokeScheduled != null)
			{
				this.OnAgentInvokeScheduled(this, session);
			}
		}

		public void AgentInvokeResumed(MExSession session)
		{
			if (this.OnAgentInvokeResumed != null)
			{
				this.OnAgentInvokeResumed(this, session);
			}
		}

		public void Shutdown()
		{
			ExTraceGlobals.ShutdownTracer.TraceDebug((long)this.GetHashCode(), this.InstanceNameFormatted + "shutdonw");
			this.watchdog.Shutdown();
		}

		public bool HasHandler(string eventTopic)
		{
			return this.executionChain != null;
		}

		public void SetCloneState(string eventTopic, int firstAgentIndex)
		{
			this.clonedTopic = eventTopic;
			this.clonedExecutionEntry = firstAgentIndex;
		}

		public int GetAgentIndex(AgentRecord agentEntry)
		{
			int num = 0;
			AgentRecord next = this.executionChain;
			while (next != null && next != agentEntry)
			{
				num++;
				next = next.Next;
			}
			ExTraceGlobals.DispatchTracer.Information<string, int, string>((long)this.GetHashCode(), this.InstanceNameFormatted + "agent '{0}' has index {1} ({2})", (agentEntry == null) ? "null" : agentEntry.Name, num, (agentEntry == null || next == null) ? "not found" : "found");
			return num;
		}

		private static AgentRecord BuildAgentList(AgentRecord[] agentsInDefaultOrder)
		{
			AgentRecord agentRecord = null;
			AgentRecord agentRecord2 = null;
			for (int i = 0; i < agentsInDefaultOrder.Length; i++)
			{
				AgentRecord agentRecord3 = (AgentRecord)agentsInDefaultOrder[i].Clone();
				if (agentRecord == null)
				{
					agentRecord = agentRecord3;
					agentRecord2 = agentRecord3;
				}
				else
				{
					agentRecord2.Next = agentRecord3;
					agentRecord2 = agentRecord3;
				}
			}
			return agentRecord;
		}

		private readonly string InstanceNameFormatted;

		private readonly AgentRecord executionChain;

		private IRuntimeSettings settings;

		private string clonedTopic;

		private int clonedExecutionEntry;

		private ExecutionMonitor watchdog;
	}
}
