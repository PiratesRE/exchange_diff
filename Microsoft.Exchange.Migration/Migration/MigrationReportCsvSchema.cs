using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationReportCsvSchema : CsvSchema
	{
		public MigrationReportCsvSchema(bool includeProvisioning) : base(int.MaxValue, MigrationReportCsvSchema.requiredColumns.Value, MigrationReportCsvSchema.optionalColumns.Value, null)
		{
			if (includeProvisioning)
			{
				this.OrderedColumns = MigrationReportCsvSchema.AllColumns;
				return;
			}
			this.OrderedColumns = MigrationReportCsvSchema.MigrationColumns;
		}

		public string[] OrderedColumns { get; private set; }

		public void WriteHeader(StreamWriter streamWriter)
		{
			streamWriter.WriteCsvLine(this.OrderedColumns);
		}

		public void WriteRow(StreamWriter streamWriter, MigrationJobItem jobItem)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			List<string> list = new List<string>(this.OrderedColumns.Length);
			foreach (string key in this.OrderedColumns)
			{
				list.Add(MigrationReportCsvSchema.CellWriters[key](jobItem));
			}
			streamWriter.WriteCsvLine(list);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MigrationReportCsvSchema()
		{
			Dictionary<string, Func<MigrationJobItem, string>> dictionary = new Dictionary<string, Func<MigrationJobItem, string>>();
			dictionary.Add("Identifier", (MigrationJobItem ji) => ji.Identifier);
			dictionary.Add("Status", (MigrationJobItem ji) => ji.Status.ToString());
			dictionary.Add("ObjectType", (MigrationJobItem ji) => ji.RecipientType.ToString());
			dictionary.Add("ItemsMigrated", (MigrationJobItem ji) => ji.ItemsSynced.ToString(CultureInfo.CurrentCulture));
			dictionary.Add("ItemsSkipped", (MigrationJobItem ji) => ji.ItemsSkipped.ToString(CultureInfo.CurrentCulture));
			dictionary.Add("Password", delegate(MigrationJobItem ji)
			{
				if (ji.RecipientType == MigrationUserRecipientType.Mailbox || ji.RecipientType == MigrationUserRecipientType.Mailuser)
				{
					ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = ji.ProvisioningData as ExchangeProvisioningDataStorage;
					if (exchangeProvisioningDataStorage != null && exchangeProvisioningDataStorage.ExchangeRecipient != null && exchangeProvisioningDataStorage.ExchangeRecipient.DoesADObjectExist)
					{
						string clearString = "<" + Strings.PasswordPreviouslySet + ">";
						return MigrationServiceFactory.Instance.GetCryptoAdapter().ClearStringToEncryptedString(clearString);
					}
					if (exchangeProvisioningDataStorage != null)
					{
						return exchangeProvisioningDataStorage.EncryptedPassword;
					}
				}
				return LocalizedString.Empty;
			});
			dictionary.Add("ErrorMessage", (MigrationJobItem ji) => ji.LocalizedError ?? LocalizedString.Empty);
			MigrationReportCsvSchema.CellWriters = dictionary;
			MigrationReportCsvSchema.requiredColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(null), LazyThreadSafetyMode.ExecutionAndPublication);
			MigrationReportCsvSchema.optionalColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(MigrationReportCsvSchema.AllColumns), LazyThreadSafetyMode.ExecutionAndPublication);
		}

		public const string PasswordColumnName = "Password";

		private const string IdentifierColumnName = "Identifier";

		private const string StatusColumnName = "Status";

		private const string ItemsMigratedColumnName = "ItemsMigrated";

		private const string ItemsSkippedColumnName = "ItemsSkipped";

		private const string TypeColumnName = "ObjectType";

		public const string ErrorMessageColumnName = "ErrorMessage";

		private const int InternalMaximumRowCount = 2147483647;

		private static readonly string[] AllColumns = new string[]
		{
			"Identifier",
			"ObjectType",
			"Password",
			"Status",
			"ItemsMigrated",
			"ItemsSkipped",
			"ErrorMessage"
		};

		private static readonly string[] MigrationColumns = new string[]
		{
			"Identifier",
			"Status",
			"ItemsMigrated",
			"ItemsSkipped",
			"ErrorMessage"
		};

		private static readonly Dictionary<string, Func<MigrationJobItem, string>> CellWriters;

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> requiredColumns;

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> optionalColumns;
	}
}
