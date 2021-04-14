using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class QueueMRSWorkStatsCsvSchema : BaseMigMonCsvSchema
	{
		public QueueMRSWorkStatsCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(QueueMRSWorkStatsCsvSchema.requiredColumnsIds, QueueMRSWorkStatsCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(QueueMRSWorkStatsCsvSchema.optionalColumnsIds, QueueMRSWorkStatsCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override DataTable GetCsvSchemaDataTable()
		{
			return base.GetCsvSchemaDataTable();
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return QueueMRSWorkStatsCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return QueueMRSWorkStatsCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return QueueMRSWorkStatsCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return QueueMRSWorkStatsCsvSchema.optionalColumnsAsIs;
		}

		public const string QueuedAndRelinquishedJobsColumnName = "QueuedAndRelinquishedJobs";

		public const string InprogressJobsColumnName = "InprogressJobs";

		public const string LastJobPickupTimeColumnName = "LastJobPickupTime";

		public const string LastScanFailureColumnName = "LastScanFailure";

		public const string LastScanTimestampColumnName = "LastScanTimestamp";

		public const string LastScanDurationMsColumnName = "LastScanDurationMs";

		public const string NextScanTimestampColumnName = "NextScanTimestamp";

		public const string MdbDiscoveryTimeStampColumnName = "MdbDiscoveryTimeStamp";

		public const string LastActiveJobFinishTimeColumnName = "LastActiveJobFinishTime";

		public const string LastActiveJobFinishedColumnName = "LastActiveJobFinished";

		public const string LongQueuedTenantUpgradeColumnName = "LongQueuedTenantUpgradeMoves";

		public const string LongQueuedCustomerMovesColumnName = "LongQueuedCustomerMoves";

		public const string LastScanFailureColumnId = "LastScanFailureId";

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>();

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<int>("QueuedAndRelinquishedJobs"),
			new ColumnDefinition<int>("InprogressJobs"),
			new ColumnDefinition<SqlDateTime>("LastJobPickupTime"),
			new ColumnDefinition<SqlDateTime>("LastScanTimestamp"),
			new ColumnDefinition<int>("LastScanDurationMs"),
			new ColumnDefinition<SqlDateTime>("NextScanTimestamp"),
			new ColumnDefinition<SqlDateTime>("MdbDiscoveryTimeStamp"),
			new ColumnDefinition<SqlDateTime>("LastActiveJobFinishTime"),
			new ColumnDefinition<Guid>("LastActiveJobFinished"),
			new ColumnDefinition<int>("LongQueuedTenantUpgradeMoves"),
			new ColumnDefinition<int>("LongQueuedCustomerMoves")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("LastScanFailure", "LastScanFailureId", KnownStringType.LastScanFailureFailureType)
		};

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>();
	}
}
