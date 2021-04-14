using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	[Cmdlet("remove", "journalrule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class RemoveJournalRule : RemoveRuleTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveJournalrule(this.Identity.ToString());
			}
		}

		public RemoveJournalRule() : base("JournalingVersioned")
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

		protected override void InternalProcessRecord()
		{
			if (Utils.Exchange12HubServersExist(this))
			{
				this.WriteWarning(Strings.RemoveRuleSyncAcrossDifferentVersionsNeeded);
			}
			TransportRule dataObject = base.DataObject;
			JournalingRule journalingRule;
			try
			{
				journalingRule = (JournalingRule)JournalingRuleParser.Instance.GetRule(dataObject.Xml);
			}
			catch (ParserException)
			{
				journalingRule = null;
			}
			if (journalingRule != null && journalingRule.GccRuleType != GccType.None && !this.LawfulInterception)
			{
				throw new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound((this.Identity != null) ? this.Identity.ToString() : null, typeof(RuleIdParameter).ToString(), (base.DataSession != null) ? base.DataSession.Source : null));
			}
			base.InternalProcessRecord();
		}
	}
}
