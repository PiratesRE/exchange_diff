using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NewMoveRequestCommand : NewMoveRequestCommandBase
	{
		public NewMoveRequestCommand(bool whatIf) : base("New-MoveRequest", NewMoveRequestCommand.ExceptionsToIgnore)
		{
			base.WhatIf = whatIf;
		}

		public bool Remote
		{
			set
			{
				base.AddParameter("Remote", value);
			}
		}

		public bool Outbound
		{
			set
			{
				base.AddParameter("Outbound", value);
			}
		}

		public bool PrimaryOnly
		{
			set
			{
				base.AddParameter("PrimaryOnly", value);
			}
		}

		public bool ArchiveOnly
		{
			set
			{
				base.AddParameter("ArchiveOnly", value);
			}
		}

		public string TargetDatabase
		{
			set
			{
				base.AddParameter("TargetDatabase", value);
			}
		}

		public string ArchiveTargetDatabase
		{
			set
			{
				base.AddParameter("ArchiveTargetDatabase", value);
			}
		}

		public string RemoteTargetDatabase
		{
			set
			{
				base.AddParameter("RemoteTargetDatabase", value);
			}
		}

		public string RemoteArchiveTargetDatabase
		{
			set
			{
				base.AddParameter("RemoteArchiveTargetDatabase", value);
			}
		}

		public bool IgnoreTenantMigrationPolicies
		{
			set
			{
				base.AddParameter("IgnoreTenantMigrationPolicies", value);
			}
		}

		public string TargetDeliveryDomain
		{
			set
			{
				base.AddParameter("TargetDeliveryDomain", value);
			}
		}

		public const string CmdletName = "New-MoveRequest";

		private static readonly Type[] ExceptionsToIgnore = new Type[0];
	}
}
