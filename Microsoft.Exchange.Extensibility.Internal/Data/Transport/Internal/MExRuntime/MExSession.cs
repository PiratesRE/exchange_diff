using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Delivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MExRuntime;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Extensibility.EventLog;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class MExSession : IMExSession, IExecutionControl, ICloneable, IDisposeTrackable, IDisposable
	{
		internal MExSession(IRuntimeSettings settings, ICloneableInternal state, string name, string parentName) : this(settings, state, name, parentName, null, null, null)
		{
		}

		internal MExSession(IRuntimeSettings settings, ICloneableInternal state, string name, string parentName, Action startAsyncAgentCallback, Action completeAsyncAgentCallback, Func<bool> resumeAgentCallback) : this(settings, state, name, parentName, null, null, startAsyncAgentCallback, completeAsyncAgentCallback, resumeAgentCallback)
		{
		}

		private MExSession(IRuntimeSettings settings, ICloneableInternal state, string name, string parentName, AgentRecord[] agentsInDefaultOrder, AgentRecord[] publicAgentsInDefaultOrder, Action startAsyncAgentCallback, Action completeAsyncAgentCallback, Func<bool> resumeAgentCallback)
		{
			settings.AddSessionRef();
			this.settings = settings;
			this.hostState = state;
			this.name = name;
			this.parentName = parentName;
			this.id = this.GetHashCode().ToString(CultureInfo.InvariantCulture);
			string str = string.IsNullOrEmpty(parentName) ? null : (parentName + ".");
			string arg = str + this.id;
			this.InstanceNameFormatted = string.Format("[{0}][{1}] ", "MExSession", arg);
			this.isCompleted = true;
			if (agentsInDefaultOrder == null)
			{
				agentsInDefaultOrder = this.settings.CreateDefaultAgentOrder();
				publicAgentsInDefaultOrder = new AgentRecord[this.settings.PublicAgentsInDefaultOrder.Length];
				int num = 0;
				for (int i = 0; i < agentsInDefaultOrder.Length; i++)
				{
					agentsInDefaultOrder[i].Instance = this.CreateAgentInstance(agentsInDefaultOrder[i]);
					agentsInDefaultOrder[i].Enabled = this.IsAgentEnabled(agentsInDefaultOrder[i]);
					if (!agentsInDefaultOrder[i].IsInternal)
					{
						publicAgentsInDefaultOrder[num] = agentsInDefaultOrder[i];
						num++;
					}
				}
			}
			if (ExTraceGlobals.DispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				foreach (AgentRecord agentRecord in agentsInDefaultOrder)
				{
					if (agentRecord.Enabled)
					{
						ExTraceGlobals.DispatchTracer.TraceDebug<string, string>((long)this.GetHashCode(), this.InstanceNameFormatted + "{0} ({1})", agentRecord.Instance.GetType().FullName, agentRecord.Instance.Id);
					}
				}
			}
			this.agentsInDefaultOrder = agentsInDefaultOrder;
			this.publicAgentsInDefaultOrder = publicAgentsInDefaultOrder;
			this.startAsyncAgentCallback = startAsyncAgentCallback;
			this.completeAsyncAgentCallback = completeAsyncAgentCallback;
			this.resumeAgentCallback = resumeAgentCallback;
			this.scheduleWorkDelegate = new MExSession.ScheduleWork(ThreadPool.QueueUserWorkItem);
			this.dispatcher = new Dispatcher(this.settings, this.agentsInDefaultOrder, arg);
			this.disposeTracker = DisposeTracker.Get<MExSession>(this);
			ExTraceGlobals.DispatchTracer.TraceDebug<string>((long)this.GetHashCode(), this.InstanceNameFormatted + "created '{0}'", this.Name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public object CurrentEventSource
		{
			get
			{
				return this.currentEventSource;
			}
		}

		public object CurrentEventArgs
		{
			get
			{
				return this.currentEventArgs;
			}
		}

		public string ExecutingAgentName
		{
			get
			{
				if (this.currentAgent != null)
				{
					return this.currentAgent.Name;
				}
				return null;
			}
		}

		public string OutstandingEventTopic
		{
			get
			{
				if (this.currentAgent != null)
				{
					return this.currentAgent.Instance.EventTopic;
				}
				return null;
			}
		}

		public long TotalProcessorTime
		{
			get
			{
				return (long)this.cpuTracker.TotalProcessorTime.TotalMilliseconds;
			}
		}

		public IDispatcher Dispatcher
		{
			get
			{
				return this.dispatcher;
			}
		}

		public AgentRecord CurrentAgent
		{
			get
			{
				return this.currentAgent;
			}
			set
			{
				this.currentAgent = value;
				if (value != null)
				{
					if (string.IsNullOrEmpty(value.Name))
					{
						ExTraceGlobals.DispatchTracer.TraceDebug((long)this.GetHashCode(), "Agent name was not specified, using type instead.");
						this.lastAgentName = value.Type;
						return;
					}
					this.lastAgentName = value.Name;
				}
			}
		}

		public string LastAgentName
		{
			get
			{
				return this.lastAgentName;
			}
		}

		public string EventTopic
		{
			get
			{
				return this.eventTopic;
			}
		}

		internal bool IsAsyncAgent
		{
			get
			{
				return this.isAsyncAgent;
			}
			set
			{
				this.isAsyncAgent = value;
			}
		}

		internal bool IsExecutionCompleted
		{
			get
			{
				return this.isCompleted;
			}
		}

		internal long BeginInvokeTicks
		{
			get
			{
				return this.beginInvokeTicks;
			}
			set
			{
				this.beginInvokeTicks = value;
			}
		}

		internal long BeginSchedulingTicks
		{
			get
			{
				return this.beginSchedulingTicks;
			}
			set
			{
				this.beginSchedulingTicks = value;
			}
		}

		internal MExAsyncResult AsyncResult
		{
			get
			{
				return this.pendingResult;
			}
		}

		internal bool IsSyncInvoke
		{
			get
			{
				return this.isSyncInvoke;
			}
		}

		internal AgentRecord NextAgent
		{
			get
			{
				return this.nextAgent;
			}
			set
			{
				this.nextAgent = value;
			}
		}

		internal bool IsStatusHalt
		{
			get
			{
				return this.isStatusHalt;
			}
			set
			{
				this.isStatusHalt = value;
			}
		}

		public void StartCpuTracking(string agentName)
		{
			this.cpuTracker = CpuTracker.StartCpuTracking(agentName);
		}

		public void StopCpuTracking()
		{
			this.cpuTracker.End();
		}

		public void CleanupCpuTracker()
		{
			this.cpuTracker = null;
		}

		public void Invoke(string topic, object source, object e)
		{
			if (topic == null)
			{
				throw new ArgumentNullException("topic");
			}
			ExTraceGlobals.DispatchTracer.TraceDebug<string>((long)this.GetHashCode(), this.InstanceNameFormatted + "sync dispatch event {0}", topic);
			if (this.closed)
			{
				throw new InvalidOperationException(MExRuntimeStrings.InvalidState);
			}
			if (!this.dispatcher.HasHandler(topic))
			{
				ExTraceGlobals.DispatchTracer.TraceDebug<string>((long)this.GetHashCode(), this.InstanceNameFormatted + "no handler for {0}", topic);
				return;
			}
			lock (this.syncRoot)
			{
				try
				{
					if (this.closed)
					{
						throw new InvalidOperationException(MExRuntimeStrings.InvalidState);
					}
					this.BindContexts(source, e);
					this.eventTopic = topic;
					this.Invoke();
				}
				finally
				{
					this.UnbindContexts();
				}
			}
		}

		public IAsyncResult BeginInvoke(string topic, object source, object e, AsyncCallback callback, object callbackState)
		{
			if (topic == null)
			{
				throw new ArgumentNullException("topic");
			}
			ExTraceGlobals.DispatchTracer.TraceDebug<string, string>((long)this.GetHashCode(), this.InstanceNameFormatted + "async dispatch event {0} {1} callback", topic, (callback == null) ? "without" : "with");
			if (this.closed)
			{
				throw new InvalidOperationException(MExRuntimeStrings.InvalidState);
			}
			MExAsyncResult result = new MExAsyncResult(callback, callbackState);
			if (!this.dispatcher.HasHandler(topic))
			{
				ExTraceGlobals.DispatchTracer.TraceDebug<string>((long)this.GetHashCode(), this.InstanceNameFormatted + "no handler for {0}", topic);
				this.pendingResult = result;
				this.pendingResult.InvokeCompleted();
			}
			else
			{
				lock (this.syncRoot)
				{
					if (this.closed)
					{
						throw new InvalidOperationException(MExRuntimeStrings.InvalidState);
					}
					this.BindContexts(source, e);
					this.eventTopic = topic;
					this.pendingResult = result;
					this.AsyncInvoke(null);
				}
			}
			return result;
		}

		public void EndInvoke(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (asyncResult != this.pendingResult)
			{
				throw new ArgumentException(MExRuntimeStrings.InvalidEndInvoke, "asyncResult");
			}
			ExTraceGlobals.DispatchTracer.TraceDebug<int>((long)this.GetHashCode(), this.InstanceNameFormatted + "async dispatch ended, async result {0}", asyncResult.GetHashCode());
			try
			{
				this.pendingResult.EndInvoke();
			}
			finally
			{
				this.pendingResult = null;
			}
		}

		public void HaltExecution()
		{
			if (this.closed)
			{
				throw new InvalidOperationException(MExRuntimeStrings.InvalidState);
			}
			this.IsStatusHalt = true;
		}

		public void OnStartAsyncAgent()
		{
			if (this.startAsyncAgentCallback != null)
			{
				this.startAsyncAgentCallback();
			}
		}

		public void ResumeAgent()
		{
			if (this.resumeAgentCallback != null)
			{
				this.resumeAgentCallback();
			}
		}

		public AgentAsyncCallback GetAgentAsyncCallback()
		{
			if (this.closed)
			{
				throw new InvalidOperationException(MExRuntimeStrings.InvalidState);
			}
			return this.GetAsyncCallbackHandler();
		}

		public object Clone()
		{
			int agentIndex;
			MExSession mexSession;
			lock (this.syncRoot)
			{
				if (this.closed)
				{
					throw new InvalidOperationException(MExRuntimeStrings.InvalidState);
				}
				agentIndex = this.dispatcher.GetAgentIndex(this.currentAgent);
				mexSession = new MExSession(this.settings, this.hostState, this.name, this.parentName, this.agentsInDefaultOrder, this.publicAgentsInDefaultOrder, this.startAsyncAgentCallback, this.completeAsyncAgentCallback, this.resumeAgentCallback);
				mexSession.SetCloneState(this.eventTopic, agentIndex);
			}
			ExTraceGlobals.DispatchTracer.TraceDebug<string, string, int>((long)this.GetHashCode(), this.InstanceNameFormatted + "{0} cloned on event '{1}' with index {2}", mexSession.GetHashCode().ToString(CultureInfo.InvariantCulture), this.eventTopic, agentIndex);
			return mexSession;
		}

		public void Close()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			if (!this.closed && !this.IsExecutionCompleted)
			{
				throw new InvalidOperationException(MExRuntimeStrings.InvalidState);
			}
			lock (this.syncRoot)
			{
				if (this.closed)
				{
					return;
				}
				this.closed = true;
			}
			if (this.syncWaitHandle != null)
			{
				this.syncWaitHandle.Close();
			}
			if (this.dispatcher != null)
			{
				this.dispatcher.Shutdown();
			}
			this.settings.SaveAgentSubscription(this.agentsInDefaultOrder);
			foreach (AgentRecord agentRecord in this.agentsInDefaultOrder)
			{
				Agent instance = agentRecord.Instance;
				if (instance != null)
				{
					instance.Id = null;
				}
			}
			this.DisposeAgents(this.settings.DisposeAgents);
			this.agentsInDefaultOrder = null;
			this.publicAgentsInDefaultOrder = null;
			this.currentEventSource = null;
			this.currentEventArgs = null;
			this.dispatcher = null;
			this.hostState = null;
			this.nextAgent = null;
			this.currentAgent = null;
			this.pendingResult = null;
			this.syncWaitHandle = null;
			this.settings.ReleaseSessionRef();
			this.settings = null;
			ExTraceGlobals.DispatchTracer.TraceDebug<string>((long)this.GetHashCode(), this.InstanceNameFormatted + "closed '{0}'", this.Name);
		}

		private void DisposeAgents(bool disposeAllAgents)
		{
			foreach (AgentRecord agentRecord in this.agentsInDefaultOrder)
			{
				Agent instance = agentRecord.Instance;
				if (instance != null && (agentRecord.IsInternal || disposeAllAgents))
				{
					IDisposable disposable = instance as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		internal void SetCloneState(string topic, int firstAgentIndex)
		{
			this.dispatcher.SetCloneState(topic, firstAgentIndex);
		}

		internal void Wait()
		{
			if (this.isSyncInvoke)
			{
				this.syncWaitHandle.WaitOne();
				this.syncWaitHandle.Reset();
			}
		}

		internal void ExecutionCompleted()
		{
			this.isCompleted = true;
		}

		private void Invoke()
		{
			ExTraceGlobals.DispatchTracer.TraceDebug<string>((long)this.GetHashCode(), this.InstanceNameFormatted + "dispatching event {0}", this.eventTopic);
			this.dispatcher.Invoke(this);
		}

		private void ResumeExecution()
		{
			ExTraceGlobals.DispatchTracer.TraceDebug<bool>((long)this.GetHashCode(), this.InstanceNameFormatted + "execution resumed ({0})", this.IsStatusHalt);
			if (this.IsStatusHalt)
			{
				this.NextAgent = null;
			}
			this.dispatcher.AgentInvokeScheduled(this);
			if (!this.scheduleWorkDelegate(new WaitCallback(this.ResumeExecutionWork)))
			{
				ExTraceGlobals.DispatchTracer.TraceError((long)this.GetHashCode(), this.InstanceNameFormatted + "scheduling workItem failed");
				throw new InsufficientMemoryException(MExRuntimeStrings.TooManyInvokes);
			}
		}

		private void ResumeExecutionWork(object state)
		{
			this.ResumeAgent();
			this.dispatcher.AgentInvokeResumed(this);
			this.AsyncInvoke(state);
		}

		private void AsyncInvoke(object state)
		{
			lock (this.syncRoot)
			{
				if (this.pendingResult == null)
				{
					MExDiagnostics.EventLog.LogEvent(EdgeExtensibilityEventLogConstants.Tuple_MExAgentCompletedTwice, null, new object[]
					{
						this.lastAgentName,
						this.eventTopic
					});
					throw new InvalidOperationException(string.Format("{0} agent is invoking async completion twice", this.lastAgentName));
				}
				ExTraceGlobals.DispatchTracer.TraceDebug<int>((long)this.GetHashCode(), this.InstanceNameFormatted + "async result {0}", this.pendingResult.GetHashCode());
				this.isSyncInvoke = false;
				try
				{
					this.Invoke();
				}
				catch (IOException e)
				{
					this.HandleLeakedException(e);
				}
				catch (LocalizedException e2)
				{
					this.HandleLeakedException(e2);
				}
				if (this.IsExecutionCompleted && this.pendingResult != null)
				{
					ExTraceGlobals.DispatchTracer.TraceDebug<bool>((long)this.GetHashCode(), this.InstanceNameFormatted + "async dispatch done: {0}", this.IsStatusHalt);
					this.UnbindContexts();
					this.pendingResult.InvokeCompleted();
				}
				else
				{
					ExTraceGlobals.DispatchTracer.TraceDebug<bool>((long)this.GetHashCode(), this.InstanceNameFormatted + "async dispatch pending: {0}", this.IsStatusHalt);
				}
			}
		}

		private void BindContexts(object source, object e)
		{
			if (!this.isCompleted)
			{
				ExTraceGlobals.DispatchTracer.TraceError((long)this.GetHashCode(), this.InstanceNameFormatted + "concurrent invoke within a session");
				throw new InvalidOperationException(MExRuntimeStrings.InvalidConcurrentInvoke);
			}
			this.isStatusHalt = false;
			this.isCompleted = false;
			this.currentAgent = null;
			this.nextAgent = null;
			this.currentEventSource = source;
			this.currentEventArgs = e;
		}

		private void UnbindContexts()
		{
			this.currentEventSource = null;
			this.currentEventArgs = null;
			this.currentAgent = null;
			this.nextAgent = null;
		}

		private AgentAsyncCallback GetAsyncCallbackHandler()
		{
			this.isAsyncAgent = true;
			if (this.isSyncInvoke)
			{
				if (this.syncWaitHandle == null)
				{
					this.syncWaitHandle = new ManualResetEvent(false);
				}
			}
			else
			{
				this.pendingResult.SetAsync();
			}
			ExTraceGlobals.DispatchTracer.TraceDebug((long)this.GetHashCode(), this.InstanceNameFormatted + "going async");
			return new AgentAsyncCallback(this.AgentAsyncCompletionCallback);
		}

		private void AgentAsyncCompletionCallback(AgentAsyncContext context)
		{
			if (this.resumeAgentCallback != null && this.resumeAgentCallback())
			{
				MExDiagnostics.EventLog.LogEvent(EdgeExtensibilityEventLogConstants.Tuple_MExAgentDidNotCallResume, this.lastAgentName, new object[]
				{
					this.lastAgentName,
					this.eventTopic
				});
			}
			ExTraceGlobals.DispatchTracer.TraceDebug<string, string>((long)this.GetHashCode(), this.InstanceNameFormatted + "async completed, async result {0}, exception {1}", this.isSyncInvoke ? "n/a" : this.pendingResult.GetHashCode().ToString(CultureInfo.InvariantCulture), (context.AsyncException == null) ? "n/a" : context.AsyncException.GetType().FullName);
			if (this.isSyncInvoke)
			{
				if (context.AsyncException != null)
				{
					MExAsyncResult.WrapAndRethrowException(context.AsyncException, new LocalizedString(MExRuntimeStrings.AgentFault(this.currentAgent.Name, this.eventTopic)));
				}
				this.syncWaitHandle.Set();
				return;
			}
			if (context.AsyncException != null)
			{
				this.pendingResult.AsyncException = context.AsyncException;
				this.HaltExecution();
				MExSession.LogMexAgentFaultEvent(MExDiagnostics.EventLog, context.AsyncException, this.currentAgent.Name, this.eventTopic);
			}
			if (this.completeAsyncAgentCallback != null)
			{
				this.completeAsyncAgentCallback();
			}
			this.Dispatcher.AgentInvokeCompleted(this);
			this.ResumeExecution();
		}

		internal static void LogMexAgentFaultEvent(ExEventLog eventLog, Exception exception, string agentName, string eventTopic)
		{
			ArgumentValidator.ThrowIfNull("eventLog", eventLog);
			ArgumentValidator.ThrowIfNull("exception", exception);
			string text = exception.GetType().Name;
			if (text == "SmtpResponseException")
			{
				return;
			}
			eventLog.LogEvent(EdgeExtensibilityEventLogConstants.Tuple_MExAgentFault, agentName + "," + text, new object[]
			{
				agentName,
				text,
				exception.Message,
				eventTopic
			});
		}

		private Agent CreateAgentInstance(AgentRecord agentRecord)
		{
			AgentFactory agentFactory = this.settings.AgentFactories[agentRecord.Id];
			object state = (this.hostState == null) ? null : this.hostState.Clone();
			Agent agent;
			try
			{
				agent = agentFactory.CreateAgent(agentRecord.Type, state);
			}
			catch (LocalizedException ex)
			{
				MExDiagnostics.EventLog.LogEvent(EdgeExtensibilityEventLogConstants.Tuple_MExAgentInstanceCreationFailure, null, new object[]
				{
					agentRecord.Name,
					ex.Message
				});
				throw;
			}
			if (agent == null || agent.Id != null)
			{
				string error = (agent == null) ? "agent instance cannot be null" : "agent instance already in use";
				ApplicationException ex2 = new ApplicationException(MExRuntimeStrings.AgentCreationFailure(agentRecord.Name, error));
				MExDiagnostics.EventLog.LogEvent(EdgeExtensibilityEventLogConstants.Tuple_MExAgentInstanceCreationFailure, null, new object[]
				{
					agentRecord.Name,
					ex2.Message
				});
				throw ex2;
			}
			agent.Id = agent.GetHashCode().ToString(CultureInfo.InvariantCulture);
			agent.Name = agentRecord.Name;
			agent.SnapshotEnabled = this.settings.MonitoringOptions.MessageSnapshotEnabled;
			agent.HostState = state;
			ExTraceGlobals.DispatchTracer.Information<string, string>((long)this.GetHashCode(), this.InstanceNameFormatted + "agent '{0}' created from factory '{1}'", agent.GetType().FullName, agentFactory.GetType().FullName);
			return agent;
		}

		private bool IsAgentEnabled(AgentRecord agentRecord)
		{
			if (string.Equals(agentRecord.Type, MExSession.DeliveryAgentType, StringComparison.OrdinalIgnoreCase))
			{
				DeliveryAgentManager deliveryAgentManager = (DeliveryAgentManager)this.settings.AgentFactories.GetAgentManager(agentRecord.Id);
				if (!string.Equals(deliveryAgentManager.SupportedDeliveryProtocol, this.name, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
			return true;
		}

		private void HandleLeakedException(Exception e)
		{
			this.pendingResult.EventTopic = this.eventTopic;
			this.pendingResult.FaultyAgentName = this.currentAgent.Name;
			this.pendingResult.AsyncException = e;
			ExTraceGlobals.DispatchTracer.TraceError((long)this.GetHashCode(), this.InstanceNameFormatted + e);
			MExSession.LogMexAgentFaultEvent(MExDiagnostics.EventLog, e, this.currentAgent.Name, this.eventTopic);
		}

		public void Dispose()
		{
			this.Close();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return this.disposeTracker;
		}

		public void SuppressDisposeTracker()
		{
			this.disposeTracker.Suppress();
		}

		private static readonly string DeliveryAgentType = typeof(DeliveryAgent).ToString();

		private readonly object syncRoot = new object();

		private readonly string InstanceNameFormatted;

		private readonly string name;

		private readonly string parentName;

		private readonly string id;

		private bool closed;

		private ICloneableInternal hostState;

		private IRuntimeSettings settings;

		private Dispatcher dispatcher;

		private AgentRecord[] agentsInDefaultOrder;

		private AgentRecord[] publicAgentsInDefaultOrder;

		private object currentEventSource;

		private object currentEventArgs;

		private readonly Func<bool> resumeAgentCallback;

		private readonly Action startAsyncAgentCallback;

		private readonly Action completeAsyncAgentCallback;

		private readonly DisposeTracker disposeTracker;

		private CpuTracker cpuTracker;

		private readonly MExSession.ScheduleWork scheduleWorkDelegate;

		private volatile bool isCompleted;

		private bool isStatusHalt;

		private AgentRecord nextAgent;

		private AgentRecord currentAgent;

		private string lastAgentName;

		private string eventTopic;

		private ManualResetEvent syncWaitHandle;

		private bool isSyncInvoke = true;

		private MExAsyncResult pendingResult;

		private long beginInvokeTicks;

		private long beginSchedulingTicks;

		private bool isAsyncAgent;

		public delegate bool ScheduleWork(WaitCallback callback);
	}
}
