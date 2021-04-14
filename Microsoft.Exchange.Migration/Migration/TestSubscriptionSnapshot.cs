using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class TestSubscriptionSnapshot : SubscriptionSnapshot, ISerializable
	{
		public TestSubscriptionSnapshot(SerializationInfo info, StreamingContext context)
		{
			this.Deserialize(info);
		}

		public TestSubscriptionSnapshot(Guid? id, SnapshotStatus status, bool isInitialSyncComplete, ExDateTime createTime, ExDateTime? lastUpdateTime, ExDateTime? lastSyncTime, LocalizedString? errorMessage, string batchName) : base(null, status, isInitialSyncComplete, createTime, lastUpdateTime, lastSyncTime, errorMessage, batchName)
		{
			this.Id = id;
		}

		private TestSubscriptionSnapshot()
		{
		}

		public new Guid? Id
		{
			get
			{
				if (base.Id == null)
				{
					return null;
				}
				return new Guid?(((MRSSubscriptionId)base.Id).Id);
			}
			private set
			{
				Guid? guid = value;
				if (guid == null)
				{
					base.Id = null;
					return;
				}
				MailboxData mailboxData = new MailboxData(guid.Value, "legDN", new ADObjectId(guid.Value), guid.Value);
				mailboxData.Update("identifier", OrganizationId.ForestWideOrgId);
				ISubscriptionId id = new MRSSubscriptionId(guid.Value, MigrationType.ExchangeOutlookAnywhere, mailboxData);
				base.Id = id;
			}
		}

		public new bool IsInitialSyncComplete
		{
			get
			{
				return base.IsInitialSyncComplete;
			}
			set
			{
				base.IsInitialSyncComplete = value;
			}
		}

		public new SnapshotStatus Status
		{
			get
			{
				return base.Status;
			}
			set
			{
				base.Status = value;
			}
		}

		public new long NumItemsSkipped
		{
			get
			{
				return base.NumItemsSkipped;
			}
			set
			{
				base.NumItemsSkipped = value;
			}
		}

		public new long NumItemsSynced
		{
			get
			{
				return base.NumItemsSynced;
			}
			set
			{
				base.NumItemsSynced = value;
			}
		}

		public new static TestSubscriptionSnapshot CreateFailed(LocalizedString errorMessage)
		{
			return new TestSubscriptionSnapshot
			{
				Status = SnapshotStatus.Failed,
				ErrorMessage = new LocalizedString?(errorMessage)
			};
		}

		public static TestSubscriptionSnapshot CreateId(Guid id)
		{
			return new TestSubscriptionSnapshot
			{
				Id = new Guid?(id),
				Status = SnapshotStatus.InProgress
			};
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.Serialize(info);
		}

		private void Deserialize(SerializationInfo info)
		{
			this.Id = (Guid?)info.GetValue("Id", typeof(Guid?));
			this.Status = (SnapshotStatus)info.GetUInt32("Status");
			this.IsInitialSyncComplete = info.GetBoolean("IsInitialSyncComplete");
			base.CreateTime = (ExDateTime)info.GetDateTime("CreateTime");
			DateTime? dateTime = (DateTime?)info.GetValue("LastUpdateTime", typeof(DateTime?));
			if (dateTime != null)
			{
				base.LastUpdateTime = new ExDateTime?((ExDateTime)dateTime.Value);
			}
			DateTime? dateTime2 = (DateTime?)info.GetValue("LastSyncTime", typeof(DateTime?));
			if (dateTime2 != null)
			{
				base.LastSyncTime = new ExDateTime?((ExDateTime)dateTime2.Value);
			}
			base.ErrorMessage = (LocalizedString?)info.GetValue("ErrorMessage", typeof(LocalizedString?));
			this.NumItemsSkipped = info.GetInt64("NumItemsSkipped");
			this.NumItemsSynced = info.GetInt64("NumItemsSynced");
		}

		private void Serialize(SerializationInfo info)
		{
			info.AddValue("Id", this.Id, typeof(Guid?));
			info.AddValue("Status", (uint)this.Status);
			info.AddValue("IsInitialSyncComplete", this.IsInitialSyncComplete);
			info.AddValue("CreateTime", (DateTime)base.CreateTime);
			info.AddValue("LastUpdateTime", (DateTime?)base.LastUpdateTime);
			info.AddValue("LastSyncTime", (DateTime?)base.LastSyncTime);
			info.AddValue("ErrorMessage", base.ErrorMessage, typeof(LocalizedString?));
			info.AddValue("NumItemsSkipped", this.NumItemsSkipped);
			info.AddValue("NumItemsSynced", this.NumItemsSynced);
		}
	}
}
