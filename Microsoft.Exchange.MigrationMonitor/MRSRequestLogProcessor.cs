using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MRSRequestLogProcessor : BaseMrsMonitorLogProcessor
	{
		public MRSRequestLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MailboxReplicationService\\Requests"), "MRS Request", MigrationMonitor.MRSRequestCsvSchemaInstance)
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetRequestLogUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertMRSRequest_BatchUploadV5";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "MrsRequestList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "dbo.MrsRequestDataV5";
			}
		}

		protected override string[] DistinctColumns
		{
			get
			{
				return new string[]
				{
					"RequestGuid"
				};
			}
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			List<ColumnDefinition<int>> list = (from c in this.CsvSchemaInstance.GetOptionalColumnsIds()
			where c.KnownStringType != KnownStringType.TenantName
			select c).ToList<ColumnDefinition<int>>();
			list.ForEach(delegate(ColumnDefinition<int> oc)
			{
				this.TryAddStringValueByLookupId(oc, dataRow, row, null, true);
			});
			ColumnDefinition<int> lookupColumnDefinition = MigMonUtilities.GetLookupColumnDefinition(this.CsvSchemaInstance.GetOptionalColumnsIds(), KnownStringType.TenantName);
			if (lookupColumnDefinition == null || lookupColumnDefinition.KnownStringType == KnownStringType.None)
			{
				return true;
			}
			string columnName = lookupColumnDefinition.ColumnName;
			string dataTableKeyColumnName = lookupColumnDefinition.DataTableKeyColumnName;
			string columnStringValue = MigMonUtilities.GetColumnStringValue(row, columnName);
			if (!string.IsNullOrWhiteSpace(columnStringValue))
			{
				dataRow[dataTableKeyColumnName] = MigMonUtilities.GetTenantNameId(columnStringValue);
			}
			ColumnDefinition<int> columnDefinition = new ColumnDefinition<int>("RequestType", "RequestTypeId", KnownStringType.RequestType);
			base.TryAddStringValueByLookupId(columnDefinition, dataRow, row, null, true);
			if (!dataRow.Table.Columns.Contains(columnDefinition.DataTableKeyColumnName) || string.IsNullOrWhiteSpace(dataRow[columnDefinition.DataTableKeyColumnName].ToString()))
			{
				dataRow[columnDefinition.DataTableKeyColumnName] = MigMonUtilities.GetValueFromIdMap("Move", KnownStringType.RequestType, KnownStringsHelper.KnownStringToSqlLookupParam[KnownStringType.RequestType]);
			}
			return true;
		}

		private const string LogTypeName = "MRS Request";
	}
}
