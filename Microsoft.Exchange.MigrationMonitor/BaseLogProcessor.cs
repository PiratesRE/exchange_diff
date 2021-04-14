using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal abstract class BaseLogProcessor
	{
		protected BaseLogProcessor(string logDirectoryPath, string logFileTypeName, BaseMigMonCsvSchema csvSchemaInstance, string logFileSearchPattern = null)
		{
			this.logDirectoryPath = logDirectoryPath;
			this.logFileTypeName = logFileTypeName;
			this.CsvSchemaInstance = csvSchemaInstance;
			this.logFileSearchPattern = (string.IsNullOrEmpty(logFileSearchPattern) ? "*.log" : logFileSearchPattern);
		}

		public string LogFileTypeName
		{
			get
			{
				return this.logFileTypeName;
			}
		}

		protected abstract string StoredProcNameToGetLastUpdateTimeStamp { get; }

		protected abstract string SqlSprocNameToHandleUpload { get; }

		protected abstract string SqlParamName { get; }

		protected abstract string SqlTypeName { get; }

		protected virtual bool AlwaysUploadLatestLog
		{
			get
			{
				return true;
			}
		}

		protected virtual bool AvoidAccessingLockedLogs
		{
			get
			{
				return false;
			}
		}

		protected virtual string[] DistinctColumns
		{
			get
			{
				return null;
			}
		}

		public void ProcessLogs()
		{
			string[] logFiles = this.GetLogFiles();
			if (logFiles == null)
			{
				return;
			}
			DateTime logLastUpdateTS = this.GetLogLastUpdateTS();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Processing all files modified after {0}", new object[]
			{
				logLastUpdateTS.ToString()
			});
			string[] array = logFiles;
			int i = 0;
			while (i < array.Length)
			{
				string text = array[i];
				DateTime t = DateTime.UtcNow;
				try
				{
					t = File.GetLastWriteTime(text);
				}
				catch (IOException ex)
				{
					this.LogExceptionAndSendWatson("Error accessing log file for last write time", text, ex);
					goto IL_173;
				}
				goto IL_83;
				IL_173:
				i++;
				continue;
				IL_83:
				if ((num3 <= 1 && this.AlwaysUploadLatestLog) || !(t <= logLastUpdateTS))
				{
					num3++;
					MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Processing file {0}", new object[]
					{
						Path.GetFileName(text)
					});
					List<CsvRow> allLogRows;
					try
					{
						using (FileStream fileStream = this.OpenFileToRead(text, this.AvoidAccessingLockedLogs))
						{
							allLogRows = this.CsvSchemaInstance.Read(fileStream, null, false, false).Reverse<CsvRow>().ToList<CsvRow>();
						}
					}
					catch (CsvValidationException ex2)
					{
						this.LogExceptionAndSendWatson("Invalid CSV format.", text, ex2);
						goto IL_173;
					}
					int num4;
					int num5;
					this.InsertLogs(allLogRows, logLastUpdateTS, out num4, out num5);
					num += num4;
					num2 += num5;
					MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Finished processing file {0}. File Stats: Records inserted/updated: {1}; Records skipped due to errors: {2}.", new object[]
					{
						Path.GetFileName(text),
						num4,
						num5
					});
					goto IL_173;
				}
				break;
			}
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Finished processing {0} logs. Files processed: {1}; Records inserted/updated: {2}; Records skipped due to errors: {3}.", new object[]
			{
				this.logFileTypeName,
				num3,
				num,
				num2
			});
		}

		protected abstract void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex);

		protected abstract bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row);

		protected abstract void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row);

		protected virtual bool ValidateLogForUpload(List<CsvRow> allLogRows)
		{
			return true;
		}

		protected virtual void AddValuesInDataTable(DataTable table, List<CsvRow> allLogRows, DateTime lastLogUpdateTs, out int recordsInFile, out int errorsInFile)
		{
			recordsInFile = 0;
			errorsInFile = 0;
			foreach (CsvRow csvRow in from logRow in allLogRows
			where logRow.Index != 0
			select logRow)
			{
				DateTime columnValue = MigMonUtilities.GetColumnValue<DateTime>(csvRow, this.CsvSchemaInstance.TimeStampColumnName);
				if (!(columnValue <= lastLogUpdateTs))
				{
					recordsInFile++;
					DataRow dataRow = table.NewRow();
					this.CsvSchemaInstance.GetRequiredColumnsIds().ForEach(delegate(ColumnDefinition<int> rc)
					{
						BaseLogProcessor.InitialInitDataRowLookups(rc.DataTableKeyColumnName, dataRow);
					});
					this.CsvSchemaInstance.GetOptionalColumnsIds().ForEach(delegate(ColumnDefinition<int> oc)
					{
						BaseLogProcessor.InitialInitDataRowLookups(oc.DataTableKeyColumnName, dataRow);
					});
					if (!this.HandleInsertCsvRowExceptions(new Action<DataRow, CsvRow>(this.InsertCsvRowInDataTable), dataRow, csvRow))
					{
						errorsInFile++;
					}
					else if (!this.TryAddSchemaSpecificDataRowValues(dataRow, csvRow))
					{
						errorsInFile++;
					}
					else
					{
						table.Rows.Add(dataRow);
					}
				}
			}
		}

		protected void AddCommonDataRowValues(DataRow dataRow, CsvRow row)
		{
			dataRow[this.CsvSchemaInstance.TimeStampColumnName] = MigMonUtilities.GetColumnValue<SqlDateTime>(row, this.CsvSchemaInstance.TimeStampColumnName);
			dataRow["LoggingServerId"] = MigMonUtilities.GetLocalServerId();
		}

		protected bool TryAddStringValueByLookupId(DataRow dataRow, CsvRow row, KnownStringType knownStringType, string errorString, bool isOptional = true)
		{
			ColumnDefinition<int> lookupColumnDefinition = MigMonUtilities.GetLookupColumnDefinition(isOptional ? this.CsvSchemaInstance.GetOptionalColumnsIds() : this.CsvSchemaInstance.GetRequiredColumnsIds(), knownStringType);
			return this.TryAddStringValueByLookupId(lookupColumnDefinition, dataRow, row, errorString, isOptional);
		}

		protected bool TryAddStringValueByLookupId(ColumnDefinition<int> columnDefinition, DataRow dataRow, CsvRow row, string errorString, bool isOptional = true)
		{
			if (columnDefinition == null || columnDefinition.KnownStringType == KnownStringType.None)
			{
				return isOptional;
			}
			string columnName = columnDefinition.ColumnName;
			string dataTableKeyColumnName = columnDefinition.DataTableKeyColumnName;
			KnownStringType knownStringType = columnDefinition.KnownStringType;
			string convertedRowString = columnDefinition.GetConvertedRowString(row);
			if (string.IsNullOrWhiteSpace(convertedRowString) && !isOptional)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, errorString ?? string.Empty, new object[0]);
				return false;
			}
			int? valueFromIdMap = MigMonUtilities.GetValueFromIdMap(convertedRowString, knownStringType, KnownStringsHelper.KnownStringToSqlLookupParam[knownStringType]);
			dataRow[dataTableKeyColumnName] = ((valueFromIdMap != null) ? valueFromIdMap.Value : DBNull.Value);
			return isOptional || (!(dataRow[dataTableKeyColumnName] is DBNull) && (int)dataRow[dataTableKeyColumnName] != 0);
		}

		protected void TryAddSimpleOptionalKnownStrings(DataRow dataRow, CsvRow row)
		{
			List<ColumnDefinition<int>> list = (from c in this.CsvSchemaInstance.GetOptionalColumnsIds()
			where !KnownStringsHelper.SpecialKnownStrings.Contains(c.KnownStringType)
			select c).ToList<ColumnDefinition<int>>();
			list.ForEach(delegate(ColumnDefinition<int> oc)
			{
				this.TryAddStringValueByLookupId(oc, dataRow, row, null, true);
			});
		}

		protected bool HandleInsertCsvRowExceptions(Action<DataRow, CsvRow> insertAction, DataRow dataRow, CsvRow row)
		{
			bool result;
			try
			{
				insertAction(dataRow, row);
				result = true;
			}
			catch (FormatException ex)
			{
				this.LogInsertCsvRowHandlerError(ex, row);
				result = false;
			}
			return result;
		}

		private static void InitialInitDataRowLookups(string rowKey, DataRow dataRow)
		{
			dataRow[rowKey] = 0;
		}

		private void InsertCsvRowInDataTable(DataRow dataRow, CsvRow row)
		{
			this.AddCommonDataRowValues(dataRow, row);
			this.InsertValuesInDataRow(dataRow, row);
		}

		private void InsertValuesInDataRow(DataRow dataRow, CsvRow row)
		{
			foreach (IColumnDefinition columnDefinition in this.CsvSchemaInstance.GetRequiredColumnsAsIs())
			{
				columnDefinition.InsertColumnValueInDataRow(dataRow, row);
			}
			foreach (IColumnDefinition columnDefinition2 in this.CsvSchemaInstance.GetOptionalColumnsAsIs())
			{
				columnDefinition2.InsertColumnValueInDataRow(dataRow, row);
			}
		}

		private void InsertLogs(List<CsvRow> allLogRows, DateTime lastLogUpdateTs, out int recordsInFile, out int errorsInFile)
		{
			recordsInFile = 0;
			errorsInFile = 0;
			if (!this.ValidateLogForUpload(allLogRows))
			{
				return;
			}
			DataTable csvSchemaDataTable = this.CsvSchemaInstance.GetCsvSchemaDataTable();
			this.AddValuesInDataTable(csvSchemaDataTable, allLogRows, lastLogUpdateTs, out recordsInFile, out errorsInFile);
			this.RemoveDuplicateRowsFromDataTable(csvSchemaDataTable);
			this.InvokeUploadStoredProcedure(csvSchemaDataTable, this.SqlParamName, this.SqlTypeName);
		}

		private void RemoveDuplicateRowsFromDataTable(DataTable dataTable)
		{
			if (this.DistinctColumns == null)
			{
				return;
			}
			Dictionary<string, object[]> dictionary = new Dictionary<string, object[]>();
			DataTable dataTable2 = dataTable.Clone();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				string text = string.Empty;
				foreach (string columnName in this.DistinctColumns)
				{
					text += dataRow[columnName].ToString();
				}
				if (dictionary.ContainsKey(text))
				{
					dataTable2.Rows.Add(dictionary[text]);
					if ((SqlDateTime)dataTable2.Rows[0][this.CsvSchemaInstance.TimeStampColumnName] < (SqlDateTime)dataRow[this.CsvSchemaInstance.TimeStampColumnName])
					{
						dictionary[text] = (dataRow.ItemArray.Clone() as object[]);
					}
					dataTable2.Rows.Clear();
				}
				else
				{
					dictionary[text] = (dataRow.ItemArray.Clone() as object[]);
				}
			}
			dataTable.Rows.Clear();
			foreach (KeyValuePair<string, object[]> keyValuePair in dictionary)
			{
				dataTable.Rows.Add(keyValuePair.Value);
			}
		}

		private void InvokeUploadStoredProcedure(DataTable table, string sqlParamName, string sqlTypeName)
		{
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter(sqlParamName, table)
			{
				SqlDbType = SqlDbType.Structured,
				TypeName = sqlTypeName
			};
			list.Add(item);
			this.HandleUploadStoredProcedureCall(list);
		}

		private void HandleUploadStoredProcedureCall(List<SqlParameter> paramList)
		{
			int config = MigrationMonitor.MigrationMonitorContext.Config.GetConfig<int>("BulkInsertSqlCommandTimeout");
			try
			{
				MigrationMonitor.SqlHelper.ExecuteSprocNonQuery(this.SqlSprocNameToHandleUpload, paramList, config);
			}
			catch (SqlQueryFailedException ex)
			{
				this.LogUploadStoredProcedureHandlerError(ex);
			}
		}

		private DateTime GetLogLastUpdateTS()
		{
			List<SqlParameter> list = new List<SqlParameter>();
			MigrationMonitor.SqlHelper.AddSqlParameter(list, "serverName", MigrationMonitor.ComputerName, false, false);
			return MigrationMonitor.SqlHelper.ExecuteSprocScalar<DateTime>(this.StoredProcNameToGetLastUpdateTimeStamp, list);
		}

		private string[] GetLogFiles()
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Starting to process {0} logs.", new object[]
			{
				this.logFileTypeName
			});
			string[] result;
			try
			{
				result = this.GetFilesInReverseModifiedOrder(this.logDirectoryPath);
			}
			catch (DirectoryNotExistException)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, "No directory for {0} log files found in server {1}", new object[]
				{
					this.logFileTypeName,
					MigrationMonitor.ComputerName
				});
				result = null;
			}
			catch (LogFileLoadException ex)
			{
				this.LogExceptionAndSendWatson("Error loading logs for the log file type", null, ex);
				result = null;
			}
			return result;
		}

		private string[] GetFilesInReverseModifiedOrder(string directoryName)
		{
			List<string> list = new List<string>();
			string[] result;
			try
			{
				if (!Directory.Exists(directoryName))
				{
					throw new DirectoryNotExistException(directoryName);
				}
				DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
				FileInfo[] array = (from p in directoryInfo.GetFiles(this.logFileSearchPattern)
				orderby p.LastWriteTime descending
				select p).ToArray<FileInfo>();
				foreach (FileInfo fileInfo in array)
				{
					list.Add(fileInfo.FullName);
				}
				result = list.ToArray();
			}
			catch (IOException ex)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Error while loading files from directory {0}. Exception: {1}", new object[]
				{
					directoryName,
					ex
				});
				throw new LogFileLoadException(directoryName, ex.InnerException);
			}
			return result;
		}

		private void LogExceptionAndSendWatson(string errorStr, string filePath, Exception ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "{0} log file type {1}, file path {2}", new object[]
			{
				errorStr,
				this.logFileTypeName,
				filePath
			});
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(errorStr);
			stringBuilder.AppendLine(string.Format("Log File Type: {0}", this.logFileTypeName));
			if (!string.IsNullOrEmpty(filePath))
			{
				stringBuilder.AppendLine(string.Format("Log File path: {0}", filePath));
			}
			stringBuilder.AppendLine(string.Format("Exception: {0}", ex));
			ExWatson.SendReport(ex, ReportOptions.None, stringBuilder.ToString());
		}

		private FileStream OpenFileToRead(string inputFile, bool onlyReadExlusiveFromWrite)
		{
			FileStream result;
			try
			{
				result = new FileStream(inputFile, FileMode.Open, FileAccess.Read, onlyReadExlusiveFromWrite ? FileShare.Read : FileShare.ReadWrite);
			}
			catch (IOException ex)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error reading {0} File. record. File name: {1}. Will try again in the next cycle.", new object[]
				{
					this.logFileTypeName,
					inputFile
				});
				throw new LogFileReadException(inputFile, ex);
			}
			return result;
		}

		public const string KeyNameIsLogProcessorEnabled = "IsBaseLogProcessorEnabled";

		protected const string LogFileExtension = "*.log";

		protected readonly BaseMigMonCsvSchema CsvSchemaInstance;

		private readonly string logFileTypeName;

		private readonly string logDirectoryPath;

		private readonly string logFileSearchPattern;
	}
}
