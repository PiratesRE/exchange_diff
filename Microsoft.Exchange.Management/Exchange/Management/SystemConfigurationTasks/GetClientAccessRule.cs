using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ClientAccessRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class GetClientAccessRule : GetMultitenancySystemConfigurationObjectTask<ClientAccessRuleIdParameter, ADClientAccessRule>
	{
		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			Dictionary<ADObjectId, bool> dictionary = new Dictionary<ADObjectId, bool>();
			ClientAccessRulesPriorityManager clientAccessRulesPriorityManager = new ClientAccessRulesPriorityManager(ClientAccessRulesStorageManager.GetClientAccessRules((IConfigurationSession)base.DataSession));
			if (this.Identity != null)
			{
				foreach (ADClientAccessRule adclientAccessRule in ((IEnumerable<ADClientAccessRule>)dataObjects))
				{
					dictionary.Add(adclientAccessRule.Id, true);
				}
			}
			int num = 1;
			foreach (ADClientAccessRule adclientAccessRule2 in clientAccessRulesPriorityManager.ADClientAccessRules)
			{
				adclientAccessRule2.Priority = num++;
				if (this.Identity == null || dictionary.ContainsKey(adclientAccessRule2.Id))
				{
					this.WriteResult(adclientAccessRule2);
				}
			}
		}
	}
}
