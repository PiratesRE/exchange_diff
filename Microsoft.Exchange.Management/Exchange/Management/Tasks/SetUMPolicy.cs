using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Management.RightsManagement;
using Microsoft.Exchange.Management.Tasks.UM;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "UMMailboxPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetUMPolicy : SetMailboxPolicyBase<UMMailboxPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetUMMailboxPolicy(this.Identity.ToString());
			}
		}

		protected override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		protected override bool ShouldUpgradeExchangeVersion(ADObject adObject)
		{
			return true;
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public UMDialPlanIdParameter UMDialPlan
		{
			get
			{
				return (UMDialPlanIdParameter)base.Fields["UMDialPlan"];
			}
			set
			{
				base.Fields["UMDialPlan"] = value;
			}
		}

		[Parameter]
		public SwitchParameter ForceUpgrade
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForceUpgrade"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForceUpgrade"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (base.Fields.IsModified("UMDialPlan"))
				{
					UMDialPlan umdialPlan = (UMDialPlan)base.GetDataObject<UMDialPlan>(this.UMDialPlan, this.ConfigurationSession, null, new LocalizedString?(Strings.NonExistantDialPlan(this.UMDialPlan.ToString())), new LocalizedString?(Strings.MultipleDialplansWithSameId(this.UMDialPlan.ToString())));
					this.DataObject.UMDialPlan = umdialPlan.Id;
				}
				if (this.Instance.IsChanged(UMMailboxPolicySchema.AllowedInCountryOrRegionGroups))
				{
					UMDialPlan dialPlan = this.DataObject.GetDialPlan();
					LocalizedString empty = LocalizedString.Empty;
					if (!DialGroupEntry.ValidateGroup(dialPlan.ConfiguredInCountryOrRegionGroups, this.DataObject.AllowedInCountryOrRegionGroups, true, out empty))
					{
						base.WriteError(new UMMailboxPolicyValidationException(empty), ErrorCategory.WriteError, this.DataObject);
					}
				}
				if (this.Instance.IsChanged(UMMailboxPolicySchema.AllowedInternationalGroups))
				{
					UMDialPlan dialPlan2 = this.DataObject.GetDialPlan();
					LocalizedString empty2 = LocalizedString.Empty;
					if (!DialGroupEntry.ValidateGroup(dialPlan2.ConfiguredInternationalGroups, this.DataObject.AllowedInternationalGroups, false, out empty2))
					{
						base.WriteError(new UMMailboxPolicyValidationException(empty2), ErrorCategory.WriteError, this.DataObject);
					}
				}
				if ((this.Instance.IsChanged(UMMailboxPolicySchema.RequireProtectedPlayOnPhone) || this.Instance.IsChanged(UMMailboxPolicySchema.AllowSubscriberAccess) || this.Instance.IsChanged(UMMailboxPolicySchema.AllowPlayOnPhone)) && this.DataObject.RequireProtectedPlayOnPhone && !this.DataObject.AllowSubscriberAccess && !this.DataObject.AllowPlayOnPhone)
				{
					base.WriteError(new UMMailboxPolicyValidationException(Strings.InCorrectRequiredPOPSetting), ErrorCategory.WriteError, this.DataObject);
				}
				if ((this.Instance.IsChanged(UMMailboxPolicySchema.ProtectUnauthenticatedVoiceMail) && this.DataObject.ProtectUnauthenticatedVoiceMail != DRMProtectionOptions.None) || (this.Instance.IsChanged(UMMailboxPolicySchema.ProtectAuthenticatedVoiceMail) && this.DataObject.ProtectAuthenticatedVoiceMail != DRMProtectionOptions.None))
				{
					UMDialPlan dialPlan3 = this.DataObject.GetDialPlan();
					if (dialPlan3.SubscriberType == UMSubscriberType.Consumer)
					{
						base.WriteError(new UMMailboxPolicyValidationException(Strings.DRMNotAllowedForConsumerDialPlan), ErrorCategory.WriteError, this.DataObject);
					}
					try
					{
						ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, false);
						RmsClientManager.ADSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 459, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxPolicies\\UMMailboxPolicyTask.cs");
						if (!RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(base.CurrentOrganizationId))
						{
							base.WriteError(new UMMailboxPolicyValidationException(Strings.RMSServerIsNotConfigured), ErrorCategory.WriteError, this.DataObject);
						}
					}
					finally
					{
						RmsClientManager.ADSession = null;
					}
				}
				string text = (string)this.DataObject[UMMailboxPolicySchema.FaxServerURI];
				bool flag = (bool)this.DataObject[UMMailboxPolicySchema.AllowFax];
				if (this.Instance.IsChanged(UMMailboxPolicySchema.FaxServerURI) && !this.ValidateFaxServerURI(text, flag))
				{
					base.WriteError(new InvalidUMFaxServerURIValue(text), ErrorCategory.InvalidOperation, this.DataObject);
				}
				if (this.Instance.IsChanged(UMMailboxPolicySchema.AllowFax) && flag && string.IsNullOrEmpty(text))
				{
					base.WriteError(new UMMailboxPolicyValidationException(Strings.UMMailboxPolicySetAllowFaxError), ErrorCategory.InvalidOperation, this.DataObject);
				}
				if (string.IsNullOrEmpty(text) && flag)
				{
					base.WriteError(new UMMailboxPolicyValidationException(Strings.UMMailboxPolicySetFaxServerURIError), ErrorCategory.InvalidOperation, this.DataObject);
				}
				string arg;
				if (this.Instance.IsChanged(UMMailboxPolicySchema.SourceForestPolicyNames) && !this.ValidateSourceForestPolicyNames(out arg))
				{
					base.WriteError(new Exception(string.Format(Strings.SetUMMailboxPolicyDuplicateSourceForestPolicyNames, arg)), ErrorCategory.InvalidOperation, this.DataObject);
				}
				if ((this.Instance.IsChanged(UMMailboxPolicySchema.AllowVoiceMailAnalysis) || this.Instance.IsChanged(UMMailboxPolicySchema.ProtectUnauthenticatedVoiceMail)) && this.DataObject.ProtectUnauthenticatedVoiceMail == DRMProtectionOptions.All && this.DataObject.AllowVoiceMailAnalysis)
				{
					base.WriteError(new UMMailboxPolicyValidationException(Strings.IncompatibleVoiceMailProtectionAndAnalysisSettings), ErrorCategory.WriteError, this.DataObject);
				}
				if (this.Instance.IsChanged(UMMailboxPolicySchema.AllowVoiceMailAnalysis) && this.DataObject.AllowVoiceMailAnalysis)
				{
					this.WriteWarning(Strings.AllowVoiceMailAnalysisWarning);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			if (!base.IsUpgrading || this.ForceUpgrade || base.ShouldContinue(Strings.ShouldUpgradeObjectVersion("UMMailboxPolicy")))
			{
				base.InternalProcessRecord();
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || RmsUtil.IsKnownException(exception);
		}

		private bool ValidateFaxServerURI(string faxServerURI, bool allowFax)
		{
			bool result = false;
			string pattern = "\\bsip:([^:]+):([\\d]+);transport=(tcp|udp|tls)$";
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
			Match match = regex.Match(faxServerURI);
			if (match.Success)
			{
				int num;
				try
				{
					num = Convert.ToInt32(match.Groups[2].ToString());
				}
				catch (OverflowException)
				{
					return false;
				}
				if (num < 0 || num > 65535)
				{
					return result;
				}
				string address = match.Groups[1].ToString();
				if (UMSmartHost.IsValidAddress(address))
				{
					return true;
				}
				return result;
			}
			return result;
		}

		private bool ValidateSourceForestPolicyNames(out string duplicateSourceForestPolicyName)
		{
			duplicateSourceForestPolicyName = null;
			IEnumerable<UMMailboxPolicy> enumerable = ((IConfigurationSession)base.DataSession).FindAllPaged<UMMailboxPolicy>();
			foreach (UMMailboxPolicy ummailboxPolicy in enumerable)
			{
				if (!(ummailboxPolicy.Guid == this.DataObject.Guid))
				{
					foreach (string text in this.DataObject.SourceForestPolicyNames)
					{
						if (ummailboxPolicy.SourceForestPolicyNames.Contains(text))
						{
							duplicateSourceForestPolicyName = text;
							return false;
						}
					}
				}
			}
			return true;
		}
	}
}
