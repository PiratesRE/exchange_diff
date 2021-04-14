using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.LogAnalyzer.Extensions.MigrationLog;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MRSWorkloadAvailabilityLogProcessor : BaseLogProcessor
	{
		public MRSWorkloadAvailabilityLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MailboxReplicationService\\MRSAvailability"), "MRSWorkloadAvailability", MigrationMonitor.MRSWorkloadAvailabilityCsvSchemaInstance, "MRSAvailability_*.LOG")
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetMRSAvailabilityLogUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertMRSAvailability_BatchUpload";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "MrsWorkloadAvailabilityList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "dbo.MrsWorkloadAvailabilityData";
			}
		}

		protected override string[] DistinctColumns
		{
			get
			{
				return new string[]
				{
					"WorkloadTypeId"
				};
			}
		}

		protected override void AddValuesInDataTable(DataTable table, List<CsvRow> allLogRows, DateTime lastLogUpdateTs, out int recordsInFile, out int errorsInFile)
		{
			recordsInFile = 0;
			errorsInFile = 0;
			foreach (CsvRow csvRow in from logRow in allLogRows
			where logRow.Index != 0
			select logRow)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				if (csvRow.ColumnMap.Contains("EventData") && MRSAvailabilityLogLine.TryParseWorkloadStates(csvRow["EventData"], dictionary))
				{
					foreach (KeyValuePair<string, int> kvp in dictionary)
					{
						DateTime? dateTime = new DateTime?(MigMonUtilities.GetColumnValue<DateTime>(csvRow, this.CsvSchemaInstance.TimeStampColumnName));
						if (dateTime == null || !(dateTime.Value <= lastLogUpdateTs))
						{
							recordsInFile++;
							DataRow dataRow = table.NewRow();
							KeyValuePair<string, int> kvp1 = kvp;
							Action<DataRow, CsvRow> insertAction = delegate(DataRow dr, CsvRow lr)
							{
								this.AddCommonDataRowValues(dr, lr);
								int? valueFromIdMap = MigMonUtilities.GetValueFromIdMap(kvp1.Key, KnownStringType.RequestWorkloadType, KnownStringsHelper.KnownStringToSqlLookupParam[KnownStringType.RequestWorkloadType]);
								dr["WorkloadTypeId"] = ((valueFromIdMap != null) ? valueFromIdMap.Value : DBNull.Value);
								dr["Version"] = MigMonUtilities.GetColumnValue<int>(lr, "Version");
								dataRow["Availability"] = (float)kvp1.Value;
							};
							if (!base.HandleInsertCsvRowExceptions(insertAction, dataRow, csvRow))
							{
								errorsInFile++;
							}
							table.Rows.Add(dataRow);
						}
					}
				}
			}
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			return true;
		}

		protected override void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error uploading MRS workload availability logs. Will attempt again next cycle.", new object[0]);
			throw new UploadMrsAvailabilityLogInBatchFailureException(ex.InnerException);
		}

		protected override void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error parsing MRS availability log.", new object[0]);
		}

		public new const string KeyNameIsLogProcessorEnabled = "IsMRSWorkloadAvailabilityLogProcessorEnabled";

		private const string LogTypeName = "MRSWorkloadAvailability";
	}
}
