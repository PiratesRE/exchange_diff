using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class WLMResourceStatsCsvSchema : BaseMigMonCsvSchema
	{
		public WLMResourceStatsCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(WLMResourceStatsCsvSchema.requiredColumnsIds, WLMResourceStatsCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(WLMResourceStatsCsvSchema.optionalColumnsIds, WLMResourceStatsCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override DataTable GetCsvSchemaDataTable()
		{
			return base.GetCsvSchemaDataTable();
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return WLMResourceStatsCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return WLMResourceStatsCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return WLMResourceStatsCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return WLMResourceStatsCsvSchema.optionalColumnsAsIs;
		}

		public const string OwnerResourceNameColumnName = "OwnerResourceName";

		public const string OwnerResourceTypeColumnName = "OwnerResourceType";

		public const string ResourceKeyColumnName = "ResourceKey";

		public const string LoadStateColumnName = "LoadState";

		public const string LoadRatioColumnName = "LoadRatio";

		public const string MetricColumnName = "Metric";

		public const string DynamicCapacityColumnName = "DynamicCapacity";

		public const string UnderloadedInFiveMinColumnName = "UnderloadedIn5Min";

		public const string FullInFiveMinColumnName = "FullIn5Min";

		public const string OverloadedInFiveMinColumnName = "OverloadedIn5Min";

		public const string CriticalInFiveMinColumnName = "CriticalIn5Min";

		public const string UnknownInFiveMinColumnName = "UnknownIn5Min";

		public const string UnderloadedInHourColumnName = "UnderloadedIn1Hour";

		public const string FullInHourColumnName = "FullIn1Hour";

		public const string OverloadedInHourColumnName = "OverloadedIn1Hour";

		public const string CriticalInHourColumnName = "CriticalIn1Hour";

		public const string UnknownInHourColumnName = "UnknownIn1Hour";

		public const string UnderloadedInDayColumnName = "UnderloadedIn1Day";

		public const string FullInDayColumnName = "FullIn1Day";

		public const string OverloadedInDayColumnName = "OverloadedIn1Day";

		public const string CriticalInDayColumnName = "CriticalIn1Day";

		public const string UnknownInDayColumnName = "UnknownIn1Day";

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("OwnerResourceName", "OwnerResourceNameID", KnownStringType.OwnerResourceNameType),
			new ColumnDefinition<int>("OwnerResourceType", "OwnerResourceTypeID", KnownStringType.OwnerResourceTypeType),
			new ColumnDefinition<int>("ResourceKey", "ResourceKeyID", KnownStringType.ResourceKeyType),
			new ColumnDefinition<int>("LoadState", "LoadStateID", KnownStringType.LoadStateType)
		};

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<float>("LoadRatio"),
			new ColumnDefinition<int>("DynamicCapacity"),
			new ColumnDefinition<int>("UnderloadedIn5Min"),
			new ColumnDefinition<int>("FullIn5Min"),
			new ColumnDefinition<int>("OverloadedIn5Min"),
			new ColumnDefinition<int>("CriticalIn5Min"),
			new ColumnDefinition<int>("UnknownIn5Min"),
			new ColumnDefinition<int>("UnderloadedIn1Hour"),
			new ColumnDefinition<int>("FullIn1Hour"),
			new ColumnDefinition<int>("OverloadedIn1Hour"),
			new ColumnDefinition<int>("CriticalIn1Hour"),
			new ColumnDefinition<int>("UnknownIn1Hour"),
			new ColumnDefinition<int>("UnderloadedIn1Day"),
			new ColumnDefinition<int>("FullIn1Day"),
			new ColumnDefinition<int>("OverloadedIn1Day"),
			new ColumnDefinition<int>("CriticalIn1Day"),
			new ColumnDefinition<int>("UnknownIn1Day")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>();

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<int>("Metric")
		};
	}
}
