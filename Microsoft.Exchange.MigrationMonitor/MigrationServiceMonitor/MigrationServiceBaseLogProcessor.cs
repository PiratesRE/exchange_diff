using System;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor.MigrationServiceMonitor
{
	internal abstract class MigrationServiceBaseLogProcessor : BaseLogProcessor
	{
		protected MigrationServiceBaseLogProcessor(string logDirectoryPath, string logFileTypeName, BaseMigMonCsvSchema csvSchemaInstance) : base(logDirectoryPath, logFileTypeName, csvSchemaInstance, null)
		{
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			return this.TryAddMigrationServiceCommonDataRowValues(dataRow, row);
		}

		protected bool TryAddMigrationServiceCommonDataRowValues(DataRow dataRow, CsvRow row)
		{
			base.TryAddSimpleOptionalKnownStrings(dataRow, row);
			dataRow["InternalError"] = DBNull.Value;
			dataRow["LocalizedError"] = DBNull.Value;
			dataRow[MigrationServiceProcessorsCommonHelpers.TenantName.DataTableKeyColumnName] = DBNull.Value;
			dataRow[MigrationServiceProcessorsCommonHelpers.WatsonHash.DataTableKeyColumnName] = DBNull.Value;
			string columnStringValue = MigMonUtilities.GetColumnStringValue(row, "LocalizedError");
			string columnStringValue2 = MigMonUtilities.GetColumnStringValue(row, "InternalError");
			string columnStringValue3 = MigMonUtilities.GetColumnStringValue(row, MigrationServiceProcessorsCommonHelpers.WatsonHash.ColumnName);
			if (!string.IsNullOrWhiteSpace(columnStringValue3))
			{
				dataRow[MigrationServiceProcessorsCommonHelpers.WatsonHash.DataTableKeyColumnName] = MigMonUtilities.GetWatsonHashId(columnStringValue3, columnStringValue + Environment.NewLine + columnStringValue2, "MigrationService");
			}
			string columnStringValue4 = MigMonUtilities.GetColumnStringValue(row, MigrationServiceProcessorsCommonHelpers.TenantName.ColumnName);
			if (!string.IsNullOrWhiteSpace(columnStringValue4))
			{
				dataRow[MigrationServiceProcessorsCommonHelpers.TenantName.DataTableKeyColumnName] = MigMonUtilities.GetTenantNameId(columnStringValue4);
			}
			return true;
		}

		public new const string KeyNameIsLogProcessorEnabled = "IsMigServiceStatsLogProcessorEnabled";
	}
}
