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
using Microsoft.Exchange.Management.RightsManagement;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Management.OutlookProtectionRules
{
	[Cmdlet("Set", "OutlookProtectionRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetOutlookProtectionRule : SetSystemConfigurationObjectTask<RuleIdParameter, OutlookProtectionRulePresentationObject, TransportRule>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetOutlookProtectionRule(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		[ValidateNotNullOrEmpty]
		public RmsTemplateIdParameter ApplyRightsProtectionTemplate
		{
			get
			{
				return (RmsTemplateIdParameter)base.Fields["ApplyRightsProtectionTemplate"];
			}
			set
			{
				base.Fields["ApplyRightsProtectionTemplate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return this.force;
			}
			set
			{
				this.force = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] FromDepartment
		{
			get
			{
				return (string[])base.Fields["FromDepartment"];
			}
			set
			{
				base.Fields["FromDepartment"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateRange(0, 2147483647)]
		public int Priority
		{
			get
			{
				return (int)base.Fields["Priority"];
			}
			set
			{
				base.Fields["Priority"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientIdParameter> SentTo
		{
			get
			{
				return (MultiValuedProperty<RecipientIdParameter>)base.Fields["SentTo"];
			}
			set
			{
				base.Fields["SentTo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ToUserScope SentToScope
		{
			get
			{
				return (ToUserScope)base.Fields["SentToScope"];
			}
			set
			{
				base.Fields["SentToScope"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UserCanOverride
		{
			get
			{
				return (bool)base.Fields["UserCanOverride"];
			}
			set
			{
				base.Fields["UserCanOverride"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigDataProvider configDataProvider = base.CreateSession();
			this.priorityHelper = new PriorityHelper(configDataProvider);
			return configDataProvider;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = RuleIdParameter.GetRuleCollectionRdn("OutlookProtectionRules");
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.RuleNameAlreadyInUse())
			{
				base.WriteError(new OutlookProtectionRuleNameIsNotUniqueException(this.DataObject.Name), (ErrorCategory)1000, this.DataObject);
			}
			if (this.IsParameterSpecified("Priority") && !this.IsPriorityValid(this.Priority))
			{
				base.WriteError(new OutlookProtectionRuleInvalidPriorityException(), (ErrorCategory)1000, this.DataObject);
			}
			if (this.IsParameterSpecified("ApplyRightsProtectionTemplate"))
			{
				this.ResolveTemplate(this.DataObject);
			}
			TaskLogger.LogExit();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			TransportRule transportRule = (TransportRule)dataObject;
			if (!Utils.IsChildOfOutlookProtectionRuleContainer(this.Identity))
			{
				throw new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound((this.Identity != null) ? this.Identity.ToString() : null, typeof(RuleIdParameter).ToString(), (base.DataSession != null) ? base.DataSession.Source : null));
			}
			OutlookProtectionRulePresentationObject outlookProtectionRulePresentationObject = new OutlookProtectionRulePresentationObject(transportRule);
			if (this.IsParameterSpecified("ApplyRightsProtectionTemplate"))
			{
				outlookProtectionRulePresentationObject.ApplyRightsProtectionTemplate = this.ResolveTemplate(transportRule);
			}
			if (this.IsParameterSpecified("FromDepartment"))
			{
				outlookProtectionRulePresentationObject.FromDepartment = this.FromDepartment;
			}
			if (this.IsParameterSpecified("Name"))
			{
				outlookProtectionRulePresentationObject.Name = this.Name;
			}
			if (this.IsParameterSpecified("Priority"))
			{
				outlookProtectionRulePresentationObject.Priority = this.Priority;
				transportRule.Priority = this.GetSequenceNumberForPriority(transportRule, outlookProtectionRulePresentationObject.Priority);
			}
			if (this.IsParameterSpecified("SentTo"))
			{
				if (this.SentTo == null)
				{
					outlookProtectionRulePresentationObject.SentTo = null;
				}
				else if (!this.SentTo.IsChangesOnlyCopy)
				{
					outlookProtectionRulePresentationObject.SentTo = this.ResolveRecipients(this.SentTo).ToArray<SmtpAddress>();
				}
				else
				{
					HashSet<SmtpAddress> first = new HashSet<SmtpAddress>(outlookProtectionRulePresentationObject.SentTo);
					IEnumerable<SmtpAddress> second = this.ResolveRecipients(this.SentTo.Added.Cast<RecipientIdParameter>());
					IEnumerable<SmtpAddress> second2 = this.ResolveRecipients(this.SentTo.Removed.Cast<RecipientIdParameter>());
					outlookProtectionRulePresentationObject.SentTo = first.Union(second).Except(second2).ToArray<SmtpAddress>();
				}
			}
			if (this.IsParameterSpecified("SentToScope"))
			{
				outlookProtectionRulePresentationObject.SentToScope = this.SentToScope;
			}
			if (this.IsParameterSpecified("UserCanOverride"))
			{
				outlookProtectionRulePresentationObject.UserCanOverride = this.UserCanOverride;
			}
			transportRule.Name = outlookProtectionRulePresentationObject.Name;
			transportRule.Xml = outlookProtectionRulePresentationObject.Serialize();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (Utils.IsEmptyCondition(this.DataObject) && !this.Force && !base.ShouldContinue(Strings.ConfirmationMessageOutlookProtectionRuleWithEmptyCondition(this.DataObject.Name)))
			{
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(ParserException).IsInstanceOfType(exception) || RmsUtil.IsKnownException(exception);
		}

		private bool IsParameterSpecified(string parameterName)
		{
			return base.Fields.IsModified(parameterName);
		}

		private int GetSequenceNumberForPriority(TransportRule rule, int newPriority)
		{
			return this.priorityHelper.GetSequenceNumberToUpdatePriority(rule, newPriority);
		}

		private bool RuleNameAlreadyInUse()
		{
			IConfigurable configurable = base.DataSession.Read<TransportRule>(Utils.GetRuleId(base.DataSession, this.DataObject.Name));
			return configurable != null && !configurable.Identity.Equals(this.DataObject.Identity);
		}

		private bool IsPriorityValid(int priority)
		{
			return this.priorityHelper.IsPriorityValidForUpdate(priority);
		}

		private RmsTemplateIdentity ResolveTemplate(ADObject dataObject)
		{
			string name = (this.ApplyRightsProtectionTemplate != null) ? this.ApplyRightsProtectionTemplate.ToString() : string.Empty;
			if (TaskHelper.ShouldUnderscopeDataSessionToOrganization((IDirectorySession)base.DataSession, dataObject))
			{
				base.UnderscopeDataSession(dataObject.OrganizationId);
			}
			RmsTemplateDataProvider session = new RmsTemplateDataProvider((IConfigurationSession)base.DataSession, RmsTemplateType.Distributed, true);
			RmsTemplatePresentation rmsTemplatePresentation = (RmsTemplatePresentation)base.GetDataObject<RmsTemplatePresentation>(this.ApplyRightsProtectionTemplate, session, null, new LocalizedString?(Strings.OutlookProtectionRuleRmsTemplateNotFound(name)), new LocalizedString?(Strings.OutlookProtectionRuleRmsTemplateNotUnique(name)));
			return (RmsTemplateIdentity)rmsTemplatePresentation.Identity;
		}

		private IEnumerable<SmtpAddress> ResolveRecipients(IEnumerable<RecipientIdParameter> recipients)
		{
			LocalizedException exception;
			IEnumerable<SmtpAddress> enumerable = Utils.ResolveRecipientIdParameters(base.TenantGlobalCatalogSession, recipients, out exception);
			if (enumerable == null)
			{
				base.WriteError(exception, (ErrorCategory)1000, this.DataObject);
			}
			return enumerable;
		}

		private PriorityHelper priorityHelper;

		private SwitchParameter force;
	}
}
