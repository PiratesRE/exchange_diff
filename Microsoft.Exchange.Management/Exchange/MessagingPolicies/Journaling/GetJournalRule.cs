using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	[Cmdlet("Get", "JournalRule", DefaultParameterSetName = "Identity")]
	public sealed class GetJournalRule : GetMultitenancySystemConfigurationObjectTask<RuleIdParameter, TransportRule>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter LawfulInterception
		{
			get
			{
				return (SwitchParameter)(base.Fields["LawfulInterception"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["LawfulInterception"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.Identity != null)
				{
					return null;
				}
				return RuleIdParameter.GetRuleCollectionId(base.DataSession, "JournalingVersioned");
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetTaskBaseModuleFactory();
		}

		protected override void InternalValidate()
		{
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = RuleIdParameter.GetRuleCollectionRdn("JournalingVersioned");
			}
			if (base.Fields.IsModified("LawfulInterception") && base.Fields.IsModified("Organization"))
			{
				base.WriteError(new InvalidOperationException(Strings.JournalingParameterErrorGccWithOrganization), ErrorCategory.InvalidOperation, null);
				return;
			}
			base.InternalValidate();
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			foreach (TransportRule transportRule in ((IEnumerable<TransportRule>)dataObjects))
			{
				this.WriteRuleObject(transportRule);
			}
		}

		private void WriteRuleObject(TransportRule transportRule)
		{
			JournalingRule journalingRule = null;
			ParserException ex = null;
			try
			{
				journalingRule = (JournalingRule)JournalingRuleParser.Instance.GetRule(transportRule.Xml);
			}
			catch (ParserException ex2)
			{
				ex = ex2;
			}
			Exception ex3 = null;
			if (journalingRule != null && journalingRule.GccRuleType != GccType.None && !this.LawfulInterception)
			{
				return;
			}
			if (journalingRule != null && journalingRule.GccRuleType == GccType.None && this.LawfulInterception)
			{
				return;
			}
			JournalRuleObject journalRuleObject;
			if (journalingRule == null)
			{
				journalRuleObject = JournalRuleObject.CreateCorruptJournalRuleObject(transportRule, Strings.CorruptRule(transportRule.Name, ex.Message));
			}
			else if (journalingRule.IsTooAdvancedToParse)
			{
				journalRuleObject = JournalRuleObject.CreateCorruptJournalRuleObject(transportRule, Strings.CannotParseRuleDueToVersion(transportRule.Name));
			}
			else
			{
				journalRuleObject = new JournalRuleObject();
				try
				{
					journalRuleObject.Deserialize(journalingRule);
				}
				catch (RecipientInvalidException ex4)
				{
					ex3 = ex4;
				}
				catch (JournalRuleCorruptException ex5)
				{
					ex3 = ex5;
				}
			}
			if (ex3 != null)
			{
				journalRuleObject = JournalRuleObject.CreateCorruptJournalRuleObject(transportRule, Strings.CorruptRule(transportRule.Name, ex3.Message));
			}
			journalRuleObject.SetTransportRule(transportRule);
			this.WriteResult(journalRuleObject);
		}
	}
}
