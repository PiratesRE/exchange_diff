using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AdminToolsRole : Role
	{
		public AdminToolsRole()
		{
			this.roleName = "AdminToolsRole";
		}

		public override ServerRole ServerRole
		{
			get
			{
				return ServerRole.None;
			}
		}

		public override Task InstallTask
		{
			get
			{
				return null;
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

		public static readonly Version FirstConfiguredVersion = new Version(8, 1, 63, 0);
	}
}
