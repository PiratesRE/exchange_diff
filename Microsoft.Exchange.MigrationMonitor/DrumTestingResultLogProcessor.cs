using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class DrumTestingResultLogProcessor : BaseLogProcessor
	{
		public DrumTestingResultLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\DrumTestingResults"), "Drum Testing Results", MigrationMonitor.DrumTestingResultCsvSchemaInstance, null)
		{
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetDrumTestingResultLogUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertDrumTestingResult_BatchUpload";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "DrumTestingResultList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "dbo.DrumTestingResultData";
			}
		}

		protected override void LogUploadStoredProcedureHandlerError(SqlQueryFailedException ex)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error uploading Drum Testing result logs. Will attempt again next cycle.", new object[0]);
			throw new UploadDrumTestingResultLogInBatchFailureException(ex.InnerException);
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			List<ColumnDefinition<int>> requiredColumnsIds = this.CsvSchemaInstance.GetRequiredColumnsIds();
			requiredColumnsIds.ForEach(delegate(ColumnDefinition<int> oc)
			{
				this.TryAddStringValueByLookupId(oc, dataRow, row, null, true);
			});
			List<ColumnDefinition<int>> optionalColumnsIds = this.CsvSchemaInstance.GetOptionalColumnsIds();
			optionalColumnsIds.ForEach(delegate(ColumnDefinition<int> oc)
			{
				this.TryAddStringValueByLookupId(oc, dataRow, row, null, true);
			});
			dataRow["ResultDetails"] = MigMonUtilities.TruncateMessage(MigMonUtilities.GetColumnStringValue(row, "ResultDetails"), 500);
			return true;
		}

		protected override void LogInsertCsvRowHandlerError(FormatException ex, CsvRow row)
		{
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error parsing Drum Testing Result log.", new object[0]);
		}

		public new const string KeyNameIsLogProcessorEnabled = "IsDrumTestingResultLogProcessorEnabled";

		private const string LogTypeName = "Drum Testing Results";
	}
}
