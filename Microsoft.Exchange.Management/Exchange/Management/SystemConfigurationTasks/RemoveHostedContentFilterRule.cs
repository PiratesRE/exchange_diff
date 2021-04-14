using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "HostedContentFilterRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveHostedContentFilterRule : RemoveRuleTaskBase
	{
		public RemoveHostedContentFilterRule() : base("HostedContentFilterVersioned")
		{
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveHostedContentFilterRule(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			FfoDualWriter.DeleteFromFfo<TransportRule>(this, base.DataObject, TenantSettingSyncLogType.DUALSYNCTR);
		}
	}
}
