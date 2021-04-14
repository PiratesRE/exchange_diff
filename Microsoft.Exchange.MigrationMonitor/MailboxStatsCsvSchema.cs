using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MailboxStatsCsvSchema : BaseMigMonCsvSchema
	{
		public MailboxStatsCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(MailboxStatsCsvSchema.requiredColumnsIds, MailboxStatsCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(MailboxStatsCsvSchema.optionalColumnsIds, MailboxStatsCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return MailboxStatsCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return MailboxStatsCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return MailboxStatsCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return MailboxStatsCsvSchema.optionalColumnsAsIs;
		}

		public const string MailboxGuidColumn = "MailboxGuid";

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("DatabaseName", "DatabaseId", KnownStringType.DatabaseName),
			new ColumnDefinition<int>("MailboxType", "MailboxTypeId", KnownStringType.MailboxType)
		};

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<Guid>("MailboxGuid"),
			new ColumnDefinition<Guid>("ExternalDirectoryOrganizationId"),
			new ColumnDefinition<long>("ItemCount"),
			new ColumnDefinition<long>("DeletedItemCount"),
			new ColumnDefinition<long>("TotalItemSizeInBytes"),
			new ColumnDefinition<long>("TotalDeletedItemSizeInBytes"),
			new ColumnDefinition<long>("MessageTableTotalSizeInBytes"),
			new ColumnDefinition<long>("AttachmentTableTotalSizeInBytes"),
			new ColumnDefinition<long>("OtherTablesTotalSizeInBytes"),
			new ColumnDefinition<SqlDateTime>("DisconnectDate"),
			new ColumnDefinition<double>("LastLogonTime"),
			new ColumnDefinition<bool>("IsArchiveMailbox"),
			new ColumnDefinition<bool>("IsMoveDestination"),
			new ColumnDefinition<bool>("IsQuarantined")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("DisconnectReason", "DisconnectReasonId", KnownStringType.DisconnectReason)
		};

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<double>("LogicalSizeInM"),
			new ColumnDefinition<double>("PhysicalSizeInM")
		};
	}
}
