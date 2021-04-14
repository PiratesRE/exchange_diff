using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Get", "InstalledRoleInfo")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class GetInstalledRoleInfo : Task
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

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.RoleName
			});
			Version configuredVersion = RolesUtility.GetConfiguredVersion(this.RoleName);
			base.WriteObject(configuredVersion);
			TaskLogger.LogExit();
		}
	}
}
