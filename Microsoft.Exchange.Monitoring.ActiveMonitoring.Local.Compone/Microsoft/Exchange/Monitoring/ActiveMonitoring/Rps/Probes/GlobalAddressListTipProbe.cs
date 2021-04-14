using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class GlobalAddressListTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			try
			{
				string uniqueName = base.GetUniqueName("TestGAL");
				text = uniqueName;
				Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("New-GlobalAddressList")
				{
					Parameters = 
					{
						{
							"Name",
							uniqueName
						}
					}
				});
				if (collection.Count <= 0)
				{
					throw new ApplicationException("New-GlobalAddressList return no result");
				}
				text = "New" + uniqueName;
				base.ExecuteCmdlet(powershell, new Command("Set-GlobalAddressList")
				{
					Parameters = 
					{
						{
							"Identity",
							uniqueName
						},
						{
							"Name",
							text
						}
					}
				});
				collection = base.ExecuteCmdlet(powershell, new Command("Get-GlobalAddressList")
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
					throw new ApplicationException("Get-GlobalAddressList return no result");
				}
				string text2 = (string)collection[0].Properties["Name"].Value;
				if (string.Compare(text, text2, true) != 0)
				{
					throw new ApplicationException(string.Format("Name is not set, Expected:{0} Actual:{1}", text, text2));
				}
			}
			finally
			{
				base.RemoveObject(powershell, "GlobalAddressList", text);
			}
		}
	}
}
