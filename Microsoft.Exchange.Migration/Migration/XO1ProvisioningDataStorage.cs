using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal class XO1ProvisioningDataStorage : ProvisioningDataStorageBase
	{
		public XO1ProvisioningDataStorage(MigrationUserRecipientType recipientType) : base(recipientType, true)
		{
		}

		public long Puid { get; private set; }

		public string FirstName { get; private set; }

		public string LastName { get; private set; }

		public ExTimeZone TimeZone { get; private set; }

		public int LocaleId { get; private set; }

		public string[] EmailAddresses { get; private set; }

		public long AccountSize { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return XO1ProvisioningDataStorage.XO1ProvisioningDataPropertyDefinitions;
			}
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.Puid = message.GetValueOrDefault<long>(MigrationBatchMessageSchema.MigrationJobItemPuid, 0L);
			this.TimeZone = MigrationHelper.GetExTimeZoneProperty(message, MigrationBatchMessageSchema.MigrationJobItemTimeZone);
			this.LocaleId = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobItemLocaleId, 0);
			this.AccountSize = message.GetValueOrDefault<long>(MigrationBatchMessageSchema.MigrationJobItemAccountSize, 0L);
			string valueOrDefault = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemAliases, null);
			if (!string.IsNullOrEmpty(valueOrDefault))
			{
				this.EmailAddresses = valueOrDefault.Split(new char[]
				{
					'\u0001'
				});
			}
			this.FirstName = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemFirstName, null);
			this.LastName = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemLastName, null);
			return true;
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			message[MigrationBatchMessageSchema.MigrationJobItemPuid] = this.Puid;
			message[MigrationBatchMessageSchema.MigrationJobItemAccountSize] = this.AccountSize;
			message[MigrationBatchMessageSchema.MigrationJobItemLocaleId] = this.LocaleId;
			if (this.TimeZone != null)
			{
				message[MigrationBatchMessageSchema.MigrationJobItemTimeZone] = this.TimeZone.Id;
			}
			if (!string.IsNullOrEmpty(this.FirstName))
			{
				message[MigrationBatchMessageSchema.MigrationJobItemFirstName] = this.FirstName;
			}
			if (!string.IsNullOrEmpty(this.LastName))
			{
				message[MigrationBatchMessageSchema.MigrationJobItemLastName] = this.LastName;
			}
			if (this.EmailAddresses != null && this.EmailAddresses.Length > 0)
			{
				message[MigrationBatchMessageSchema.MigrationJobItemAliases] = string.Join(string.Empty + '\u0001', this.EmailAddresses);
			}
		}

		public override ProvisioningDataStorageBase Clone()
		{
			return new XO1ProvisioningDataStorage(this.RecipientType)
			{
				Puid = this.Puid,
				FirstName = this.FirstName,
				LastName = this.LastName,
				TimeZone = this.TimeZone,
				LocaleId = this.LocaleId,
				EmailAddresses = this.EmailAddresses,
				AccountSize = this.AccountSize
			};
		}

		public override void UpdateFromDataRow(IMigrationDataRow dataRow)
		{
			XO1MigrationDataRow xo1MigrationDataRow = dataRow as XO1MigrationDataRow;
			if (xo1MigrationDataRow == null)
			{
				throw new ArgumentException("expected a XO1MigrationDataRow", "dataRow");
			}
			this.Puid = xo1MigrationDataRow.Puid;
			this.FirstName = xo1MigrationDataRow.FirstName;
			this.LastName = xo1MigrationDataRow.LastName;
			this.TimeZone = xo1MigrationDataRow.TimeZone;
			this.LocaleId = xo1MigrationDataRow.LocaleId;
			this.EmailAddresses = xo1MigrationDataRow.EmailAddresses;
			this.AccountSize = xo1MigrationDataRow.AccountSize;
		}

		public override XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			return new XElement("XO1ProvisioningDataStorage", new object[]
			{
				new XElement("Puid", this.Puid),
				new XElement("FirstName", this.FirstName),
				new XElement("LastName", this.LastName),
				new XElement("TimeZone", this.TimeZone),
				new XElement("LocaleId", this.LocaleId),
				new XElement("EmailAddresses", string.Join(", ", this.EmailAddresses ?? new string[0])),
				new XElement("AccountSize", this.AccountSize)
			});
		}

		internal static readonly PropertyDefinition[] XO1ProvisioningDataPropertyDefinitions = new StorePropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationJobItemPuid,
			MigrationBatchMessageSchema.MigrationJobItemFirstName,
			MigrationBatchMessageSchema.MigrationJobItemLastName,
			MigrationBatchMessageSchema.MigrationJobItemTimeZone,
			MigrationBatchMessageSchema.MigrationJobItemLocaleId,
			MigrationBatchMessageSchema.MigrationJobItemAliases,
			MigrationBatchMessageSchema.MigrationJobItemAccountSize
		};
	}
}
