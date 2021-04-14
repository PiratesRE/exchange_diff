using System;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.SharePoint.Client;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Install", "UnifiedCompliancePrerequisite", DefaultParameterSetName = "Initialize")]
	public sealed class InstallUnifiedCompliancePrerequisite : NewMultitenancyFixedNameSystemConfigurationObjectTask<PolicyStorage>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "Initialize")]
		public SmtpAddress PolicyCenterSiteOwner
		{
			get
			{
				return (SmtpAddress)(base.Fields["PolicyCenterSiteOwner"] ?? SmtpAddress.Empty);
			}
			set
			{
				base.Fields["PolicyCenterSiteOwner"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "LoadOnly")]
		public SwitchParameter LoadOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["LoadOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["LoadOnly"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Initialize")]
		public SwitchParameter ForceInitialize
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForceInitialize"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForceInitialize"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			Utils.ThrowIfNotRunInEOP();
			Utils.ValidateNotForestWideOrganization(base.CurrentOrganizationId);
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			UnifiedCompliancePrerequisite unifiedCompliancePrerequisite = this.LoadInitializedPrerequisite();
			if (!this.LoadOnly && (base.Organization == null || this.ForceInitialize) && unifiedCompliancePrerequisite.CanInitializeSharepoint && (this.ForceInitialize || !unifiedCompliancePrerequisite.IsSharepointInitialized))
			{
				this.InitializeSharePoint(ref unifiedCompliancePrerequisite);
				this.SaveInitializedPrerequisite(unifiedCompliancePrerequisite);
			}
			if (base.NeedSuppressingPiiData)
			{
				unifiedCompliancePrerequisite.Redact();
			}
			base.WriteObject(unifiedCompliancePrerequisite);
			TaskLogger.LogExit();
		}

		private UnifiedCompliancePrerequisite LoadInitializedPrerequisite()
		{
			Uri spRootSiteUrl;
			Uri spTenantAdminUrl;
			UnifiedPolicyConfiguration.GetInstance().GetTenantSharePointUrls(this.ConfigurationSession, out spRootSiteUrl, out spTenantAdminUrl);
			return new UnifiedCompliancePrerequisite(spRootSiteUrl, spTenantAdminUrl, UnifiedPolicyConfiguration.GetInstance().GetUnifiedPolicyPreReqState(this.ConfigurationSession));
		}

		private void SaveInitializedPrerequisite(UnifiedCompliancePrerequisite prerequisite)
		{
			UnifiedPolicyConfiguration.GetInstance().SetUnifiedPolicyPreReqState(this.ConfigurationSession, prerequisite.ToPrerequisiteList());
		}

		private void InitializeSharePoint(ref UnifiedCompliancePrerequisite prerequisite)
		{
			if (!this.PolicyCenterSiteOwner.IsValidAddress && base.CurrentTaskContext != null && base.CurrentTaskContext.UserInfo != null)
			{
				this.PolicyCenterSiteOwner = base.CurrentTaskContext.UserInfo.ExecutingWindowsLiveId;
			}
			if (!this.PolicyCenterSiteOwner.IsValidAddress)
			{
				throw new ErrorInvalidPolicyCenterSiteOwnerException();
			}
			base.WriteVerbose(Strings.VerbosePolicyCenterSiteOwner(this.PolicyCenterSiteOwner.ToString()));
			SpPolicyCenterSite spPolicyCenterSite = new SpPolicyCenterSite(prerequisite.SharepointRootSiteUrl, prerequisite.SharepointTenantAdminUrl, UnifiedPolicyConfiguration.GetInstance().GetCredentials(this.ConfigurationSession, null));
			bool flag = true;
			long num = 3600000L;
			Stopwatch stopwatch = new Stopwatch();
			int num2 = 0;
			while (num2 <= 999 && num > 0L)
			{
				stopwatch.Restart();
				if (flag)
				{
					Uri policyCenterSite = spPolicyCenterSite.GetPolicyCenterSite(false);
					base.WriteVerbose(Strings.VerboseTryLoadPolicyCenterSite(policyCenterSite));
					flag = false;
					if (policyCenterSite != null)
					{
						prerequisite.SharepointPolicyCenterSiteUrl = policyCenterSite.AbsoluteUri;
						prerequisite.SharepointSuccessInitializedUtc = DateTime.UtcNow.ToString();
						return;
					}
				}
				Uri uri = spPolicyCenterSite.GeneratePolicyCenterSiteUri((num2 == 0) ? null : new int?(num2));
				ServerException ex;
				bool flag2 = !spPolicyCenterSite.IsAnExistingSite(uri, out ex);
				base.WriteVerbose(Strings.VerboseTrytoCheckSiteExistence(uri, (ex == null) ? string.Empty : ex.Message));
				if (flag2)
				{
					flag2 = !spPolicyCenterSite.IsADeletedSite(uri, out ex);
					base.WriteVerbose(Strings.VerboseTrytoCheckSiteDeletedState(uri, (ex == null) ? string.Empty : ex.Message));
				}
				if (flag2)
				{
					base.WriteVerbose(Strings.VerboseTrytoCreatePolicyCenterSite(uri));
					spPolicyCenterSite.CreatePolicyCenterSite(uri, this.PolicyCenterSiteOwner.ToString(), num);
					flag = true;
				}
				stopwatch.Stop();
				num -= stopwatch.ElapsedMilliseconds;
				num2++;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || Utils.KnownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(exception));
		}

		private const long SpInitializationTimeoutInMilliSeconds = 3600000L;

		private const int SpMaxSalt = 999;
	}
}
