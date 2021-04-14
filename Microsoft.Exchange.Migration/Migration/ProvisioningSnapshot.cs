using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ProvisioningSnapshot : IStepSnapshot
	{
		public ProvisioningSnapshot(ProvisionedObject provisionedObject)
		{
			this.Id = (ISnapshotId)provisionedObject.ItemId;
			if (provisionedObject.Succeeded)
			{
				this.Status = SnapshotStatus.Finalized;
			}
			else
			{
				this.Status = SnapshotStatus.Failed;
				this.ErrorMessage = new LocalizedString?(new LocalizedString(provisionedObject.Error ?? string.Empty));
			}
			this.MailboxData = provisionedObject.MailboxData;
			this.InjectionCompletedTime = provisionedObject.TimeFinished;
		}

		protected ProvisioningSnapshot()
		{
		}

		public ISnapshotId Id { get; private set; }

		public SnapshotStatus Status { get; set; }

		public LocalizedString? ErrorMessage { get; private set; }

		public IMailboxData MailboxData { get; private set; }

		public ExDateTime? InjectionCompletedTime { get; private set; }

		public static ProvisioningSnapshot CreateRemoved()
		{
			return new ProvisioningSnapshot
			{
				Status = SnapshotStatus.Removed
			};
		}

		public static ProvisioningSnapshot CreateInProgress(ISnapshotId id)
		{
			return new ProvisioningSnapshot
			{
				Id = id,
				Status = SnapshotStatus.InProgress
			};
		}

		public static ProvisioningSnapshot CreateCompleted(ISnapshotId id)
		{
			return new ProvisioningSnapshot
			{
				Id = id,
				Status = SnapshotStatus.Finalized
			};
		}

		internal static ProvisioningSnapshot CreateFromMessage(IMigrationStoreObject message, MigrationUserRecipientType recipientType)
		{
			if (recipientType != MigrationUserRecipientType.Group)
			{
				return null;
			}
			GroupProvisioningSnapshot groupProvisioningSnapshot = new GroupProvisioningSnapshot();
			groupProvisioningSnapshot.ReadFromMessageItem(message);
			return groupProvisioningSnapshot;
		}
	}
}
