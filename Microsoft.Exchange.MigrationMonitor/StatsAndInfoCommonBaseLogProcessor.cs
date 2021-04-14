using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal abstract class StatsAndInfoCommonBaseLogProcessor : BaseLogProcessor
	{
		protected StatsAndInfoCommonBaseLogProcessor(string logDirectoryPath, string logFileTypeName, BaseMigMonCsvSchema csvSchemaInstance, string logFileSearchPattern) : base(logDirectoryPath, logFileTypeName, csvSchemaInstance, logFileSearchPattern)
		{
			this.uploadedDatabases = new List<string>();
			this.CurrentDatabaseName = string.Empty;
		}

		protected string CurrentDatabaseName { get; set; }

		protected override bool AlwaysUploadLatestLog
		{
			get
			{
				return false;
			}
		}

		protected override bool AvoidAccessingLockedLogs
		{
			get
			{
				return true;
			}
		}

		protected override bool ValidateLogForUpload(List<CsvRow> allLogRows)
		{
			string columnName = this.CsvSchemaInstance.GetRequiredColumnsIds().First((ColumnDefinition<int> lookupCols) => lookupCols.KnownStringType == KnownStringType.DatabaseName).ColumnName;
			return this.ValidateLogForUpload(allLogRows, columnName);
		}

		protected bool TryAddDatabaseIdKeyValue(DataRow dataRow)
		{
			if (this.currentDatabaseId == 0)
			{
				return false;
			}
			ColumnDefinition<int> lookupColumnDefinition = MigMonUtilities.GetLookupColumnDefinition(this.CsvSchemaInstance.GetRequiredColumnsIds(), KnownStringType.DatabaseName);
			if (lookupColumnDefinition == null || lookupColumnDefinition.KnownStringType == KnownStringType.None)
			{
				return false;
			}
			string dataTableKeyColumnName = lookupColumnDefinition.DataTableKeyColumnName;
			dataRow[dataTableKeyColumnName] = this.currentDatabaseId;
			return (int)dataRow[dataTableKeyColumnName] != 0;
		}

		private bool ValidateLogRows(List<CsvRow> allLogRows, string databaseNameColumn, out string databaseName)
		{
			databaseName = null;
			if (allLogRows == null || allLogRows.Count <= 1)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Not enough log data to continue with uploading.", new object[0]);
				return false;
			}
			if (string.IsNullOrEmpty(databaseNameColumn))
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Database name log column is empty - cannot continue with uploading.", new object[0]);
				return false;
			}
			databaseName = (from lr in allLogRows
			where lr.Index != 0
			select lr into logRow
			select MigMonUtilities.GetColumnStringValue(logRow, databaseNameColumn)).FirstOrDefault<string>();
			if (string.IsNullOrWhiteSpace(databaseName))
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Unable to read database name - cannot continue with uploading.", new object[0]);
				return false;
			}
			if (this.uploadedDatabases.Contains(databaseName))
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "We already uploaded database stats for {0}, skip all other log files for the same database", new object[]
				{
					databaseName
				});
				return false;
			}
			this.uploadedDatabases.Add(databaseName);
			return true;
		}

		private bool ValidateLogForUpload(List<CsvRow> allLogRows, string databaseNameColumn)
		{
			this.CurrentDatabaseName = string.Empty;
			this.currentDatabaseId = 0;
			string text;
			if (!this.ValidateLogRows(allLogRows, databaseNameColumn, out text))
			{
				return false;
			}
			this.CurrentDatabaseName = text;
			int? valueFromIdMap = MigMonUtilities.GetValueFromIdMap(text, KnownStringType.DatabaseName, KnownStringsHelper.KnownStringToSqlLookupParam[KnownStringType.DatabaseName]);
			if (valueFromIdMap != null)
			{
				this.currentDatabaseId = valueFromIdMap.Value;
			}
			return true;
		}

		private readonly List<string> uploadedDatabases;

		private int currentDatabaseId;
	}
}
