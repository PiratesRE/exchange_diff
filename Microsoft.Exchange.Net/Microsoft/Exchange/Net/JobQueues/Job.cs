using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.JobQueues
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class Job
	{
		public Exception LastError { get; protected set; }

		public string ClientString { get; private set; }

		public bool IsShuttingdown
		{
			get
			{
				return this.queue.IsShuttingdown;
			}
		}

		public Job(JobQueue queue, Configuration config, string clientString)
		{
			if (queue == null)
			{
				throw new ArgumentNullException("queue");
			}
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			this.ClientString = clientString;
			this.queue = queue;
			this.Config = config;
		}

		public Configuration Config { get; private set; }

		public abstract void Begin(object state);

		protected virtual void End()
		{
			this.queue.OnJobCompletion(this);
		}

		protected readonly JobQueue queue;
	}
}
