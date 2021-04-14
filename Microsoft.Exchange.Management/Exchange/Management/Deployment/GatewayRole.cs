using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GatewayRole : Role
	{
		public GatewayRole()
		{
			this.roleName = "GatewayRole";
		}

		public override ServerRole ServerRole
		{
			get
			{
				return ServerRole.Edge;
			}
		}

		public override Task InstallTask
		{
			get
			{
				return new InstallGatewayRole();
			}
		}

		public override Task DisasterRecoveryTask
		{
			get
			{
				return new DisasterRecoveryGatewayRole();
			}
		}

		public override Task UninstallTask
		{
			get
			{
				return new UninstallGatewayRole();
			}
		}

		public override ValidatingTask ValidateTask
		{
			get
			{
				return new ValidateGatewayRole();
			}
		}
	}
}
