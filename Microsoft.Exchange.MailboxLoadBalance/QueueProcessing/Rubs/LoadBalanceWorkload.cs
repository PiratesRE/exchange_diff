using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing.Rubs
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceWorkload : SystemWorkloadBase, IRequestQueueManager
	{
		public LoadBalanceWorkload(ILoadBalanceSettings settings)
		{
			this.queue = new RubsQueue(settings);
		}

		public override int BlockedTaskCount
		{
			get
			{
				return this.queue.BlockedTaskCount;
			}
		}

		public override string Id
		{
			get
			{
				return "Mailbox Load Balancing";
			}
		}

		public IRequestQueue MainProcessingQueue
		{
			get
			{
				return this.queue;
			}
		}

		public override int TaskCount
		{
			get
			{
				return this.queue.TaskCount;
			}
		}

		public override WorkloadType WorkloadType
		{
			get
			{
				return WorkloadType.MailboxReplicationServiceInternalMaintenance;
			}
		}

		public QueueManagerDiagnosticData GetDiagnosticData(bool includeRequestDetails, bool includeRequestVerboseData)
		{
			QueueDiagnosticData diagnosticData = this.queue.GetDiagnosticData(includeRequestDetails, includeRequestVerboseData);
			return new QueueManagerDiagnosticData
			{
				ProcessingQueues = new List<QueueDiagnosticData>
				{
					diagnosticData
				}
			};
		}

		public void Clean()
		{
			this.queue.Clean();
		}

		public IRequestQueue GetInjectionQueue(DirectoryDatabase database)
		{
			return this.queue;
		}

		public IRequestQueue GetProcessingQueue(LoadEntity loadObject)
		{
			return this.queue;
		}

		public IRequestQueue GetProcessingQueue(DirectoryObject directoryObject)
		{
			return this.queue;
		}

		protected override SystemTaskBase GetTask(ResourceReservationContext context)
		{
			return this.queue.GetTask(this, context);
		}

		private readonly RubsQueue queue;
	}
}
