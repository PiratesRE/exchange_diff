using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ScheduledRequest : IRequest
	{
		public ScheduledRequest(IRequest request, DateTime firstExecution, Func<TimeSpan> periodGetter)
		{
			this.periodGetter = periodGetter;
			this.request = request;
			this.nextExecution = firstExecution;
		}

		public bool IsBlocked
		{
			get
			{
				return TimeProvider.UtcNow < this.nextExecution || this.request.IsBlocked;
			}
		}

		public IRequestQueue Queue
		{
			get
			{
				return this.request.Queue;
			}
		}

		public IEnumerable<ResourceKey> Resources
		{
			get
			{
				return this.request.Resources;
			}
		}

		public void Abort()
		{
			this.request.Abort();
		}

		public void AssignQueue(IRequestQueue queue)
		{
			this.request.AssignQueue(queue);
		}

		public RequestDiagnosticData GetDiagnosticData(bool verbose)
		{
			return this.request.GetDiagnosticData(verbose);
		}

		public void Process()
		{
			this.request.Process();
			TimeSpan timeSpan = this.periodGetter();
			if (timeSpan > TimeSpan.Zero)
			{
				this.nextExecution = TimeProvider.UtcNow.Add(timeSpan);
				this.Queue.EnqueueRequest(this);
			}
		}

		public bool ShouldCancel(TimeSpan queueTimeout)
		{
			return false;
		}

		public bool WaitExecution()
		{
			return this.request.WaitExecution();
		}

		public bool WaitExecution(TimeSpan timeout)
		{
			return this.request.WaitExecution(timeout);
		}

		public bool WaitExecutionAndThrowOnFailure(TimeSpan timeout)
		{
			return this.request.WaitExecutionAndThrowOnFailure(timeout);
		}

		private readonly Func<TimeSpan> periodGetter;

		private readonly IRequest request;

		private DateTime nextExecution;
	}
}
