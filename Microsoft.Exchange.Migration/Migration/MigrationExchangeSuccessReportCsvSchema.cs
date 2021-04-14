using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationExchangeSuccessReportCsvSchema : ReportCsvSchema
	{
		public MigrationExchangeSuccessReportCsvSchema(bool isCompletionReport) : base(int.MaxValue, MigrationExchangeSuccessReportCsvSchema.requiredColumns.Value, MigrationExchangeSuccessReportCsvSchema.optionalColumns.Value, null)
		{
			this.isCompletionReport = isCompletionReport;
		}

		private string[] Headers
		{
			get
			{
				if (this.isCompletionReport)
				{
					return MigrationExchangeSuccessReportCsvSchema.CompletionHeaders;
				}
				return MigrationExchangeSuccessReportCsvSchema.FinalizationHeaders;
			}
		}

		public override void WriteHeader(StreamWriter streamWriter)
		{
			streamWriter.WriteCsvLine(this.Headers);
		}

		public override void WriteRow(StreamWriter streamWriter, MigrationJobItem jobItem)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			List<string> list = new List<string>(this.Headers.Length);
			list.Add(jobItem.Identifier);
			list.Add(jobItem.RecipientType.ToString());
			if (this.isCompletionReport)
			{
				ExchangeProvisioningDataStorage exchangeProvisioningDataStorage = (ExchangeProvisioningDataStorage)jobItem.ProvisioningData;
				string item = exchangeProvisioningDataStorage.EncryptedPassword;
				if ((jobItem.RecipientType == MigrationUserRecipientType.Mailbox || jobItem.RecipientType == MigrationUserRecipientType.Mailuser) && exchangeProvisioningDataStorage.ExchangeRecipient.DoesADObjectExist)
				{
					string clearString = "<" + Strings.PasswordPreviouslySet + ">";
					item = MigrationServiceFactory.Instance.GetCryptoAdapter().ClearStringToEncryptedString(clearString);
				}
				list.Add(item);
			}
			if (jobItem.ItemsSkipped != 0L)
			{
				list.Add(ServerStrings.MigrationStatisticsPartiallyCompleteStatus);
			}
			else
			{
				list.Add(ServerStrings.MigrationStatisticsCompleteStatus);
			}
			list.Add(jobItem.ItemsSynced.ToString(CultureInfo.CurrentCulture));
			list.Add(jobItem.ItemsSkipped.ToString(CultureInfo.CurrentCulture));
			streamWriter.WriteCsvLine(list);
		}

		private static readonly string[] RequiredColumnNames = new string[]
		{
			"EmailAddress",
			"ObjectType",
			"Status",
			"ItemsMigrated",
			"ItemsSkipped"
		};

		private static readonly string[] OptionalColumnNames = new string[]
		{
			"Password",
			"AdditionalComments"
		};

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> optionalColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(MigrationExchangeSuccessReportCsvSchema.OptionalColumnNames), LazyThreadSafetyMode.ExecutionAndPublication);

		private static Lazy<Dictionary<string, ProviderPropertyDefinition>> requiredColumns = new Lazy<Dictionary<string, ProviderPropertyDefinition>>(() => MigrationCsvSchemaBase.GenerateDefaultColumnInfo(MigrationExchangeSuccessReportCsvSchema.RequiredColumnNames), LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly string[] FinalizationHeaders = new string[]
		{
			"EmailAddress",
			"ObjectType",
			"Status",
			"ItemsMigrated",
			"ItemsSkipped"
		};

		private static readonly string[] CompletionHeaders = new string[]
		{
			"EmailAddress",
			"ObjectType",
			"Password",
			"Status",
			"ItemsMigrated",
			"ItemsSkipped"
		};

		private readonly bool isCompletionReport;
	}
}
