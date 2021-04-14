using System;
using Microsoft.Exchange.Extensibility.EventLog;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class ExecutionMonitor
	{
		public ExecutionMonitor(int executionTimeLimit, Dispatcher dispatcher)
		{
			this.executionTimeLimit = ((executionTimeLimit > 0) ? executionTimeLimit : 0);
			if (executionTimeLimit > 0)
			{
				this.dispatcher = dispatcher;
				this.dispatcher.OnAgentInvokeStart += this.AgentInvokeStartEventHandler;
				this.dispatcher.OnAgentInvokeEnd += this.AgentInvokeEndEventHandler;
			}
		}

		public void Shutdown()
		{
			if (this.executionTimeLimit > 0 && this.dispatcher != null)
			{
				this.dispatcher.OnAgentInvokeStart -= this.AgentInvokeStartEventHandler;
				this.dispatcher.OnAgentInvokeEnd -= this.AgentInvokeEndEventHandler;
				this.dispatcher = null;
			}
		}

		internal void AgentInvokeStartEventHandler(object source, MExSession context)
		{
			this.agentName = context.ExecutingAgentName;
			this.eventTopic = context.OutstandingEventTopic;
			MailItem mailItem = context.CurrentAgent.Instance.MailItem;
			if (mailItem != null)
			{
				this.internetMessageId = mailItem.InternetMessageId;
			}
			this.agentInvocationTime = DateTime.UtcNow;
		}

		internal void AgentInvokeEndEventHandler(object source, MExSession context)
		{
			double totalMilliseconds = DateTime.UtcNow.Subtract(this.agentInvocationTime).TotalMilliseconds;
			if (totalMilliseconds > (double)this.executionTimeLimit && this.agentName != null && this.eventTopic != null)
			{
				MExDiagnostics.EventLog.LogEvent(EdgeExtensibilityEventLogConstants.Tuple_MExAgentTooSlow, null, new object[]
				{
					this.agentName,
					totalMilliseconds,
					this.eventTopic,
					this.internetMessageId ?? "Not Available"
				});
			}
			this.agentName = null;
			this.eventTopic = null;
			this.internetMessageId = null;
		}

		private readonly int executionTimeLimit;

		private Dispatcher dispatcher;

		private string agentName;

		private string eventTopic;

		private string internetMessageId;

		private DateTime agentInvocationTime;
	}
}
