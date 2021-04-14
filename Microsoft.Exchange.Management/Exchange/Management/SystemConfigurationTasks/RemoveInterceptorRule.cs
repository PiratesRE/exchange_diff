using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "InterceptorRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveInterceptorRule : RemoveSystemConfigurationObjectTask<InterceptorRuleIdParameter, InterceptorRule>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveInterceptorRule(this.Identity.ToString());
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return base.RootOrgContainerId.GetDescendantId(InterceptorRule.InterceptorRulesContainer);
			}
		}
	}
}
