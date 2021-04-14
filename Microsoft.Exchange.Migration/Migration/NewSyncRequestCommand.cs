using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NewSyncRequestCommand : NewSyncRequestCommandBase
	{
		public NewSyncRequestCommand(bool whatIf) : base("New-SyncRequest", NewSyncRequestCommand.ExceptionsToIgnore)
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

		public SmtpAddress RemoteEmailAddress
		{
			set
			{
				base.AddParameter("RemoteEmailAddress", value);
			}
		}

		public string UserName
		{
			set
			{
				base.AddParameter("UserName", value);
			}
		}

		public bool Imap
		{
			set
			{
				base.AddParameter("Imap", value);
			}
		}

		public bool Olc
		{
			set
			{
				base.AddParameter("Olc", value);
			}
		}

		public RequestWorkloadType WorkloadType
		{
			set
			{
				base.AddParameter("WorkloadType", value);
			}
		}

		public string SkipMerging
		{
			set
			{
				base.AddParameter("SkipMerging", value);
			}
		}

		public const string CmdletName = "New-SyncRequest";

		internal const string MailboxParameter = "Mailbox";

		internal const string RemoteServerNameParameter = "RemoteServerName";

		internal const string RemoteServerPortParameter = "RemoteServerPort";

		internal const string RemoteEmailAddressParameter = "RemoteEmailAddress";

		internal const string UserNameParameter = "UserName";

		internal const string PasswordParameter = "Password";

		internal const string SecurityParameter = "Security";

		internal const string AuthenticationParameter = "Authentication";

		internal const string ImapParameter = "Imap";

		internal const string OlcParameter = "Olc";

		internal const string WorkloadTypeParameter = "WorkloadType";

		private static readonly Type[] ExceptionsToIgnore = new Type[0];
	}
}
