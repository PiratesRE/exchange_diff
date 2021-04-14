using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FrontendTransportRole : Role
	{
		public FrontendTransportRole()
		{
			this.roleName = "FrontendTransportRole";
		}

		public override ServerRole ServerRole
		{
			get
			{
				return ServerRole.FrontendTransport;
			}
		}

		public override Task InstallTask
		{
			get
			{
				return new InstallFrontendTransportRole();
			}
		}

		public override Task DisasterRecoveryTask
		{
			get
			{
				return new DisasterRecoveryFrontendTransportRole();
			}
		}

		public override Task UninstallTask
		{
			get
			{
				return new UninstallFrontendTransportRole();
			}
		}

		public override ValidatingTask ValidateTask
		{
			get
			{
				return new ValidateFrontendTransportRole();
			}
		}
	}
}
