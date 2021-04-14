using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Transport.Sync.Worker.Throttling
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SyncTransportUserResourceMonitor : SyncResourceMonitor
	{
		public SyncTransportUserResourceMonitor(SyncLogSession syncLogSession, int maxPendingMessagesPerUser) : base(syncLogSession, null, SyncResourceMonitorType.UserTransportQueue)
		{
			this.maxPendingMessagesPerUser = maxPendingMessagesPerUser;
			this.CurrentPendingMessagesCountPerUser = new Dictionary<Guid, int>(20);
		}

		private protected Dictionary<Guid, int> CurrentPendingMessagesCountPerUser { protected get; private set; }

		internal void AddLoad(Guid userMailboxGuid, int newMessages)
		{
			int num;
			lock (this.syncLock)
			{
				this.CurrentPendingMessagesCountPerUser.TryGetValue(userMailboxGuid, out num);
				this.CurrentPendingMessagesCountPerUser[userMailboxGuid] = num + newMessages;
			}
			base.SyncLogSession.LogDebugging((TSLID)941UL, "Adding load for user {0} from prevLoad {1} by {2}", new object[]
			{
				userMailboxGuid,
				num,
				newMessages
			});
		}

		internal void RemoveLoad(Guid userMailboxGuid, int countOfMessagesNoLongerPending)
		{
			int num;
			lock (this.syncLock)
			{
				this.CurrentPendingMessagesCountPerUser.TryGetValue(userMailboxGuid, out num);
				int num2 = num - countOfMessagesNoLongerPending;
				num2 = Math.Max(0, num2);
				if (num2 > 0)
				{
					this.CurrentPendingMessagesCountPerUser[userMailboxGuid] = num2;
				}
				else
				{
					this.CurrentPendingMessagesCountPerUser.Remove(userMailboxGuid);
				}
			}
			base.SyncLogSession.LogDebugging((TSLID)1126UL, "Removing load for user {0} from prevLoad {1} by {2}", new object[]
			{
				userMailboxGuid,
				num,
				countOfMessagesNoLongerPending
			});
		}

		protected override ResourceLoad GetResourceLoad(AggregationWorkItem workItem)
		{
			int num;
			this.CurrentPendingMessagesCountPerUser.TryGetValue(workItem.UserMailboxGuid, out num);
			double num2 = (double)num / (double)this.maxPendingMessagesPerUser;
			if (num2 > 1.0)
			{
				num2 = ResourceLoad.Critical.LoadRatio;
			}
			return new ResourceLoad(num2, null, null);
		}

		protected override IResourceLoadMonitor CreateResourceHealthMonitor(ResourceKey resourceKey)
		{
			return null;
		}

		private readonly object syncLock = new object();

		private readonly int maxPendingMessagesPerUser;
	}
}
