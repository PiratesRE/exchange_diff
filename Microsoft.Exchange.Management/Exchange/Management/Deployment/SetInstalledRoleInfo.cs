using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Set", "InstalledRoleInfo")]
	public sealed class SetInstalledRoleInfo : Task
	{
		[Parameter(Mandatory = true)]
		public string RoleName
		{
			get
			{
				return (string)base.Fields["RoleName"];
			}
			set
			{
				base.Fields["RoleName"] = value;
			}
		}

		[Parameter]
		public Version ConfiguredVersion
		{
			get
			{
				return (Version)base.Fields["ConfiguredVersion"];
			}
			set
			{
				base.Fields["ConfiguredVersion"] = value;
			}
		}

		protected override void InternalValidate()
		{
			if (null == this.ConfiguredVersion)
			{
				this.ConfiguredVersion = ConfigurationContext.Setup.InstalledVersion;
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.RoleName,
				this.ConfiguredVersion
			});
			RolesUtility.SetConfiguredVersion(this.RoleName, this.ConfiguredVersion);
			base.WriteObject(this.ConfiguredVersion);
			TaskLogger.LogExit();
		}
	}
}
