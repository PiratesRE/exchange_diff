using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MRSWorkloadAvailabilityCsvSchema : BaseMigMonCsvSchema
	{
		public MRSWorkloadAvailabilityCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(MRSWorkloadAvailabilityCsvSchema.requiredColumnsIds, MRSWorkloadAvailabilityCsvSchema.requiredColumnsAsIs, "Time"), null, null)
		{
		}

		public override DataTable GetCsvSchemaDataTable()
		{
			return new DataTable
			{
				Columns = 
				{
					{
						this.TimeStampColumnName,
						typeof(SqlDateTime)
					},
					{
						"LoggingServerId",
						typeof(int)
					},
					{
						"Version",
						typeof(int)
					},
					{
						"WorkloadTypeId",
						typeof(int)
					},
					{
						"Availability",
						typeof(float)
					}
				}
			};
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return MRSWorkloadAvailabilityCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return MRSWorkloadAvailabilityCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return new List<ColumnDefinition<int>>();
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return new List<IColumnDefinition>();
		}

		public const string ServerColumn = "Server";

		public const string VersionColumn = "Version";

		public const string EventContextColumn = "EventContext";

		public const string EventDataColumn = "EventData";

		public const string WorkloadTypeIdColumn = "WorkloadTypeId";

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>();

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<object>("Server"),
			new ColumnDefinition<object>("Version"),
			new ColumnDefinition<object>("EventContext"),
			new ColumnDefinition<object>("EventData")
		};
	}
}
