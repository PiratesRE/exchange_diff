using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ClientAccessRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetClientAccessRule : SetSystemConfigurationObjectTask<ClientAccessRuleIdParameter, ADClientAccessRule>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter DatacenterAdminsOnly { get; set; }

		protected override void InternalProcessRecord()
		{
			if (!ClientAccessRulesStorageManager.IsADRuleValid(this.DataObject))
			{
				base.WriteError(new InvalidOperationException(RulesTasksStrings.ClientAccessRulesAuthenticationTypeInvalid), ErrorCategory.InvalidOperation, null);
			}
			if (this.DataObject.IsModified(ADObjectSchema.Name) || this.DataObject.IsModified(ADClientAccessRuleSchema.Priority))
			{
				List<ADClientAccessRule> list = new List<ADClientAccessRule>(ClientAccessRulesStorageManager.GetClientAccessRules((IConfigurationSession)base.DataSession));
				if (this.DataObject.IsModified(ADObjectSchema.Name))
				{
					foreach (ADClientAccessRule adclientAccessRule in list)
					{
						if (!adclientAccessRule.Identity.Equals(this.DataObject.Identity) && adclientAccessRule.Name.Equals(this.DataObject.Name))
						{
							base.WriteError(new InvalidOperationException(RulesTasksStrings.ClientAccessRulesNameAlreadyInUse), ErrorCategory.InvalidOperation, null);
						}
					}
				}
				if (this.DataObject.IsModified(ADClientAccessRuleSchema.Priority))
				{
					bool flag = false;
					ClientAccessRulesPriorityManager clientAccessRulesPriorityManager = new ClientAccessRulesPriorityManager(list);
					this.DataObject.InternalPriority = clientAccessRulesPriorityManager.GetInternalPriority(this.DataObject.Priority, this.DataObject, out flag);
					if (flag)
					{
						ClientAccessRulesStorageManager.SaveRules((IConfigurationSession)base.DataSession, clientAccessRulesPriorityManager.ADClientAccessRules);
					}
				}
			}
			base.InternalProcessRecord();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!this.DatacenterAdminsOnly.IsPresent && this.DataObject.DatacenterAdminsOnly)
			{
				base.WriteError(new InvalidOperationException(RulesTasksStrings.ClientAccessRuleSetDatacenterAdminsOnlyError), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return RulesTasksStrings.ConfirmationMessageSetClientAccessRule(this.Identity.ToString());
			}
		}
	}
}
