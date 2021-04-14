using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Set", "AdminConfiguredVersion")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class SetAdminConfiguredVersion : Task
	{
		public SetAdminConfiguredVersion()
		{
			base.Fields["InstallationMode"] = InstallationModes.Unknown;
		}

		[Parameter(Mandatory = true)]
		public InstallationModes Mode
		{
			get
			{
				return (InstallationModes)base.Fields["InstallationMode"];
			}
			set
			{
				base.Fields["InstallationMode"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			Version unpackedVersion = RolesUtility.GetUnpackedVersion("AdminToolsRole");
			if (unpackedVersion != null && unpackedVersion < AdminToolsRole.FirstConfiguredVersion && RolesUtility.GetConfiguredVersion("AdminToolsRole") == null)
			{
				TaskLogger.Log(Strings.UpdatingAdminToolsConfiguredVersion(unpackedVersion.ToString()));
				RolesUtility.SetConfiguredVersion("AdminToolsRole", unpackedVersion);
			}
			TaskLogger.LogExit();
		}

		private const string TargetRoleName = "AdminToolsRole";
	}
}
