using System;
using System.Data;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MRSFailureLogProcessor : BaseMrsMonitorLogProcessor
	{
		public MRSFailureLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MailboxReplicationService\\Failures"), "MRS Failure", MigrationMonitor.MRSFailureCsvSchemaInstance)
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetFailureLogUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertMRSFailure_BatchUploadV4";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "MrsFailureList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "dbo.MrsFailureDataV4";
			}
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			base.TryAddSimpleOptionalKnownStrings(dataRow, row);
			string text = MigMonUtilities.GetColumnStringValue(row, "OperationType");
			text = MigMonUtilities.TruncateMessage(text, 500);
			dataRow["OperationType"] = text;
			dataRow["StackTrace"] = DBNull.Value;
			dataRow[MRSFailureCsvSchema.WatsonHashColumn.DataTableKeyColumnName] = DBNull.Value;
			string columnStringValue = MigMonUtilities.GetColumnStringValue(row, MRSFailureCsvSchema.WatsonHashColumn.ColumnName);
			string columnStringValue2 = MigMonUtilities.GetColumnStringValue(row, "StackTrace");
			if (!string.IsNullOrWhiteSpace(columnStringValue) && !string.IsNullOrWhiteSpace(columnStringValue2))
			{
				int? watsonHashId = MigMonUtilities.GetWatsonHashId(columnStringValue, columnStringValue2, "MRS");
				if (watsonHashId != null)
				{
					dataRow[MRSFailureCsvSchema.WatsonHashColumn.DataTableKeyColumnName] = watsonHashId;
				}
			}
			string errorString = string.Format("Error parsing MRS failure log. Request guid is {0}", MigMonUtilities.GetColumnValue<Guid>(row, "RequestGuid"));
			return base.TryAddStringValueByLookupId(dataRow, row, KnownStringType.FailureType, errorString, false);
		}

		private const string LogTypeName = "MRS Failure";
	}
}
