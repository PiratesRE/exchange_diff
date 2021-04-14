using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Cmdlet("Install", "ThrottlingPolicies")]
	public sealed class InstallThrottlingPolicies : SetupTaskBase
	{
		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public InstallationModes InstallationMode
		{
			get
			{
				return (InstallationModes)(base.Fields["InstallationMode"] ?? InstallationModes.Install);
			}
			set
			{
				base.Fields["InstallationMode"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if (this.InstallationMode != InstallationModes.Install && this.InstallationMode != InstallationModes.BuildToBuildUpgrade)
			{
				base.WriteError(new ArgumentOutOfRangeException("InstallationMode", this.InstallationMode, Strings.ErrorInstallationModeNotSupported), ErrorCategory.InvalidArgument, null);
			}
			base.InternalBeginProcessing();
			this.rootOrgSession.SessionSettings.IsSharedConfigChecked = true;
			base.WriteVerbose(Strings.RetrievingGlobalThrottlingPolicy);
			this.globalPolicy = this.rootOrgSession.GetGlobalThrottlingPolicy();
			if (this.globalPolicy != null)
			{
				if (this.InstallationMode == InstallationModes.Install)
				{
					this.WriteWarning(Strings.FoundGlobalThrottlingPolicy(this.globalPolicy.Id.ToString()));
					this.WriteWarning(Strings.WillNotCreateGlobalThrottlingPolicy);
				}
				else
				{
					base.WriteVerbose(Strings.FoundGlobalThrottlingPolicy(this.globalPolicy.Id.ToString()));
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (this.globalPolicy == null)
			{
				this.InstallGlobalThrottlingPolicy();
			}
			if (this.InstallationMode == InstallationModes.BuildToBuildUpgrade)
			{
				this.ResetGlobalThrottlingPolicySettings();
			}
			TaskLogger.LogExit();
		}

		private void InstallGlobalThrottlingPolicy()
		{
			string text = "GlobalThrottlingPolicy_" + Guid.NewGuid().ToString("D");
			base.WriteVerbose(Strings.InstallingGlobalThrottlingPolicy(text));
			ThrottlingPolicy throttlingPolicy = new ThrottlingPolicy();
			throttlingPolicy.CloneThrottlingSettingsFrom(FallbackThrottlingPolicy.GetSingleton());
			throttlingPolicy.ThrottlingPolicyScope = ThrottlingPolicyScopeType.Global;
			throttlingPolicy.SetId(this.configurationSession, new ADObjectId("CN=Global Settings"), text);
			if (base.CurrentOrganizationId != null)
			{
				throttlingPolicy.OrganizationId = base.CurrentOrganizationId;
			}
			else
			{
				throttlingPolicy.OrganizationId = base.ExecutingUserOrganizationId;
			}
			this.SaveObject(throttlingPolicy);
		}

		private void ResetGlobalThrottlingPolicySettings()
		{
			this.rootOrgSession.SessionSettings.IsSharedConfigChecked = true;
			ThrottlingPolicy globalThrottlingPolicy = this.rootOrgSession.GetGlobalThrottlingPolicy();
			if (globalThrottlingPolicy == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorGlobalThrottlingPolicyNotFound), ErrorCategory.InvalidResult, null);
			}
			globalThrottlingPolicy.CloneThrottlingSettingsFrom(FallbackThrottlingPolicy.GetSingleton());
			this.SaveObject(globalThrottlingPolicy);
		}

		private void SaveObject(ADConfigurationObject dataObject)
		{
			try
			{
				if (dataObject.Identity != null)
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(dataObject, this.configurationSession, dataObject.GetType()));
				}
				using (TaskPerformanceData.SaveResult.StartRequestTimer())
				{
					this.configurationSession.Save(dataObject);
				}
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, (ErrorCategory)1002, null);
			}
			finally
			{
				if (dataObject.Identity != null)
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(this.configurationSession));
				}
			}
		}

		private ThrottlingPolicy globalPolicy;
	}
}
