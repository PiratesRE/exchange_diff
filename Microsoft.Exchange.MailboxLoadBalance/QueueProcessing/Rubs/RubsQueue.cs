using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Logging;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing.Rubs
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RubsQueue : IRequestQueue
	{
		public RubsQueue(ILoadBalanceSettings settings)
		{
			this.settings = settings;
		}

		public int BlockedTaskCount { get; private set; }

		public Guid Id
		{
			get
			{
				return Guid.Empty;
			}
		}

		public int TaskCount
		{
			get
			{
				return this.requests.Count;
			}
		}

		public void Clean()
		{
			lock (this.requestQueueLock)
			{
				this.requests.Clear();
			}
		}

		public void EnqueueRequest(IRequest request)
		{
			lock (this.requestQueueLock)
			{
				request.AssignQueue(this);
				this.requests.Add(request);
			}
		}

		public QueueDiagnosticData GetDiagnosticData(bool includeRequestDetails, bool includeRequestVerboseDiagnostics)
		{
			QueueDiagnosticData queueDiagnosticData = new QueueDiagnosticData
			{
				QueueGuid = Guid.Empty,
				QueueLength = this.requests.Count,
				IsActive = true
			};
			queueDiagnosticData.CurrentRequest = null;
			if (includeRequestDetails)
			{
				lock (this.requestQueueLock)
				{
					queueDiagnosticData.Requests = (from request in this.requests
					select request.GetDiagnosticData(includeRequestVerboseDiagnostics)).ToList<RequestDiagnosticData>();
				}
			}
			return queueDiagnosticData;
		}

		public SystemTaskBase GetTask(LoadBalanceWorkload workload, ResourceReservationContext context)
		{
			lock (this.requestQueueLock)
			{
				this.BlockedTaskCount = 0;
				if (this.requests.Count == 0)
				{
					return null;
				}
				for (int i = 0; i < this.requests.Count; i++)
				{
					IRequest request = this.requests[i];
					if (request.IsBlocked)
					{
						this.BlockedTaskCount++;
					}
					else if (request.ShouldCancel(this.settings.IdleRunDelay))
					{
						request.Abort();
						DatabaseRequestLog.Write(request);
						this.requests.RemoveAt(i);
						i--;
					}
					else
					{
						ResourceKey obj2;
						ResourceReservation reservation = context.GetReservation(workload, request.Resources, out obj2);
						if (reservation != null)
						{
							this.requests.RemoveAt(i);
							return new LoadBalanceTask(workload, reservation, request);
						}
						if (ProcessorResourceKey.Local.Equals(obj2))
						{
							this.BlockedTaskCount = this.requests.Count;
							break;
						}
						this.BlockedTaskCount++;
					}
				}
			}
			return null;
		}

		private readonly object requestQueueLock = new object();

		private readonly List<IRequest> requests = new List<IRequest>();

		private readonly ILoadBalanceSettings settings;
	}
}
