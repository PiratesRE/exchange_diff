using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class GroupProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			string text2 = null;
			try
			{
				PSObject psObject = base.CreateMailbox(powershell, "mailbox");
				text = psObject.GetStringValue("Name");
				PSObject psObject2 = base.NewDistributionGroup(powershell, text2, text);
				text2 = psObject2.GetStringValue("Name");
				string text3 = "rename_" + text2;
				Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("Set-Group")
				{
					Parameters = 
					{
						{
							"Identity",
							text2
						},
						{
							"DisplayName",
							text3
						}
					}
				});
				collection = base.ExecuteCmdlet(powershell, new Command("Get-Group")
				{
					Parameters = 
					{
						{
							"Identity",
							text2
						}
					}
				});
				if (collection.Count <= 0)
				{
					throw new ApplicationException("Get-Group didn't return any result");
				}
				string text4 = (string)collection[0].Properties["DisplayName"].Value;
				if (string.Compare(text3, text4, true) != 0)
				{
					throw new ApplicationException(string.Format("DisplayName is not set successfully. Expected:{0} Actual:{1}", text3, text4));
				}
			}
			finally
			{
				base.RemoveObject(powershell, "Mailbox", text);
				base.RemoveObject(powershell, "DistributionGroup", text2);
			}
		}
	}
}
