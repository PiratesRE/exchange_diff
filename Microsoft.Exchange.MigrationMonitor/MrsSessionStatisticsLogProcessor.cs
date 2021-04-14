using System;
using System.Data;
using System.IO;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MrsSessionStatisticsLogProcessor : BaseMrsMonitorLogProcessor
	{
		public MrsSessionStatisticsLogProcessor() : base(Path.Combine(MigrationMonitor.ExchangeInstallPath, "Logging\\MailboxReplicationService\\SessionStatistics"), "MRS SessionStatistics", MigrationMonitor.MrsSessionStatisticsCsvSchemaInstance)
		{
		}

		protected override string[] DistinctColumns
		{
			get
			{
				return new string[]
				{
					"RequestGuid",
					"SessionId",
					"SessionId_Archive"
				};
			}
		}

		protected override string StoredProcNameToGetLastUpdateTimeStamp
		{
			get
			{
				return "MIGMON_GetSessionStatisticsLogUpdateTimestamp";
			}
		}

		protected override string SqlSprocNameToHandleUpload
		{
			get
			{
				return "MIGMON_InsertMrsSessionStatistics_BatchUpload";
			}
		}

		protected override string SqlParamName
		{
			get
			{
				return "MrsSessionStatisticsList";
			}
		}

		protected override string SqlTypeName
		{
			get
			{
				return "dbo.MrsSessionStatisticsData";
			}
		}

		protected override bool TryAddSchemaSpecificDataRowValues(DataRow dataRow, CsvRow row)
		{
			string empty = string.Empty;
			base.TryAddStringValueByLookupId(dataRow, row, KnownStringType.MaxProviderDurationMethodName, empty, true);
			return true;
		}

		public new const string KeyNameIsLogProcessorEnabled = "IsMrsSessionStatisticsLogProcessorEnabled";

		private const string LogTypeName = "MRS SessionStatistics";
	}
}
