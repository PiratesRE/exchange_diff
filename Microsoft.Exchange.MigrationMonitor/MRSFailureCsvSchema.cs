using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MRSFailureCsvSchema : BaseMrsMonitorCsvSchema
	{
		public MRSFailureCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(MRSFailureCsvSchema.requiredColumnsIds, MRSFailureCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(MRSFailureCsvSchema.optionalColumnsIds, MRSFailureCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return MRSFailureCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return MRSFailureCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return MRSFailureCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return MRSFailureCsvSchema.optionalColumnsAsIs;
		}

		public const string FailureTypeColumn = "FailureType";

		public const string FailureSideColumn = "FailureSide";

		public const string SyncStageColumn = "SyncStage";

		public const string OperationTypeColumn = "OperationType";

		public const string IsFatalColumn = "IsFatal";

		public const string StackTraceColumn = "StackTrace";

		public const string AppVersionColumn = "AppVersion";

		public const string FailureGuidColumn = "FailureGuid";

		public const string FailureLevelColumn = "FailureLevel";

		public static readonly ColumnDefinition<int> WatsonHashColumn = new ColumnDefinition<int>("WatsonHash", "WatsonHashId", KnownStringType.WatsonHash);

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("FailureType", "FailureTypeId", KnownStringType.FailureType)
		};

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<Guid>("RequestGuid")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("FailureSide", "FailureSideId", KnownStringType.FailureSide),
			new ColumnDefinition<int>("SyncStage", "SyncStageId", KnownStringType.RequestSyncStage),
			MRSFailureCsvSchema.WatsonHashColumn,
			new ColumnDefinition<int>("AppVersion", "AppVersionId", KnownStringType.AppVersion)
		};

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<string>("OperationType"),
			new ColumnDefinition<bool>("IsFatal"),
			new ColumnDefinition<string>("StackTrace"),
			new ColumnDefinition<Guid>("FailureGuid"),
			new ColumnDefinition<int>("FailureLevel")
		};
	}
}
