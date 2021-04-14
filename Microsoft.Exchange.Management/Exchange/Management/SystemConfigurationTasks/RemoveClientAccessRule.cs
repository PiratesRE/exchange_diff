using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "ClientAccessRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveClientAccessRule : RemoveSystemConfigurationObjectTask<ClientAccessRuleIdParameter, ADClientAccessRule>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter DatacenterAdminsOnly { get; set; }

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!this.DatacenterAdminsOnly.IsPresent && base.DataObject.DatacenterAdminsOnly)
			{
				base.WriteError(new InvalidOperationException(RulesTasksStrings.ClientAccessRuleRemoveDatacenterAdminsOnlyError), ErrorCategory.InvalidOperation, base.DataObject.Identity);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return RulesTasksStrings.ConfirmationMessageRemoveClientAccessRule(this.Identity.ToString());
			}
		}
	}
}
