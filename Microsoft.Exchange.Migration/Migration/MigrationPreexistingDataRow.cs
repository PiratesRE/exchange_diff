using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationPreexistingDataRow : IMigrationDataRow
	{
		internal MigrationPreexistingDataRow(int rowIndex, MigrationJobItem originalJobItem)
		{
			this.CursorPosition = rowIndex;
			this.Identifier = originalJobItem.Identifier;
			this.MigrationType = originalJobItem.MigrationType;
			this.RecipientType = originalJobItem.RecipientType;
			this.RemoteIdentifier = originalJobItem.RemoteIdentifier;
			this.LocalMailboxIdentifier = originalJobItem.LocalMailboxIdentifier;
			if (originalJobItem.ProvisioningData != null)
			{
				this.ProvisioningData = originalJobItem.ProvisioningData.Clone();
			}
			if (originalJobItem.SubscriptionSettings != null)
			{
				this.SubscriptionSettings = ((JobItemSubscriptionSettingsBase)originalJobItem.SubscriptionSettings).Clone();
			}
			this.SubscriptionId = originalJobItem.SubscriptionId;
		}

		public ISubscriptionSettings SubscriptionSettings { get; private set; }

		public ISubscriptionId SubscriptionId { get; private set; }

		public ProvisioningDataStorageBase ProvisioningData { get; private set; }

		public MigrationType MigrationType { get; private set; }

		public MigrationUserRecipientType RecipientType { get; private set; }

		public int CursorPosition { get; internal set; }

		public string Identifier { get; private set; }

		public string LocalMailboxIdentifier { get; private set; }

		public string RemoteIdentifier { get; private set; }

		public bool SupportsRemoteIdentifier
		{
			get
			{
				return this.RemoteIdentifier != null;
			}
		}
	}
}
