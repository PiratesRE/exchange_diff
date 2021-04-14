using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OSPRole : Role
	{
		public OSPRole()
		{
			this.roleName = "OSPRole";
		}

		public override ServerRole ServerRole
		{
			get
			{
				return ServerRole.OSP;
			}
		}

		public override bool IsDatacenterOnly
		{
			get
			{
				return true;
			}
		}

		public override Task InstallTask
		{
			get
			{
				return new InstallOSPRole();
			}
		}

		public override Task DisasterRecoveryTask
		{
			get
			{
				return null;
			}
		}

		public override Task UninstallTask
		{
			get
			{
				return null;
			}
		}

		public override ValidatingTask ValidateTask
		{
			get
			{
				return null;
			}
		}
	}
}
