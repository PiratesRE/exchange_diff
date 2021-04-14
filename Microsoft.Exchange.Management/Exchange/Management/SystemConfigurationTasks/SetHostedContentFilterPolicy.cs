using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "HostedContentFilterPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetHostedContentFilterPolicy : SetSystemConfigurationObjectTask<HostedContentFilterPolicyIdParameter, HostedContentFilterPolicy>
	{
		[Parameter]
		public SwitchParameter MakeDefault { get; set; }

		[Parameter]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetHostedContentFilterPolicy(this.Identity.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			((IConfigurationSession)base.DataSession).SessionSettings.IsSharedConfigChecked = true;
			if (!this.IgnoreDehydratedFlag)
			{
				SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (this.DataObject.LanguageBlockList != null)
			{
				foreach (string text in this.DataObject.LanguageBlockList)
				{
					if (!HygieneUtils.IsAntispamFilterableLanguage(text))
					{
						base.WriteError(new ArgumentException(Strings.ErrorUnsupportedBlockLanguage(text)), ErrorCategory.InvalidArgument, text);
					}
				}
			}
			if (this.DataObject.RegionBlockList != null)
			{
				foreach (string text2 in this.DataObject.RegionBlockList)
				{
					if (!HygieneUtils.IsValidIso3166Alpha2Code(text2))
					{
						base.WriteError(new ArgumentException(Strings.ErrorInvalidIso3166Alpha2Code(text2)), ErrorCategory.InvalidArgument, text2);
					}
				}
			}
			if (this.DataObject.IsModified(HostedContentFilterPolicySchema.EnableEndUserSpamNotifications) && this.DataObject.EnableEndUserSpamNotifications)
			{
				HostedContentFilterRule policyScopingRule = this.GetPolicyScopingRule();
				if (policyScopingRule != null && !policyScopingRule.IsEsnCompatible)
				{
					base.WriteError(new OperationNotAllowedException(Strings.ErrorEsnIncompatibleRule(policyScopingRule.Name)), ErrorCategory.InvalidOperation, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			HostedContentFilterPolicy hostedContentFilterPolicy = null;
			if (this.MakeDefault && !this.DataObject.IsDefault)
			{
				this.DataObject.IsDefault = true;
				hostedContentFilterPolicy = ((ITenantConfigurationSession)base.DataSession).GetDefaultFilteringConfiguration<HostedContentFilterPolicy>();
				if (hostedContentFilterPolicy != null && hostedContentFilterPolicy.IsDefault)
				{
					hostedContentFilterPolicy.IsDefault = false;
					base.DataSession.Save(hostedContentFilterPolicy);
					FfoDualWriter.SaveToFfo<HostedContentFilterPolicy>(this, hostedContentFilterPolicy, null);
				}
			}
			else if (base.Fields.Contains("MakeDefault") && !this.MakeDefault && this.DataObject.IsDefault)
			{
				base.WriteError(new OperationNotAllowedException(Strings.OperationNotAllowed), ErrorCategory.InvalidOperation, this.MakeDefault);
			}
			try
			{
				base.InternalProcessRecord();
				FfoDualWriter.SaveToFfo<HostedContentFilterPolicy>(this, this.DataObject, null);
				hostedContentFilterPolicy = null;
			}
			finally
			{
				if (hostedContentFilterPolicy != null)
				{
					hostedContentFilterPolicy.IsDefault = true;
					base.DataSession.Save(hostedContentFilterPolicy);
					FfoDualWriter.SaveToFfo<HostedContentFilterPolicy>(this, hostedContentFilterPolicy, null);
				}
			}
			TaskLogger.LogExit();
		}

		private HostedContentFilterRule GetPolicyScopingRule()
		{
			TransportRule transportRule = HygieneUtils.ResolvePolicyRuleObject<HostedContentFilterPolicy>(this.DataObject, this.ConfigurationSession, "HostedContentFilterVersioned");
			if (transportRule != null)
			{
				TransportRule transportRule2 = this.GetTransportRule(transportRule.Name);
				return HostedContentFilterRule.CreateFromInternalRule(transportRule, -1, transportRule2);
			}
			return null;
		}

		private TransportRule GetTransportRule(string ruleName)
		{
			ADRuleStorageManager adruleStorageManager = new ADRuleStorageManager("HostedContentFilterVersioned", base.DataSession);
			adruleStorageManager.LoadRuleCollectionWithoutParsing(new TextFilter(ADObjectSchema.Name, ruleName, MatchOptions.FullString, MatchFlags.Default));
			TransportRule result = null;
			if (adruleStorageManager.Count > 0)
			{
				adruleStorageManager.GetRuleWithoutParsing(0, out result);
			}
			return result;
		}
	}
}
