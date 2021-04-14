using System;
using System.Threading;

namespace Microsoft.Exchange.Data.Transport
{
	public class AgentAsyncContext
	{
		internal AgentAsyncContext(Agent agent)
		{
			this.callback = agent.Session.GetAgentAsyncCallback();
			this.agent = agent;
			this.completed = 0;
			this.agent.Session.OnStartAsyncAgent();
		}

		public Exception AsyncException
		{
			get
			{
				return this.asyncException;
			}
			set
			{
				this.asyncException = value;
			}
		}

		public virtual void Resume()
		{
			this.agent.Session.ResumeAgent();
		}

		public virtual void Complete()
		{
			if (Interlocked.Increment(ref this.completed) != 1)
			{
				throw new InvalidOperationException(string.Format("Agent '{0}' ({1}) completed its asynchronous operation more than once while handling event '{2}'", this.agent.Name, this.agent.Id, this.agent.EventTopic));
			}
			if (this.agent.SnapshotWriter != null)
			{
				this.agent.SnapshotWriter.WriteProcessedData(this.agent.SnapshotPrefix, this.agent.EventArgId, this.agent.EventTopic, this.agent.Name, this.agent.MailItem);
			}
			this.agent.AsyncComplete();
			this.agent.MailItem = null;
			this.callback(this);
		}

		private Agent agent;

		private AgentAsyncCallback callback;

		private int completed;

		private Exception asyncException;
	}
}
