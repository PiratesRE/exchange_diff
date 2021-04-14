using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "HostedContentFilterRule", SupportsShouldProcess = true)]
	public sealed class NewHostedContentFilterRule : NewHygieneFilterRuleTaskBase
	{
		public NewHostedContentFilterRule() : base("HostedContentFilterVersioned")
		{
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewHostedContentFilterRule(base.Name);
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] SentTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["SentTo"];
			}
			set
			{
				base.Fields["SentTo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] SentToMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["SentToMemberOf"];
			}
			set
			{
				base.Fields["SentToMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] RecipientDomainIs
		{
			get
			{
				return (Word[])base.Fields["RecipientDomainIs"];
			}
			set
			{
				base.Fields["RecipientDomainIs"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfSentTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfSentTo"];
			}
			set
			{
				base.Fields["ExceptIfSentTo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] ExceptIfSentToMemberOf
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ExceptIfSentToMemberOf"];
			}
			set
			{
				base.Fields["ExceptIfSentToMemberOf"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Word[] ExceptIfRecipientDomainIs
		{
			get
			{
				return (Word[])base.Fields["ExceptIfRecipientDomainIs"];
			}
			set
			{
				base.Fields["ExceptIfRecipientDomainIs"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNull]
		public HostedContentFilterPolicyIdParameter HostedContentFilterPolicy
		{
			get
			{
				return (HostedContentFilterPolicyIdParameter)base.Fields["HostedContentFilterPolicy"];
			}
			set
			{
				base.Fields["HostedContentFilterPolicy"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			this.policyObject = HygieneUtils.ResolvePolicyObject<HostedContentFilterPolicy>(this, this.ConfigurationSession, this.HostedContentFilterPolicy);
			TransportRule transportRule = HygieneUtils.ResolvePolicyRuleObject<HostedContentFilterPolicy>(this.policyObject, this.ConfigurationSession, this.ruleCollectionName);
			if (transportRule != null)
			{
				base.WriteError(new OperationNotAllowedException(Strings.ErrorPolicyRuleExists(this.policyObject.Name, transportRule.Name)), ErrorCategory.InvalidOperation, null);
			}
			if (this.policyObject != null && this.policyObject.IsDefault)
			{
				base.WriteError(new OperationNotAllowedException(Strings.ErrorDefaultPolicyCannotHaveRule(this.policyObject.Name)), ErrorCategory.InvalidOperation, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			HostedContentFilterRule hostedContentFilterRule = new HostedContentFilterRule(null, base.Name, base.Priority, base.Enabled ? RuleState.Enabled : RuleState.Disabled, base.Comments, base.Conditions, base.Exceptions, new HostedContentFilterPolicyIdParameter(this.policyObject.Name));
			if (this.policyObject.EnableEndUserSpamNotifications && !hostedContentFilterRule.IsEsnCompatible)
			{
				base.WriteError(new OperationNotAllowedException(Strings.ErrorCannotScopeEsnPolicy(this.policyObject.Name)), ErrorCategory.InvalidOperation, null);
			}
			int priority = base.Fields.IsModified("Priority") ? hostedContentFilterRule.Priority : -1;
			TransportRule transportRule = null;
			try
			{
				TransportRule rule = hostedContentFilterRule.ToInternalRule();
				ADRuleStorageManager adruleStorageManager = new ADRuleStorageManager(this.ruleCollectionName, base.DataSession);
				adruleStorageManager.LoadRuleCollection();
				adruleStorageManager.NewRule(rule, this.ResolveCurrentOrganization(), ref priority, out transportRule);
				FfoDualWriter.SaveToFfo<TransportRule>(this, transportRule, TenantSettingSyncLogType.DUALSYNCTR, null);
			}
			catch (RulesValidationException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, base.Name);
			}
			catch (InvalidPriorityException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
			}
			catch (ParserException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidData, null);
			}
			hostedContentFilterRule.Priority = priority;
			hostedContentFilterRule.SetTransportRule(transportRule);
			base.WriteObject(hostedContentFilterRule);
		}

		private HostedContentFilterPolicy policyObject;
	}
}
