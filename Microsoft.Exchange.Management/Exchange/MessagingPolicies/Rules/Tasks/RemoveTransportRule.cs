using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Cmdlet("Remove", "TransportRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveTransportRule : RemoveRuleTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				try
				{
					TransportRule transportRule = (TransportRule)TransportRuleParser.Instance.GetRule(base.DataObject.Xml);
					Guid a;
					if (transportRule.TryGetDlpPolicyId(out a) && a != Guid.Empty)
					{
						return Strings.ConfirmationMessageRemoveTransportRuleThatBelongsToDLpPolicy(this.Identity.ToString(), a.ToString());
					}
				}
				catch (ParserException ex)
				{
					this.WriteWarning(Strings.RuleIsCorrupt(this.Identity.ToString(), ex.Message));
				}
				return Strings.ConfirmationMessageRemoveTransportRule(this.Identity.ToString());
			}
		}

		public RemoveTransportRule() : base(Utils.RuleCollectionNameFromRole())
		{
		}

		protected override void InternalProcessRecord()
		{
			if (Utils.Exchange12HubServersExist(this))
			{
				this.WriteWarning(Strings.RemoveRuleSyncAcrossDifferentVersionsNeeded);
			}
			IConfigDataProvider configDataProvider = new MessagingPoliciesSyncLogDataSession(base.DataSession, null, null);
			configDataProvider.Delete(base.DataObject);
		}
	}
}
