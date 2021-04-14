using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MRSBadItemCsvSchema : BaseMrsMonitorCsvSchema
	{
		public MRSBadItemCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(MRSBadItemCsvSchema.requiredColumnsIds, MRSBadItemCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(MRSBadItemCsvSchema.optionalColumnsIds, MRSBadItemCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override DataTable GetCsvSchemaDataTable()
		{
			DataTable csvSchemaDataTable = base.GetCsvSchemaDataTable();
			csvSchemaDataTable.Columns.Add("BadItemKind", typeof(string));
			csvSchemaDataTable.Columns.Add("WKFType", typeof(string));
			csvSchemaDataTable.Columns.Add("MessageClass", typeof(string));
			csvSchemaDataTable.Columns.Add("Category", typeof(string));
			return csvSchemaDataTable;
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return MRSBadItemCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return MRSBadItemCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return MRSBadItemCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return MRSBadItemCsvSchema.optionalColumnsAsIs;
		}

		public const string BadItemKindColumn = "BadItemKind";

		public const string CategoryColumn = "Category";

		public const string WKFTypeColumn = "WKFType";

		public const string MessageClassColumn = "MessageClass";

		public const string FailureMessageColumn = "FailureMessage";

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>();

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<Guid>("RequestGuid")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("BadItemKind", "BadItemKindId", KnownStringType.BadItemKind),
			new ColumnDefinition<int>("WKFType", "WkfTypeId", KnownStringType.BadItemWkfTypeId),
			new ColumnDefinition<int>("MessageClass", "MessageClassId", KnownStringType.BadItemMessageClass),
			new ColumnDefinition<int>("Category", "CategoryId", KnownStringType.BadItemCategory)
		};

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<string>("FailureMessage"),
			new ColumnDefinition<string>("EntryId"),
			new ColumnDefinition<string>("FolderId"),
			new ColumnDefinition<long>("MessageSize"),
			new ColumnDefinition<SqlDateTime>("DateSent"),
			new ColumnDefinition<SqlDateTime>("DateReceived")
		};
	}
}
