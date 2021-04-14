using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class JobPickupResultsCsvSchema : BaseMigMonCsvSchema
	{
		public JobPickupResultsCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(JobPickupResultsCsvSchema.requiredColumnsIds, JobPickupResultsCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(JobPickupResultsCsvSchema.optionalColumnsIds, JobPickupResultsCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override DataTable GetCsvSchemaDataTable()
		{
			return base.GetCsvSchemaDataTable();
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return JobPickupResultsCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return JobPickupResultsCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return JobPickupResultsCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return JobPickupResultsCsvSchema.optionalColumnsAsIs;
		}

		public const string RequestGuidColumnName = "RequestGuid";

		public const string JobTimestampColumnName = "TimeStamp";

		public const string RequestTypeColumnName = "RequestType";

		public const string RequestStatusColumnName = "RequestStatus";

		public const string WorkloadTypeColumnName = "WorkloadType";

		public const string PriorityColumnName = "Priority";

		public const string LastUpdateTimestampColumnName = "LastUpdateTimeStamp";

		public const string PickupResultsColumnName = "PickupResult";

		public const string NextScanTimestampColumnName = "NextRecommendedPickup";

		public const string MessageColumnName = "Message";

		public const string ReservationFailureReasonColumnName = "ReservationFailureReason";

		public const string ReservationFailureResourceTypeColumnName = "ReservationFailureResourceType";

		public const string ReservationFailureWLMResourceTypeColumnName = "ReservationFailureWLMResourceType";

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("RequestType", "RequestTypeID", KnownStringType.RequestType),
			new ColumnDefinition<int>("RequestStatus", "RequestStatusID", KnownStringType.RequestStatus),
			new ColumnDefinition<int>("WorkloadType", "WorkloadTypeID", KnownStringType.RequestWorkloadType),
			new ColumnDefinition<int>("PickupResult", "PickupResultID", KnownStringType.PickupResultsType)
		};

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<Guid>("RequestGuid"),
			new ColumnDefinition<SqlDateTime>("TimeStamp"),
			new ColumnDefinition<int>("Priority"),
			new ColumnDefinition<SqlDateTime>("LastUpdateTimeStamp"),
			new ColumnDefinition<SqlDateTime>("NextRecommendedPickup")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("ReservationFailureReason", "ReservationFailureReasonID", KnownStringType.ReservationFailureReasonType),
			new ColumnDefinition<int>("ReservationFailureResourceType", "ReservationFailureResourceTypeID", KnownStringType.ReservationFailureResourceTypeType),
			new ColumnDefinition<int>("ReservationFailureWLMResourceType", "ReservationFailureWLMResourceTypeID", KnownStringType.ReservationFailureWLMResourceTypeType)
		};

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<int>("Message")
		};
	}
}
