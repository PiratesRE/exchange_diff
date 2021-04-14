using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.RightsManagement;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.OutlookProtectionRules
{
	[Serializable]
	public sealed class OutlookProtectionRulePresentationObject : RulePresentationObjectBase
	{
		public OutlookProtectionRulePresentationObject(TransportRule transportRule) : base(transportRule)
		{
			if (transportRule == null || string.IsNullOrEmpty(transportRule.Xml))
			{
				return;
			}
			OutlookProtectionRule outlookProtectionRule = (OutlookProtectionRule)OutlookProtectionRuleParser.Instance.GetRule(transportRule.Xml);
			this.enabled = (outlookProtectionRule.Enabled == RuleState.Enabled);
			this.userCanOverride = outlookProtectionRule.UserOverridable;
			PredicateCondition senderDepartmentPredicate = outlookProtectionRule.GetSenderDepartmentPredicate();
			if (senderDepartmentPredicate != null && senderDepartmentPredicate.Value != null)
			{
				this.fromDepartment = new List<string>(senderDepartmentPredicate.Value.RawValues);
			}
			PredicateCondition recipientIsPredicate = outlookProtectionRule.GetRecipientIsPredicate();
			if (recipientIsPredicate != null && recipientIsPredicate.Value != null)
			{
				this.sentTo = new List<SmtpAddress>(from s in recipientIsPredicate.Value.RawValues
				select SmtpAddress.Parse(s));
			}
			PredicateCondition allInternalPredicate = outlookProtectionRule.GetAllInternalPredicate();
			if (allInternalPredicate != null)
			{
				this.sentToScope = ToUserScope.InOrganization;
			}
			RightsProtectMessageAction rightsProtectMessageAction = outlookProtectionRule.GetRightsProtectMessageAction();
			if (rightsProtectMessageAction != null)
			{
				this.applyRightsProtectionTemplate = new RmsTemplateIdentity(new Guid(rightsProtectMessageAction.TemplateId), rightsProtectMessageAction.TemplateName);
			}
		}

		public OutlookProtectionRulePresentationObject() : this(null)
		{
		}

		public RmsTemplateIdentity ApplyRightsProtectionTemplate
		{
			get
			{
				return this.applyRightsProtectionTemplate;
			}
			internal set
			{
				this.applyRightsProtectionTemplate = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			internal set
			{
				this.enabled = value;
			}
		}

		public string[] FromDepartment
		{
			get
			{
				if (this.fromDepartment == null)
				{
					return null;
				}
				return this.fromDepartment.ToArray();
			}
			internal set
			{
				this.fromDepartment = ((value != null) ? new List<string>(value) : null);
			}
		}

		public int Priority
		{
			get
			{
				return this.priority;
			}
			internal set
			{
				this.priority = value;
			}
		}

		public SmtpAddress[] SentTo
		{
			get
			{
				if (this.sentTo == null)
				{
					return null;
				}
				return this.sentTo.ToArray();
			}
			internal set
			{
				this.sentTo = ((value != null) ? new List<SmtpAddress>(value) : null);
			}
		}

		public ToUserScope SentToScope
		{
			get
			{
				return this.sentToScope;
			}
			internal set
			{
				this.sentToScope = value;
			}
		}

		public bool UserCanOverride
		{
			get
			{
				return this.userCanOverride;
			}
			internal set
			{
				this.userCanOverride = value;
			}
		}

		public override ValidationError[] Validate()
		{
			if (this.applyRightsProtectionTemplate == null)
			{
				return new ValidationError[]
				{
					new ObjectValidationError(Strings.OutlookProtectionRuleRmsTemplateNotSet, base.Identity, null)
				};
			}
			return OutlookProtectionRulePresentationObject.EmptyValidationErrorArray;
		}

		internal bool IsEmptyCondition()
		{
			return (this.fromDepartment == null || this.fromDepartment.Count == 0) && (this.sentTo == null || this.sentTo.Count == 0) && this.sentToScope == ToUserScope.All;
		}

		internal string Serialize()
		{
			ValidationError[] array = this.Validate();
			if (array != null && array.Length > 0)
			{
				throw new DataValidationException(array[0]);
			}
			OutlookProtectionRule outlookProtectionRule = new OutlookProtectionRule(base.Name);
			outlookProtectionRule.Enabled = (this.Enabled ? RuleState.Enabled : RuleState.Disabled);
			outlookProtectionRule.UserOverridable = this.UserCanOverride;
			outlookProtectionRule.Condition = new AndCondition
			{
				SubConditions = 
				{
					this.CreateAllInternalCondition(),
					new AndCondition
					{
						SubConditions = 
						{
							this.CreateSenderDepartmentCondition(),
							this.CreateRecipientIsCondition()
						}
					}
				}
			};
			outlookProtectionRule.Actions.Add(this.CreateRightsProtectMessageAction());
			OutlookProtectionRuleSerializer outlookProtectionRuleSerializer = new OutlookProtectionRuleSerializer();
			return outlookProtectionRuleSerializer.SaveRuleToString(outlookProtectionRule);
		}

		private Condition CreateSenderDepartmentCondition()
		{
			if (this.fromDepartment == null || this.fromDepartment.Count == 0)
			{
				return Condition.True;
			}
			ShortList<string> shortList = new ShortList<string>();
			foreach (string item in this.fromDepartment)
			{
				shortList.Add(item);
			}
			return new IsPredicate(new StringProperty("Message.Sender.Department"), shortList, new RulesCreationContext());
		}

		private Condition CreateRecipientIsCondition()
		{
			if (this.sentTo == null || this.sentTo.Count == 0)
			{
				return Condition.True;
			}
			ShortList<string> shortList = new ShortList<string>();
			foreach (SmtpAddress smtpAddress in this.sentTo)
			{
				shortList.Add(smtpAddress.ToString());
			}
			return new RecipientIsPredicate(shortList, new RulesCreationContext());
		}

		private Condition CreateAllInternalCondition()
		{
			if (this.sentToScope != ToUserScope.InOrganization)
			{
				return Condition.True;
			}
			return new AllInternalPredicate();
		}

		private Microsoft.Exchange.MessagingPolicies.Rules.Action CreateRightsProtectMessageAction()
		{
			return RightsProtectMessageAction.Create(this.applyRightsProtectionTemplate.TemplateId, this.applyRightsProtectionTemplate.TemplateName);
		}

		private static readonly ValidationError[] EmptyValidationErrorArray = new ValidationError[0];

		private RmsTemplateIdentity applyRightsProtectionTemplate;

		private bool enabled;

		private List<string> fromDepartment;

		private int priority;

		private List<SmtpAddress> sentTo;

		private ToUserScope sentToScope;

		private bool userCanOverride;
	}
}
