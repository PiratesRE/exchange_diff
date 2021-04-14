using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MonitoringRole : Role
	{
		public MonitoringRole()
		{
			this.roleName = "MonitoringRole";
		}

		public override ServerRole ServerRole
		{
			get
			{
				return ServerRole.Monitoring;
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
				return new InstallMonitoringRole();
			}
		}

		public override Task DisasterRecoveryTask
		{
			get
			{
				return new DisasterRecoveryMonitoringRole();
			}
		}

		public override Task UninstallTask
		{
			get
			{
				return new UninstallMonitoringRole();
			}
		}

		public override ValidatingTask ValidateTask
		{
			get
			{
				return new ValidateMonitoringRole();
			}
		}
	}
}
