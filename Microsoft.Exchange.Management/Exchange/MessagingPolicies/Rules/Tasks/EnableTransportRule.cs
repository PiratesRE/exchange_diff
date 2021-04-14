using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.RightsManagement;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.RightsManagement;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Cmdlet("Enable", "TransportRule", SupportsShouldProcess = true)]
	public sealed class EnableTransportRule : EnableRuleTaskBase
	{
		[Parameter(Mandatory = false)]
		public RuleMode Mode
		{
			get
			{
				return (RuleMode)base.Fields["Mode"];
			}
			set
			{
				base.Fields["Mode"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableTransportRule(this.Identity.ToString());
			}
		}

		public EnableTransportRule() : base(Utils.RuleCollectionNameFromRole())
		{
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			TransportRule dataObject = this.DataObject;
			if (dataObject == null)
			{
				ExAssert.RetailAssert(false, "EnableTransportRule.InternalValidate data object is invalid");
			}
			TransportRule rule = (TransportRule)TransportRuleParser.Instance.GetRule(dataObject.Xml);
			Rule rule2 = Rule.CreateFromInternalRule(TransportRulePredicate.GetAvailablePredicateMappings(), TransportRuleAction.GetAvailableActionMappings(), rule, dataObject.Priority, dataObject);
			IEnumerable<RightsProtectMessageAction> source = (from action in rule2.Actions
			where action is RightsProtectMessageAction
			select action as RightsProtectMessageAction).ToList<RightsProtectMessageAction>();
			if (source.Any<RightsProtectMessageAction>())
			{
				if (!RmsClientManager.IRMConfig.IsInternalLicensingEnabledForTenant(base.CurrentOrganizationId))
				{
					base.WriteError(new IrmLicensingIsDisabledException(), ErrorCategory.InvalidArgument, null);
				}
				RmsTemplateIdentity template = source.First<RightsProtectMessageAction>().Template;
				RmsTemplateDataProvider session = new RmsTemplateDataProvider((IConfigurationSession)base.DataSession);
				base.GetDataObject<RmsTemplatePresentation>(new RmsTemplateIdParameter(template), session, null, new LocalizedString?(Strings.OutlookProtectionRuleRmsTemplateNotFound(template.TemplateName)), new LocalizedString?(Strings.OutlookProtectionRuleRmsTemplateNotUnique(template.TemplateName)));
			}
			bool flag;
			if (!rule2.Actions.Any((TransportRuleAction action) => action is ApplyOMEAction))
			{
				flag = rule2.Actions.Any((TransportRuleAction action) => action is RemoveOMEAction);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (flag2)
			{
				IRMConfiguration irmconfiguration = IRMConfiguration.Read((IConfigurationSession)base.DataSession);
				if (irmconfiguration == null || !irmconfiguration.InternalLicensingEnabled)
				{
					base.WriteError(new E4eLicensingIsDisabledExceptionEnableRule(), ErrorCategory.InvalidArgument, null);
				}
				if (RmsClientManager.IRMConfig.GetRmsTemplate(base.CurrentOrganizationId, RmsTemplate.InternetConfidential.Id) == null)
				{
					base.WriteError(new E4eRuleRmsTemplateNotFoundException(RmsTemplate.InternetConfidential.Name), ErrorCategory.InvalidArgument, null);
				}
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			TransportRule transportRule = (TransportRule)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			TransportRule transportRule2;
			try
			{
				transportRule2 = (TransportRule)TransportRuleParser.Instance.GetRule(transportRule.Xml);
			}
			catch (ParserException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
				return null;
			}
			if (transportRule2.IsTooAdvancedToParse)
			{
				base.WriteError(new InvalidOperationException(Strings.CannotModifyRuleDueToVersion(transportRule2.Name)), ErrorCategory.InvalidOperation, null);
				return null;
			}
			OrganizationId organizationId = transportRule.OrganizationId;
			if (organizationId != OrganizationId.ForestWideOrgId)
			{
				ADRuleStorageManager adruleStorageManager;
				try
				{
					adruleStorageManager = new ADRuleStorageManager(base.RuleCollectionName, base.DataSession);
				}
				catch (RuleCollectionNotInAdException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
					return null;
				}
				adruleStorageManager.LoadRuleCollection();
				InvalidOperationException ex = Utils.CheckRuleForOrganizationLimits((IConfigurationSession)base.DataSession, base.TenantGlobalCatalogSession, adruleStorageManager, organizationId, transportRule2, false);
				if (ex != null)
				{
					base.WriteError(ex, ErrorCategory.InvalidOperation, null);
					return null;
				}
			}
			if (Utils.Exchange12HubServersExist(this))
			{
				this.WriteWarning(Strings.SetRuleSyncAcrossDifferentVersionsNeeded);
			}
			transportRule2.Enabled = RuleState.Enabled;
			transportRule2.Mode = (base.Fields.IsModified("Mode") ? this.Mode : RuleMode.Enforce);
			string xml = TransportRuleSerializer.Instance.SaveRuleToString(transportRule2);
			transportRule.Xml = xml;
			TaskLogger.LogExit();
			return transportRule;
		}
	}
}
