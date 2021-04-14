using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	[Cmdlet("enable", "journalrule", SupportsShouldProcess = true)]
	public class EnableJournalRule : EnableRuleTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableJournalrule(this.Identity.ToString());
			}
		}

		public EnableJournalRule() : base("JournalingVersioned")
		{
		}

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

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			TransportRule transportRule = (TransportRule)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			JournalingRule journalingRule;
			try
			{
				journalingRule = (JournalingRule)JournalingRuleParser.Instance.GetRule(transportRule.Xml);
			}
			catch (ParserException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidData, null);
				return null;
			}
			if (journalingRule.GccRuleType != GccType.None && !this.LawfulInterception)
			{
				LocalizedString errorMessageObjectNotFound = base.GetErrorMessageObjectNotFound((this.Identity != null) ? this.Identity.ToString() : null, typeof(RuleIdParameter).ToString(), (base.DataSession != null) ? base.DataSession.Source : null);
				base.WriteError(new ManagementObjectNotFoundException(errorMessageObjectNotFound), ErrorCategory.ObjectNotFound, null);
				return null;
			}
			if (journalingRule.IsTooAdvancedToParse)
			{
				base.WriteError(new InvalidOperationException(Strings.CannotModifyRuleDueToVersion(journalingRule.Name)), ErrorCategory.InvalidOperation, null);
				return null;
			}
			journalingRule.Enabled = RuleState.Enabled;
			string xml = JournalingRuleSerializer.Instance.SaveRuleToString(journalingRule);
			transportRule.Xml = xml;
			TaskLogger.LogExit();
			return transportRule;
		}
	}
}
