using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class ExchangeProvisioningDataStorage : ProvisioningDataStorageBase
	{
		public ExchangeProvisioningDataStorage(MigrationUserRecipientType recipientType, bool isPAW = true) : base(recipientType, isPAW)
		{
		}

		public ExchangeMigrationRecipient ExchangeRecipient { get; set; }

		public string EncryptedPassword { get; set; }

		public bool ForceChangePassword { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return ExchangeProvisioningDataStorage.ExchangeProvisioningDataPropertyDefinitions;
			}
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			try
			{
				this.ExchangeRecipient = ExchangeMigrationRecipient.Create(message, this.RecipientType, this.IsPAW);
			}
			catch (InvalidDataException)
			{
				return false;
			}
			this.EncryptedPassword = MigrationHelper.GetProperty<string>(message, MigrationBatchMessageSchema.MigrationJobItemExchangeMbxEncryptedPassword, false);
			this.ForceChangePassword = message.GetValueOrDefault<bool>(MigrationBatchMessageSchema.MigrationJobItemForceChangePassword, false);
			return true;
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			if (this.ExchangeRecipient != null)
			{
				this.ExchangeRecipient.WriteToMessageItem(message, loaded);
			}
			message[MigrationBatchMessageSchema.MigrationJobItemForceChangePassword] = this.ForceChangePassword;
			if (string.IsNullOrEmpty(this.EncryptedPassword))
			{
				message.Delete(MigrationBatchMessageSchema.MigrationJobItemExchangeMbxEncryptedPassword);
				return;
			}
			message[MigrationBatchMessageSchema.MigrationJobItemExchangeMbxEncryptedPassword] = this.EncryptedPassword;
		}

		public override ProvisioningDataStorageBase Clone()
		{
			return new ExchangeProvisioningDataStorage(this.RecipientType, this.IsPAW)
			{
				ExchangeRecipient = this.ExchangeRecipient,
				EncryptedPassword = this.EncryptedPassword,
				ForceChangePassword = this.ForceChangePassword
			};
		}

		public override void UpdateFromDataRow(IMigrationDataRow dataRow)
		{
			ExchangeMigrationDataRow exchangeMigrationDataRow = dataRow as ExchangeMigrationDataRow;
			if (exchangeMigrationDataRow == null)
			{
				throw new ArgumentException("expected a ExchangeMigrationDataRow", "dataRow");
			}
			this.EncryptedPassword = exchangeMigrationDataRow.EncryptedPassword;
			this.ForceChangePassword = exchangeMigrationDataRow.ForceChangePassword;
			NspiMigrationDataRow nspiMigrationDataRow = exchangeMigrationDataRow as NspiMigrationDataRow;
			if (nspiMigrationDataRow != null)
			{
				this.ExchangeRecipient = nspiMigrationDataRow.Recipient;
			}
		}

		public override XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("ExchangeProvisioningDataStorage", new object[]
			{
				new XElement("ForceChangePassword", this.ForceChangePassword),
				new XElement("EncryptedPasswordSet", !string.IsNullOrEmpty(this.EncryptedPassword))
			});
			if (this.ExchangeRecipient != null)
			{
				xelement.Add(this.ExchangeRecipient.GetDiagnosticInfo(dataProvider, argument));
			}
			return xelement;
		}

		internal static readonly PropertyDefinition[] ExchangeProvisioningDataPropertyDefinitions = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			ExchangeMigrationRecipient.ExchangeMigrationRecipientPropertyDefinitions,
			new StorePropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobItemExchangeMbxEncryptedPassword,
				MigrationBatchMessageSchema.MigrationJobItemForceChangePassword
			}
		});
	}
}
