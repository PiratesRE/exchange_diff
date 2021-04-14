using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class BridgeheadRole : Role
	{
		public BridgeheadRole()
		{
			this.roleName = "BridgeheadRole";
		}

		public override ServerRole ServerRole
		{
			get
			{
				return ServerRole.HubTransport;
			}
		}

		public override Task InstallTask
		{
			get
			{
				return new InstallBridgeheadRole();
			}
		}

		public override Task DisasterRecoveryTask
		{
			get
			{
				return new DisasterRecoveryBridgeheadRole();
			}
		}

		public override Task UninstallTask
		{
			get
			{
				return new UninstallBridgeheadRole();
			}
		}

		public override ValidatingTask ValidateTask
		{
			get
			{
				return new ValidateBridgeheadRole();
			}
		}
	}
}
