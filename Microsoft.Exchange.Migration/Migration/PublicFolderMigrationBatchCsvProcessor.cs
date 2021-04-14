using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Migration
{
	internal sealed class PublicFolderMigrationBatchCsvProcessor : MigrationBatchCsvProcessor
	{
		public PublicFolderMigrationBatchCsvProcessor(PublicFolderMigrationCsvSchema schema, IMigrationDataProvider dataProvider) : base(schema)
		{
			this.hierarchyMailboxName = dataProvider.ADProvider.GetPublicFolderHierarchyMailboxName();
			if (this.hierarchyMailboxName == null)
			{
				throw new PublicFolderMailboxesNotProvisionedException();
			}
		}

		protected override bool ValidationWarningAsError
		{
			get
			{
				return true;
			}
		}

		protected override LocalizedException InternalProcessRow(CsvRow row, out bool isDataRow)
		{
			string text = row["FolderPath"];
			string text2 = row["TargetMailbox"];
			if (row.IsValid && !this.folderList.Add(text))
			{
				isDataRow = false;
				return new DuplicateFolderInCSVException(row.Index, text, text2);
			}
			if ("\\".Equals(text, StringComparison.InvariantCultureIgnoreCase))
			{
				if (!this.hierarchyMailboxName.Equals(text2, StringComparison.InvariantCultureIgnoreCase))
				{
					isDataRow = false;
					return new InvalidRootFolderMappingInCSVException(row.Index, text, text2, this.hierarchyMailboxName);
				}
				this.validPublicFolderMappingFound = true;
			}
			isDataRow = this.mailboxes.Add(text2);
			return null;
		}

		protected override LocalizedException Validate()
		{
			if (!this.validPublicFolderMappingFound)
			{
				return new MissingRootFolderMappingInCSVException(this.hierarchyMailboxName);
			}
			return null;
		}

		private const string RootFolderPath = "\\";

		private readonly HashSet<string> folderList = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		private readonly HashSet<string> mailboxes = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		private readonly string hierarchyMailboxName;

		private bool validPublicFolderMappingFound;
	}
}
