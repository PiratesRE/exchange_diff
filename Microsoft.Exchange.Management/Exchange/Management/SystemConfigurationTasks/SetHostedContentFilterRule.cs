using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "HostedContentFilterRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetHostedContentFilterRule : SetHygieneFilterRuleTaskBase<HostedContentFilterRule>
	{
		public SetHostedContentFilterRule() : base("HostedContentFilterVersioned")
		{
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetHostedContentFilterRule(this.Identity.ToString());
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

		[Parameter(Mandatory = false)]
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

		internal override HygieneFilterRule CreateTaskRuleFromInternalRule(TransportRule internalRule, int priority)
		{
			return HostedContentFilterRule.CreateFromInternalRule(internalRule, priority, this.DataObject);
		}

		internal override ADIdParameter GetPolicyIdentity()
		{
			if (this.HostedContentFilterPolicy != null && this.policyObject != null)
			{
				return new HostedContentFilterPolicyIdParameter(this.policyObject.Name);
			}
			return null;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.HostedContentFilterPolicy != null)
			{
				this.policyObject = HygieneUtils.ResolvePolicyObject<HostedContentFilterPolicy>(this, base.DataSession, this.HostedContentFilterPolicy);
				this.effectivePolicyObject = this.policyObject;
				TransportRule transportRule = HygieneUtils.ResolvePolicyRuleObject<HostedContentFilterPolicy>((HostedContentFilterPolicy)this.policyObject, base.DataSession, this.ruleCollectionName);
				if (transportRule != null)
				{
					base.WriteError(new OperationNotAllowedException(Strings.ErrorPolicyRuleExists(this.policyObject.Name, transportRule.Name)), ErrorCategory.InvalidOperation, null);
				}
				if (this.policyObject != null && ((HostedContentFilterPolicy)this.policyObject).IsDefault)
				{
					base.WriteError(new OperationNotAllowedException(Strings.ErrorDefaultPolicyCannotHaveRule(this.policyObject.Name)), ErrorCategory.InvalidOperation, null);
					return;
				}
			}
			else
			{
				HostedContentFilterRule hostedContentFilterRule = HostedContentFilterRule.CreateFromInternalRule((TransportRule)TransportRuleParser.Instance.GetRule(this.DataObject.Xml), -1, this.DataObject);
				this.effectivePolicyObject = HygieneUtils.ResolvePolicyObject<HostedContentFilterPolicy>(this, base.DataSession, hostedContentFilterRule.HostedContentFilterPolicy);
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			FfoDualWriter.SaveToFfo<TransportRule>(this, this.DataObject, TenantSettingSyncLogType.DUALSYNCTR, null);
		}
	}
}
