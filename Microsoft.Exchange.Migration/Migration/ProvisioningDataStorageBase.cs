using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal abstract class ProvisioningDataStorageBase : IStepSettings, IMigrationSerializable
	{
		public ProvisioningDataStorageBase(MigrationUserRecipientType recipientType, bool isPAW = true)
		{
			this.RecipientType = recipientType;
			this.IsPAW = isPAW;
		}

		public abstract PropertyDefinition[] PropertyDefinitions { get; }

		public abstract bool ReadFromMessageItem(IMigrationStoreObject message);

		public abstract void WriteToMessageItem(IMigrationStoreObject message, bool loaded);

		public abstract ProvisioningDataStorageBase Clone();

		public abstract void UpdateFromDataRow(IMigrationDataRow dataRow);

		public abstract XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument);

		internal static ProvisioningDataStorageBase Create(MigrationType migrationType, MigrationUserRecipientType recipientType, bool isPAW)
		{
			switch (migrationType)
			{
			case MigrationType.XO1:
				MigrationUtil.AssertOrThrow(isPAW, "How do we have XO1 running on pre-PAW?!", new object[0]);
				return new XO1ProvisioningDataStorage(recipientType);
			case MigrationType.ExchangeOutlookAnywhere:
				return new ExchangeProvisioningDataStorage(recipientType, isPAW);
			}
			return null;
		}

		internal static ProvisioningDataStorageBase CreateFromMessage(IMigrationStoreObject message, MigrationType migrationType, MigrationUserRecipientType recipientType, bool isPAW = false)
		{
			ProvisioningDataStorageBase provisioningDataStorageBase = ProvisioningDataStorageBase.Create(migrationType, recipientType, isPAW);
			if (provisioningDataStorageBase != null && provisioningDataStorageBase.ReadFromMessageItem(message))
			{
				return provisioningDataStorageBase;
			}
			return null;
		}

		internal static ProvisioningDataStorageBase CreateFromDataRow(IMigrationDataRow dataRow, bool isPAW)
		{
			if (dataRow is InvalidDataRow)
			{
				return null;
			}
			MigrationPreexistingDataRow migrationPreexistingDataRow = dataRow as MigrationPreexistingDataRow;
			if (migrationPreexistingDataRow != null)
			{
				return migrationPreexistingDataRow.ProvisioningData;
			}
			ProvisioningDataStorageBase provisioningDataStorageBase = ProvisioningDataStorageBase.Create(dataRow.MigrationType, dataRow.RecipientType, isPAW);
			if (provisioningDataStorageBase == null)
			{
				return null;
			}
			provisioningDataStorageBase.UpdateFromDataRow(dataRow);
			return provisioningDataStorageBase;
		}

		internal static PropertyDefinition[] GetPropertyDefinitions(MigrationType migrationType)
		{
			switch (migrationType)
			{
			case MigrationType.XO1:
				return XO1ProvisioningDataStorage.XO1ProvisioningDataPropertyDefinitions;
			case MigrationType.ExchangeOutlookAnywhere:
				return ExchangeProvisioningDataStorage.ExchangeProvisioningDataPropertyDefinitions;
			}
			return null;
		}

		protected readonly MigrationUserRecipientType RecipientType;

		protected readonly bool IsPAW;
	}
}
