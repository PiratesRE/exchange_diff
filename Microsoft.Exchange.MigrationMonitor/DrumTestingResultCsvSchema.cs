using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class DrumTestingResultCsvSchema : BaseMigMonCsvSchema
	{
		public DrumTestingResultCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(DrumTestingResultCsvSchema.requiredColumnsIds, DrumTestingResultCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(DrumTestingResultCsvSchema.optionalColumnsIds, DrumTestingResultCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return DrumTestingResultCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return DrumTestingResultCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return DrumTestingResultCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return DrumTestingResultCsvSchema.optionalColumnsAsIs;
		}

		public const string ExecutionGuidColumn = "ExecutionGuid";

		public const string TestTypeColumn = "TestType";

		public const string ObjectTypeColumn = "ObjectType";

		public const string IdentityColumn = "Identity";

		public const string FlagsColumn = "Flags";

		public const string StringIdColumn = "StringId";

		public const string ResultTypeColumn = "ResultType";

		public const string ResultCategoryColumn = "ResultCategory";

		public const string ResultDetailsColumn = "ResultDetails";

		public const string ResultHashColumn = "ResultHash";

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("TestType", "DrumTestingTestTypeId", KnownStringType.DrumTestingTestType)
		};

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<Guid>("ExecutionGuid"),
			new ColumnDefinition<Guid>("Identity")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("ObjectType", "DrumTestingObjectTypeId", KnownStringType.DrumTestingObjectType),
			new ColumnDefinition<int>("ResultType", "DrumTestingResultTypeId", KnownStringType.DrumTestingResultType),
			new ColumnDefinition<int>("ResultCategory", "DrumTestingResultCategoryId", KnownStringType.DrumTestingResultCategoryType)
		};

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<int>("Flags"),
			new ColumnDefinition<string>("StringId"),
			new ColumnDefinition<string>("ResultDetails"),
			new ColumnDefinition<int>("ResultHash")
		};
	}
}
