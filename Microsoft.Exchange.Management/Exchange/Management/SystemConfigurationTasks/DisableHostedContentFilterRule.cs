using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Disable", "HostedContentFilterRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableHostedContentFilterRule : DisableHygieneFilterRuleTaskBase
	{
		public DisableHostedContentFilterRule() : base("HostedContentFilterVersioned")
		{
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageDisableHostedContentFilterRule(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			FfoDualWriter.SaveToFfo<TransportRule>(this, this.DataObject, TenantSettingSyncLogType.DUALSYNCTR, null);
		}
	}
}
