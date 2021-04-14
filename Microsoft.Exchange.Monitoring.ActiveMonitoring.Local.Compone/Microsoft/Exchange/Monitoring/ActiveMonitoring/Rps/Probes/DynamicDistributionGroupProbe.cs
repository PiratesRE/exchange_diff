using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class DynamicDistributionGroupProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string uniqueName = base.GetUniqueName("DynamicDG");
			Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("New-DynamicDistributionGroup")
			{
				Parameters = 
				{
					{
						"Name",
						uniqueName
					},
					{
						"IncludedRecipients",
						"MailboxUsers"
					}
				}
			});
			if (collection.Count <= 0)
			{
				throw new ApplicationException("New-DynamicDistributionGroup didn't return any result");
			}
			try
			{
				string text = "Rename_" + uniqueName;
				collection = base.ExecuteCmdlet(powershell, new Command("Set-DynamicDistributionGroup")
				{
					Parameters = 
					{
						{
							"Identity",
							uniqueName
						},
						{
							"DisplayName",
							text
						}
					}
				});
				collection = base.ExecuteCmdlet(powershell, new Command("Get-DynamicDistributionGroup")
				{
					Parameters = 
					{
						{
							"Identity",
							uniqueName
						}
					}
				});
				if (collection.Count <= 0)
				{
					throw new ApplicationException("Get-DynamicDistributionGroup didn't return any result");
				}
				string text2 = (string)collection[0].Properties["DisplayName"].Value;
				if (string.Compare(text, text2, true) != 0)
				{
					throw new ApplicationException(string.Format("DisplayName is not set successfully. Expected:{0} Actual:{1}", text, text2));
				}
			}
			finally
			{
				base.RemoveObject(powershell, "DynamicDistributionGroup", uniqueName);
			}
		}
	}
}
