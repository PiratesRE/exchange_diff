using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ClientAccessRole : Role
	{
		public ClientAccessRole()
		{
			this.roleName = "ClientAccessRole";
		}

		public override ServerRole ServerRole
		{
			get
			{
				return ServerRole.ClientAccess;
			}
		}

		public override Task InstallTask
		{
			get
			{
				return new InstallClientAccessRole();
			}
		}

		public override Task DisasterRecoveryTask
		{
			get
			{
				return new DisasterRecoveryClientAccessRole();
			}
		}

		public override Task UninstallTask
		{
			get
			{
				return new UninstallClientAccessRole();
			}
		}

		public override ValidatingTask ValidateTask
		{
			get
			{
				return new ValidateClientAccessRole();
			}
		}
	}
}
