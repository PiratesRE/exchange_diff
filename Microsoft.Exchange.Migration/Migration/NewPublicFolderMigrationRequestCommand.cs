using System;
using System.IO;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NewPublicFolderMigrationRequestCommand : NewPublicFolderMigrationRequestCommandBase
	{
		public NewPublicFolderMigrationRequestCommand(bool whatIf) : base("New-PublicFolderMailboxMigrationRequest", NewPublicFolderMigrationRequestCommand.ExceptionsToIgnore)
		{
			base.WhatIf = whatIf;
		}

		public Stream CSVStream
		{
			set
			{
				base.AddParameter("CSVStream", value);
			}
		}

		public MailboxIdParameter TargetMailbox
		{
			set
			{
				base.AddParameter("TargetMailbox", value);
			}
		}

		public OrganizationIdParameter Organization
		{
			set
			{
				base.AddParameter("Organization", value);
			}
		}

		public const string CmdletName = "New-PublicFolderMailboxMigrationRequest";

		private const string CSVStreamParameter = "CSVStream";

		private const string TargetMailboxParameter = "TargetMailbox";

		private const string OrganizationParameter = "Organization";

		private static readonly Type[] ExceptionsToIgnore = new Type[]
		{
			typeof(ManagementObjectAlreadyExistsException)
		};
	}
}
