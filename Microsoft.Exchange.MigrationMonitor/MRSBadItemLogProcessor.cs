using System;
using System.Data;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MRSBadItemLogProcessor : BaseMrsMonitorLogProcessor
	{
		public MRSBadItemLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MailboxReplicationService\\BadItems"), "MRS Bad Item", MigrationMonitor.MRSBadItemCsvSchemaInstance)
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetBadItemLogUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertMRSBadItem_BatchUpload";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "MrsBadItemList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "dbo.MrsBadItemData";
			}
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			string empty = string.Empty;
			base.TryAddStringValueByLookupId(dataRow, row, KnownStringType.BadItemKind, empty, true);
			base.TryAddStringValueByLookupId(dataRow, row, KnownStringType.BadItemWkfTypeId, empty, true);
			base.TryAddStringValueByLookupId(dataRow, row, KnownStringType.BadItemMessageClass, empty, true);
			base.TryAddStringValueByLookupId(dataRow, row, KnownStringType.BadItemCategory, empty, true);
			string columnStringValue = MigMonUtilities.GetColumnStringValue(row, "BadItemKind");
			string columnStringValue2 = MigMonUtilities.GetColumnStringValue(row, "WKFType");
			string columnStringValue3 = MigMonUtilities.GetColumnStringValue(row, "MessageClass");
			string columnStringValue4 = MigMonUtilities.GetColumnStringValue(row, "Category");
			dataRow["BadItemKind"] = columnStringValue;
			dataRow["WKFType"] = columnStringValue2;
			dataRow["MessageClass"] = columnStringValue3;
			dataRow["Category"] = columnStringValue4;
			string text = MigMonUtilities.GetColumnStringValue(row, "FailureMessage");
			text = MigMonUtilities.TruncateMessage(text, 500);
			dataRow["FailureMessage"] = text;
			return true;
		}

		private const string LogTypeName = "MRS Bad Item";
	}
}
