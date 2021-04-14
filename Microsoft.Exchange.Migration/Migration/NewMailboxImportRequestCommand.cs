using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NewMailboxImportRequestCommand : NewMailboxImportRequestCommandBase
	{
		public NewMailboxImportRequestCommand(bool whatIf) : base("New-MailboxImportRequest", NewMailboxImportRequestCommand.ExceptionsToIgnore)
		{
			base.WhatIf = whatIf;
		}

		public string Name
		{
			set
			{
				base.AddParameter("Name", value);
			}
		}

		public object Mailbox
		{
			set
			{
				base.AddParameter("Mailbox", value);
			}
		}

		public string PstFilePath
		{
			set
			{
				base.AddParameter("FilePath", value);
			}
		}

		public bool IsArchive
		{
			set
			{
				base.AddParameter("IsArchive", value);
			}
		}

		public string SourceRootFolder
		{
			set
			{
				base.AddParameter("SourceRootFolder", value);
			}
		}

		public string TargetRootFolder
		{
			set
			{
				base.AddParameter("TargetRootFolder", value);
			}
		}

		public const string CmdletName = "New-MailboxImportRequest";

		private static readonly Type[] ExceptionsToIgnore = new Type[0];
	}
}
