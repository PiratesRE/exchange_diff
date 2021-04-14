using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Enable", "HostedContentFilterRule", SupportsShouldProcess = true)]
	public sealed class EnableHostedContentFilterRule : EnableHygieneFilterRuleTaskBase
	{
		public EnableHostedContentFilterRule() : base("HostedContentFilterVersioned")
		{
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageEnableHostedContentFilterRule(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			FfoDualWriter.SaveToFfo<TransportRule>(this, this.DataObject, TenantSettingSyncLogType.DUALSYNCTR, null);
		}
	}
}
