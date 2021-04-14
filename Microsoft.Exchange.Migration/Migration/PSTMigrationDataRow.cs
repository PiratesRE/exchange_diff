using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PSTMigrationDataRow : IMigrationDataRow
	{
		public PSTMigrationDataRow(int rowIndex, string pstFilePath, SmtpAddress targetMailboxAddress, MigrationMailboxType targetMailboxType, string sourceRootFolder, string targetRootFolder)
		{
			MigrationUtil.ThrowOnNullArgument(pstFilePath, "pstFilePath");
			if (rowIndex < 1)
			{
				throw new ArgumentException("RowIndex should not be less than 1");
			}
			this.CursorPosition = rowIndex;
			this.PSTFilePath = pstFilePath;
			this.TargetMailboxAddress = targetMailboxAddress;
			this.TargetMailboxType = targetMailboxType;
			this.SourceRootFolder = sourceRootFolder;
			this.TargetRootFolder = targetRootFolder;
		}

		public MigrationType MigrationType
		{
			get
			{
				return MigrationType.PSTImport;
			}
		}

		public MigrationUserRecipientType RecipientType
		{
			get
			{
				return MigrationUserRecipientType.Mailbox;
			}
		}

		public int CursorPosition { get; private set; }

		public string PSTFilePath { get; private set; }

		public SmtpAddress TargetMailboxAddress { get; private set; }

		public MigrationMailboxType TargetMailboxType { get; private set; }

		public string SourceRootFolder { get; private set; }

		public string TargetRootFolder { get; private set; }

		public string Identifier
		{
			get
			{
				string arg = this.PSTFilePath;
				int num = this.PSTFilePath.LastIndexOf("\\");
				if (num != -1)
				{
					arg = this.PSTFilePath.Substring(num + 1);
				}
				return string.Format("{0}_{1}:{2}", this.TargetMailboxAddress.ToString().ToLowerInvariant(), arg, this.CursorPosition.ToString());
			}
		}

		public string LocalMailboxIdentifier
		{
			get
			{
				return this.TargetMailboxAddress.ToString().ToLowerInvariant();
			}
		}

		public bool SupportsRemoteIdentifier
		{
			get
			{
				return false;
			}
		}

		public string RemoteIdentifier
		{
			get
			{
				return null;
			}
		}
	}
}
