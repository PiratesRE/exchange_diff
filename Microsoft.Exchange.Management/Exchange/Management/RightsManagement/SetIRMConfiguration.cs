using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Security.RightsManagement.SOAP.Server;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Cmdlet("Set", "IRMConfiguration", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetIRMConfiguration : SetMultitenancySingletonSystemConfigurationObjectTask<IRMConfiguration>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter RefreshServerCertificates
		{
			get
			{
				return (SwitchParameter)(base.Fields["RefreshServerCertificates"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RefreshServerCertificates"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageIRMConfig;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			return IRMConfiguration.Read((IConfigurationSession)base.DataSession);
		}

		protected override IConfigurable PrepareDataObject()
		{
			IRMConfiguration irmconfiguration = (IRMConfiguration)base.PrepareDataObject();
			if (this.RefreshServerCertificates)
			{
				irmconfiguration.ServerCertificatesVersion++;
			}
			return irmconfiguration;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			IRMConfiguration dataObject = this.DataObject;
			try
			{
				this.ValidateCommon();
				this.datacenter = (Datacenter.IsMicrosoftHostedOnly(true) || Datacenter.IsPartnerHostedOnly(true));
				if (this.datacenter)
				{
					this.ValidateForDC(dataObject);
				}
				else
				{
					this.ValidateForEnterprise(dataObject);
				}
			}
			catch (CannotDetermineExchangeModeException exception)
			{
				base.WriteError(SetIRMConfiguration.GetExceptionInfo(exception), ErrorCategory.InvalidOperation, base.Identity);
			}
			catch (ExchangeConfigurationException exception2)
			{
				base.WriteError(SetIRMConfiguration.GetExceptionInfo(exception2), ErrorCategory.InvalidOperation, base.Identity);
			}
			catch (RightsManagementException exception3)
			{
				base.WriteError(SetIRMConfiguration.GetExceptionInfo(exception3), ErrorCategory.InvalidOperation, base.Identity);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.DataObject.IsChanged(IRMConfigurationSchema.InternalLicensingEnabled) && !this.DataObject.InternalLicensingEnabled && !this.Force && !base.ShouldContinue(Strings.ConfirmationOnDisablingInternalLicensing))
			{
				return;
			}
			if (SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				return;
			}
			if (this.DataObject.InternalLicensingEnabled && this.DataObject.IsChanged(IRMConfigurationSchema.TransportDecryptionMandatory) && this.DataObject.TransportDecryptionSetting == TransportDecryptionSetting.Mandatory && !this.datacenter)
			{
				this.WriteWarning(Strings.WarningTransportDecryptionMandatoryRequiresSuperUser);
			}
			base.InternalProcessRecord();
		}

		private static Exception GetExceptionInfo(Exception exception)
		{
			string text = string.Empty;
			Exception ex = exception;
			int num = 0;
			while (num < 10 && ex != null)
			{
				text = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
				{
					(num == 9 || ex.InnerException == null) ? string.Empty : " ---> ",
					ex.Message,
					text
				});
				ex = ex.InnerException;
				num++;
			}
			return new Exception(text, exception);
		}

		private void ValidateCommon()
		{
			if (this.DataObject.IsChanged(IRMConfigurationSchema.InternalLicensingEnabled) && !this.DataObject.InternalLicensingEnabled)
			{
				this.ThrowIfAnyEnabledTransportRulesHaveRightsProtectMessageAction();
				this.ThrowIfAnyUmMailboxPoliciesProtectVoiceMail();
				this.ThrowIfAnyEnabledTransportRuleHaveEncryptOrDecryptAction();
			}
		}

		private void ThrowIfAnyEnabledTransportRulesHaveRightsProtectMessageAction()
		{
			IEnumerable<TransportRule> transportRulesMatchingDelegate = Utils.GetTransportRulesMatchingDelegate(this.ConfigurationSession, new Utils.TransportRuleSelectionDelegate(SetIRMConfiguration.IsTransportRuleEnabledAndHasRightsProtectMessageAction), null);
			TransportRule transportRule = transportRulesMatchingDelegate.FirstOrDefault<TransportRule>();
			if (transportRule != null)
			{
				base.WriteError(new EtrHasRmsActionException(transportRule.Name), ExchangeErrorCategory.Client, base.Identity);
			}
		}

		private static bool IsTransportRuleEnabledAndHasRightsProtectMessageAction(Microsoft.Exchange.MessagingPolicies.Rules.Rule transportRule, object delegateContext)
		{
			if (transportRule.Enabled != RuleState.Enabled)
			{
				return false;
			}
			return transportRule.Actions.Any((Microsoft.Exchange.MessagingPolicies.Rules.Action action) => action is RightsProtectMessage);
		}

		private void ThrowIfAnyEnabledTransportRuleHaveEncryptOrDecryptAction()
		{
			IEnumerable<TransportRule> transportRulesMatchingDelegate = Utils.GetTransportRulesMatchingDelegate(this.ConfigurationSession, new Utils.TransportRuleSelectionDelegate(SetIRMConfiguration.IsTransportRuleEnabledAndHasEncryptMessageOrDecryptMessageAction), null);
			TransportRule transportRule = transportRulesMatchingDelegate.FirstOrDefault<TransportRule>();
			if (transportRule != null)
			{
				base.WriteError(new EtrHasE4eActionException(transportRule.Name), ExchangeErrorCategory.Client, base.Identity);
			}
		}

		private static bool IsTransportRuleEnabledAndHasEncryptMessageOrDecryptMessageAction(Microsoft.Exchange.MessagingPolicies.Rules.Rule transportRule, object delegateContext)
		{
			if (transportRule.Enabled != RuleState.Enabled)
			{
				return false;
			}
			if (!transportRule.Actions.Any((Microsoft.Exchange.MessagingPolicies.Rules.Action action) => action is ApplyOME))
			{
				return transportRule.Actions.Any((Microsoft.Exchange.MessagingPolicies.Rules.Action action) => action is RemoveOME);
			}
			return true;
		}

		private void ThrowIfAnyUmMailboxPoliciesProtectVoiceMail()
		{
			IEnumerable<UMMailboxPolicy> enumerable = this.ConfigurationSession.FindAllPaged<UMMailboxPolicy>();
			foreach (UMMailboxPolicy ummailboxPolicy in enumerable)
			{
				if (ummailboxPolicy.ProtectAuthenticatedVoiceMail != DRMProtectionOptions.None)
				{
					base.WriteError(new UnifiedMessagingMailboxPolicyHasProtectAuthenticatedVoiceMailSetToException(ummailboxPolicy.Name, ummailboxPolicy.ProtectAuthenticatedVoiceMail), ExchangeErrorCategory.Client, base.Identity);
				}
				if (ummailboxPolicy.ProtectUnauthenticatedVoiceMail != DRMProtectionOptions.None)
				{
					base.WriteError(new UnifiedMessagingMailboxPolicyHasProtectUnauthenticatedVoiceMailSetToException(ummailboxPolicy.Name, ummailboxPolicy.ProtectUnauthenticatedVoiceMail), ExchangeErrorCategory.Client, base.Identity);
				}
			}
		}

		private void ValidateForDC(IRMConfiguration config)
		{
			if (config.InternalLicensingEnabled)
			{
				IEnumerable<RMSTrustedPublishingDomain> enumerable = ((IConfigurationSession)base.DataSession).FindPaged<RMSTrustedPublishingDomain>(this.DataObject.Id, QueryScope.OneLevel, null, null, 0);
				foreach (RMSTrustedPublishingDomain rmstrustedPublishingDomain in enumerable)
				{
					if (string.IsNullOrEmpty(rmstrustedPublishingDomain.PrivateKey))
					{
						base.WriteError(new TPDWithoutPrivateKeyException(rmstrustedPublishingDomain.Name), (ErrorCategory)1000, base.Identity);
					}
				}
				if (config.ServiceLocation == null)
				{
					base.WriteError(new NoTPDsImportedException(), ErrorCategory.InvalidOperation, base.Identity);
				}
			}
		}

		private void ValidateForEnterprise(IRMConfiguration config)
		{
			if (config.ExternalLicensingEnabled && !ExternalAuthentication.GetCurrent().Enabled)
			{
				base.WriteError(new OrganizationNotFederatedException(), ErrorCategory.InvalidOperation, base.Identity);
			}
			if (config.InternalLicensingEnabled)
			{
				Uri rmsserviceLocation = RmsClientManager.GetRMSServiceLocation(OrganizationId.ForestWideOrgId, ServiceType.Certification);
				if (rmsserviceLocation == null)
				{
					base.WriteError(new NoRMSServersFoundException(), ErrorCategory.InvalidOperation, base.Identity);
				}
				this.ValidateRmsVersion(rmsserviceLocation, ServiceType.CertificationService);
				this.ValidateRmsVersion(rmsserviceLocation, ServiceType.LicensingService);
			}
			if (!MultiValuedPropertyBase.IsNullOrEmpty(config.LicensingLocation))
			{
				foreach (Uri uri in config.LicensingLocation)
				{
					if (string.IsNullOrEmpty(RMUtil.ConvertUriToLicenseUrl(uri)))
					{
						base.WriteError(new RmsUrlIsInvalidException(uri), ErrorCategory.InvalidOperation, base.Identity);
					}
					this.ValidateRmsVersion(uri, ServiceType.LicensingService);
				}
			}
		}

		private void ValidateRmsVersion(Uri uri, ServiceType serviceType)
		{
			if (serviceType == ServiceType.LicensingService)
			{
				uri = RmsoProxyUtil.GetLicenseServerRedirectUrl(uri);
			}
			if (serviceType == ServiceType.CertificationService)
			{
				uri = RmsoProxyUtil.GetCertificationServerRedirectUrl(uri);
			}
			if ((this.DataObject.IsChanged(IRMConfigurationSchema.InternalLicensingEnabled) && this.DataObject.InternalLicensingEnabled) || (this.DataObject.IsChanged(IRMConfigurationSchema.LicensingLocation) && serviceType == ServiceType.LicensingService))
			{
				using (ServerWSManager serverWSManager = new ServerWSManager(uri, serviceType, null, null, RmsClientManagerUtils.GetLocalServerProxy(this.datacenter), RmsClientManager.AppSettings.RmsSoapQueriesTimeout))
				{
					if (serviceType == ServiceType.CertificationService && !serverWSManager.ValidateCertificationServiceVersion())
					{
						base.WriteError(new RmsVersionMismatchException(uri), ErrorCategory.InvalidOperation, base.Identity);
					}
					if (serviceType == ServiceType.LicensingService && !serverWSManager.ValidateLicensingServiceVersion())
					{
						base.WriteError(new RmsVersionMismatchException(uri), ErrorCategory.InvalidOperation, base.Identity);
					}
				}
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || RmsUtil.IsKnownException(exception);
		}

		private const string Separator = " ---> ";

		private bool datacenter;
	}
}
