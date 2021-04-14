using System;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class Agent
	{
		internal string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		internal bool Synchronous
		{
			get
			{
				return this.asynchronous == 0;
			}
			set
			{
				this.asynchronous = (value ? 0 : 1);
			}
		}

		internal string EventArgId
		{
			get
			{
				return this.eventArgId;
			}
			set
			{
				this.eventArgId = value;
			}
		}

		internal SnapshotWriter SnapshotWriter
		{
			get
			{
				return this.snapshotWriter;
			}
			set
			{
				this.snapshotWriter = value;
			}
		}

		internal bool SnapshotEnabled
		{
			get
			{
				return this.snapshotEnabled;
			}
			set
			{
				this.snapshotEnabled = value;
			}
		}

		internal IExecutionControl Session
		{
			get
			{
				return this.session;
			}
			set
			{
				this.session = value;
			}
		}

		internal IDictionary Handlers
		{
			get
			{
				return this.handlers;
			}
		}

		internal object HostStateInternal
		{
			get
			{
				return this.hostState;
			}
			set
			{
				this.hostState = value;
			}
		}

		internal virtual object HostState
		{
			get
			{
				return this.hostState;
			}
			set
			{
				this.hostState = value;
			}
		}

		internal virtual string SnapshotPrefix
		{
			get
			{
				return string.Empty;
			}
		}

		internal MailItem MailItem
		{
			get
			{
				return this.mailItem;
			}
			set
			{
				this.mailItem = value;
			}
		}

		protected internal string Name
		{
			get
			{
				return this.name;
			}
			internal set
			{
				this.name = value;
			}
		}

		protected internal string EventTopic
		{
			get
			{
				return this.topic;
			}
			internal set
			{
				this.topic = value;
			}
		}

		internal void AddHandler(string eventTopic, Delegate handler)
		{
			try
			{
				this.handlers.Add(eventTopic, handler);
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidOperationException(string.Format("A transport agent has attempted to subscribe to the same event - {0} - more than once", eventTopic), innerException);
			}
		}

		internal void RemoveHandler(string eventTopic)
		{
			try
			{
				this.handlers.Remove(eventTopic);
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidOperationException(string.Format("A transport agent has attempted to subscribe to the same event - {0} - more than once", eventTopic), innerException);
			}
		}

		internal abstract void Invoke(string eventTopic, object source, object e);

		internal virtual void AsyncComplete()
		{
		}

		internal void EnsureMimeWriteStreamClosed()
		{
			if (!this.mailItem.MimeWriteStreamOpen)
			{
				return;
			}
			this.mailItem.RestoreLastSavedMime(this.name, this.topic);
			this.mailItem.Recipients.Clear();
		}

		protected AgentAsyncContext GetAgentAsyncContext()
		{
			if (Interlocked.Increment(ref this.asynchronous) != 1)
			{
				throw new InvalidOperationException(string.Format("Agent '{0}' ({1}) tried to acquire its asynchronous context object more than once while handling event '{2}'", this.name, this.id, this.topic));
			}
			return new AgentAsyncContext(this);
		}

		private string id;

		private string name;

		private string topic;

		private int asynchronous;

		private string eventArgId;

		private SnapshotWriter snapshotWriter;

		private bool snapshotEnabled;

		private IExecutionControl session;

		private object hostState;

		private HybridDictionary handlers = new HybridDictionary();

		private MailItem mailItem;
	}
}
