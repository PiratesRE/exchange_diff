using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "InstalledRoleInfo")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class RemoveInstalledRoleInfo : Task
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
			RolesUtility.DeleteConfiguredVersion(this.RoleName);
			TaskLogger.LogExit();
		}
	}
}
