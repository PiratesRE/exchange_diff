using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class DistributionGroupProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			try
			{
				PSObject psObject = base.NewDistributionGroup(powershell, "DG");
				text = psObject.GetStringValue("Name");
				string text2 = "rename_" + text;
				Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("Set-DistributionGroup")
				{
					Parameters = 
					{
						{
							"Identity",
							text
						},
						{
							"DisplayName",
							text2
						}
					}
				});
				collection = base.ExecuteCmdlet(powershell, new Command("Get-DistributionGroup")
				{
					Parameters = 
					{
						{
							"Identity",
							text
						}
					}
				});
				if (collection.Count <= 0)
				{
					throw new ApplicationException("Get-DistributionGroup didn't return any result");
				}
				string text3 = (string)collection[0].Properties["DisplayName"].Value;
				if (string.Compare(text2, text3, true) != 0)
				{
					throw new ApplicationException(string.Format("DisplayName is not set successfully. Expected:{0} Actual:{1}", text2, text3));
				}
			}
			finally
			{
				base.RemoveObject(powershell, "DistributionGroup", text);
			}
		}
	}
}
