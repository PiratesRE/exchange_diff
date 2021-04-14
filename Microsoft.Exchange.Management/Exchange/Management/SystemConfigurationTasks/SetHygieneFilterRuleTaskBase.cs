using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class SetHygieneFilterRuleTaskBase<TPublicObject> : SetSystemConfigurationObjectTask<RuleIdParameter, TPublicObject, TransportRule> where TPublicObject : IConfigurable, new()
	{
		protected SetHygieneFilterRuleTaskBase(string ruleCollectionName)
		{
			this.ruleCollectionName = ruleCollectionName;
			this.Priority = 0;
			base.Fields.ResetChangeTracking();
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
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
		public int Priority
		{
			get
			{
				return (int)base.Fields["Priority"];
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(Strings.NegativePriority);
				}
				base.Fields["Priority"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comments
		{
			get
			{
				return (string)base.Fields["Comments"];
			}
			set
			{
				base.Fields["Comments"] = value;
			}
		}

		public TransportRulePredicate[] Conditions
		{
			get
			{
				return (TransportRulePredicate[])base.Fields["Conditions"];
			}
			set
			{
				base.Fields["Conditions"] = value;
			}
		}

		public TransportRulePredicate[] Exceptions
		{
			get
			{
				return (TransportRulePredicate[])base.Fields["Exceptions"];
			}
			set
			{
				base.Fields["Exceptions"] = value;
			}
		}

		internal abstract HygieneFilterRule CreateTaskRuleFromInternalRule(TransportRule internalRule, int priority);

		internal abstract ADIdParameter GetPolicyIdentity();

		protected override void InternalValidate()
		{
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = RuleIdParameter.GetRuleCollectionRdn(this.ruleCollectionName);
			}
			this.DataObject = (TransportRule)this.ResolveDataObject();
			if (this.DataObject != null)
			{
				base.CurrentOrganizationId = this.DataObject.OrganizationId;
			}
			if (!this.DataObject.OrganizationId.Equals(OrganizationId.ForestWideOrgId) && !((IDirectorySession)base.DataSession).SessionSettings.CurrentOrganizationId.Equals(this.DataObject.OrganizationId))
			{
				base.UnderscopeDataSession(this.DataObject.OrganizationId);
			}
			if (base.HasErrors)
			{
				return;
			}
			if (!Utils.IsChildOfRuleContainer(this.Identity, this.ruleCollectionName))
			{
				throw new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound((this.Identity != null) ? this.Identity.ToString() : null, typeof(RuleIdParameter).ToString(), (base.DataSession != null) ? base.DataSession.Source : null));
			}
			Exception exception;
			string target;
			if (!Utils.ValidateRecipientIdParameters(base.Fields, base.TenantGlobalCatalogSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), out exception, out target))
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, target);
			}
			try
			{
				Utils.BuildConditionsAndExceptionsFromParameters(base.Fields, base.TenantGlobalCatalogSession, base.DataSession, false, out this.conditionTypesToUpdate, out this.exceptionTypesToUpdate, out this.conditionsSetByParameters, out this.exceptionsSetByParameters);
			}
			catch (TransientException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, this.Name);
			}
			catch (DataValidationException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, this.Name);
			}
			catch (ArgumentException exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidArgument, this.Name);
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				ADRuleStorageManager adruleStorageManager = new ADRuleStorageManager(this.ruleCollectionName, base.DataSession);
				adruleStorageManager.LoadRuleCollection();
				if (base.Fields.IsModified("Priority"))
				{
					this.SetRuleWithPriorityChange(adruleStorageManager);
				}
				else
				{
					this.SetRuleWithoutPriorityChange(adruleStorageManager);
				}
			}
			catch (RuleCollectionNotInAdException)
			{
				base.WriteError(new ArgumentException(Strings.RuleNotFound(this.Identity.ToString()), "Identity"), ErrorCategory.InvalidArgument, this.Identity);
			}
			catch (ParserException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
			}
			catch (ArgumentException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
			}
			catch (InvalidPriorityException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, null);
			}
		}

		private void SetRuleWithPriorityChange(ADRuleStorageManager storedRules)
		{
			TransportRule transportRule;
			int priority;
			storedRules.GetRule(this.DataObject.Identity, out transportRule, out priority);
			if (transportRule == null)
			{
				base.WriteError(new ArgumentException(Strings.RuleNotFound(this.Identity.ToString()), "Identity"), ErrorCategory.InvalidArgument, this.Identity);
			}
			HygieneFilterRule hygieneFilterRule = this.CreateTaskRuleFromInternalRule(transportRule, priority);
			this.UpdateRuleFromParameters(hygieneFilterRule);
			this.ValidateRuleEsnCompatibility(hygieneFilterRule);
			transportRule = hygieneFilterRule.ToInternalRule();
			try
			{
				storedRules.UpdateRule(transportRule, hygieneFilterRule.Identity, hygieneFilterRule.Priority);
			}
			catch (RulesValidationException)
			{
				base.WriteError(new ArgumentException(Strings.RuleNameAlreadyExist, "Name"), ErrorCategory.InvalidArgument, this.Name);
			}
		}

		private void SetRuleWithoutPriorityChange(ADRuleStorageManager storedRules)
		{
			TransportRule transportRule = (TransportRule)TransportRuleParser.Instance.GetRule(this.DataObject.Xml);
			HygieneFilterRule hygieneFilterRule = this.CreateTaskRuleFromInternalRule(transportRule, -1);
			this.UpdateRuleFromParameters(hygieneFilterRule);
			this.ValidateRuleEsnCompatibility(hygieneFilterRule);
			transportRule = hygieneFilterRule.ToInternalRule();
			this.DataObject.Xml = TransportRuleSerializer.Instance.SaveRuleToString(transportRule);
			if (base.Fields.IsModified("Name") && !storedRules.CanRename((ADObjectId)this.DataObject.Identity, ((ADObjectId)this.DataObject.Identity).Name, transportRule.Name))
			{
				base.WriteError(new ArgumentException(Strings.RuleNameAlreadyExist, "Name"), ErrorCategory.InvalidArgument, this.Name);
			}
			base.InternalProcessRecord();
		}

		private void UpdateRuleFromParameters(HygieneFilterRule rule)
		{
			if (this.Name != null)
			{
				rule.Name = this.Name;
			}
			if (this.Comments != null)
			{
				rule.Comments = this.Comments;
			}
			if (base.Fields.IsModified("Priority"))
			{
				rule.Priority = this.Priority;
			}
			if (this.Conditions != null)
			{
				rule.Conditions = this.Conditions;
			}
			else
			{
				List<TransportRulePredicate> list = new List<TransportRulePredicate>();
				if (rule.Conditions != null)
				{
					foreach (TransportRulePredicate transportRulePredicate in rule.Conditions)
					{
						if (!this.conditionTypesToUpdate.Contains(transportRulePredicate.GetType()))
						{
							Utils.InsertPredicateSorted(transportRulePredicate, list);
						}
					}
				}
				foreach (TransportRulePredicate predicate in this.conditionsSetByParameters)
				{
					Utils.InsertPredicateSorted(predicate, list);
				}
				rule.Conditions = ((list.Count > 0) ? list.ToArray() : null);
				if (rule.Conditions == null)
				{
					base.WriteError(new ArgumentException(Strings.ErrorCannotCreateRuleWithoutCondition), ErrorCategory.InvalidArgument, this.Name);
				}
			}
			if (this.Exceptions != null)
			{
				rule.Exceptions = this.Exceptions;
			}
			else
			{
				List<TransportRulePredicate> list2 = new List<TransportRulePredicate>();
				if (rule.Exceptions != null)
				{
					foreach (TransportRulePredicate transportRulePredicate2 in rule.Exceptions)
					{
						if (!this.exceptionTypesToUpdate.Contains(transportRulePredicate2.GetType()))
						{
							Utils.InsertPredicateSorted(transportRulePredicate2, list2);
						}
					}
				}
				foreach (TransportRulePredicate predicate2 in this.exceptionsSetByParameters)
				{
					Utils.InsertPredicateSorted(predicate2, list2);
				}
				if (list2.Count > 0)
				{
					rule.Exceptions = list2.ToArray();
				}
				else
				{
					rule.Exceptions = null;
				}
			}
			if (this.policyObject != null)
			{
				rule.SetPolicyId(this.GetPolicyIdentity());
			}
		}

		private void ValidateRuleEsnCompatibility(HygieneFilterRule rule)
		{
			if (rule is HostedContentFilterRule && ((HostedContentFilterPolicy)this.effectivePolicyObject).EnableEndUserSpamNotifications && !((HostedContentFilterRule)rule).IsEsnCompatible)
			{
				base.WriteError(new OperationNotAllowedException(Strings.ErrorCannotScopeEsnPolicy(this.effectivePolicyObject.Name)), ErrorCategory.InvalidOperation, null);
			}
		}

		private TransportRulePredicate[] conditionsSetByParameters;

		private TransportRulePredicate[] exceptionsSetByParameters;

		private List<Type> conditionTypesToUpdate;

		private List<Type> exceptionTypesToUpdate;

		protected readonly string ruleCollectionName;

		protected ADConfigurationObject policyObject;

		protected ADConfigurationObject effectivePolicyObject;
	}
}
