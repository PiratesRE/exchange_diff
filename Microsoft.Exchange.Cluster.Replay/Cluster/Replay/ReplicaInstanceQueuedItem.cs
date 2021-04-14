using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ReplicaInstanceQueuedItem : ReplayQueuedItemBase
	{
		protected ReplicaInstanceQueuedItem(ReplicaInstance replicaInstance)
		{
			this.DbGuid = replicaInstance.Configuration.IdentityGuid;
			this.DbCopyName = replicaInstance.Configuration.DisplayName;
			this.DbName = replicaInstance.Configuration.DatabaseName;
		}

		public ReplicaInstance ReplicaInstance
		{
			get
			{
				if (this.m_instance == null)
				{
					ReplicaInstanceManager replicaInstanceManager = Dependencies.ReplayCoreManager.ReplicaInstanceManager;
					ReplicaInstance instance = null;
					if (!replicaInstanceManager.TryGetReplicaInstance(this.DbGuid, out instance))
					{
						Exception replicaInstanceNotFoundException = this.GetReplicaInstanceNotFoundException();
						throw replicaInstanceNotFoundException;
					}
					this.m_instance = instance;
				}
				return this.m_instance;
			}
			private set
			{
				this.m_instance = value;
			}
		}

		public Guid DbGuid { get; private set; }

		public string DbCopyName { get; private set; }

		public string DbName { get; private set; }

		public override bool IsEquivalentOrSuperset(IQueuedCallback otherCallback)
		{
			bool flag = base.IsEquivalentOrSuperset(otherCallback);
			if (!flag && otherCallback != null)
			{
				flag = (base.GetType() == otherCallback.GetType());
				if (flag)
				{
					ReplicaInstanceQueuedItem replicaInstanceQueuedItem = otherCallback as ReplicaInstanceQueuedItem;
					flag = this.DbGuid.Equals(replicaInstanceQueuedItem.DbGuid);
				}
			}
			return flag;
		}

		protected override void ExecuteInternal()
		{
			this.ReplicaInstance = null;
			this.CheckOperationApplicable();
		}

		protected abstract void CheckOperationApplicable();

		protected override Exception GetOperationCancelledException()
		{
			return new ReplayDatabaseOperationCancelledException(this.Name, this.DbCopyName);
		}

		protected override Exception GetOperationTimedoutException(TimeSpan timeout)
		{
			return new ReplayDatabaseOperationTimedoutException(this.Name, this.DbCopyName, timeout);
		}

		protected virtual Exception GetReplicaInstanceNotFoundException()
		{
			return new ReplayServiceUnknownReplicaInstanceException(this.Name, this.DbCopyName);
		}

		private ReplicaInstance m_instance;
	}
}
