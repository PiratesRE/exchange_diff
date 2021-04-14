using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxRole : Role
	{
		public MailboxRole()
		{
			this.roleName = "MailboxRole";
		}

		public override ServerRole ServerRole
		{
			get
			{
				return ServerRole.Mailbox;
			}
		}

		public override Task InstallTask
		{
			get
			{
				return new InstallMailboxRole();
			}
		}

		public override Task DisasterRecoveryTask
		{
			get
			{
				return new DisasterRecoveryMailboxRole();
			}
		}

		public override Task UninstallTask
		{
			get
			{
				return new UninstallMailboxRole();
			}
		}

		public override ValidatingTask ValidateTask
		{
			get
			{
				return new ValidateMailboxRole();
			}
		}
	}
}
