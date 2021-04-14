using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class AddressListTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			try
			{
				text = base.GetUniqueName("TestAL");
				Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("New-AddressList")
				{
					Parameters = 
					{
						{
							"Name",
							text
						}
					}
				});
				if (collection.Count <= 0)
				{
					throw new ApplicationException("New-AddressList return no result");
				}
				string text2 = "Set DisplayName " + text;
				base.ExecuteCmdlet(powershell, new Command("Set-AddressList")
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
				collection = base.ExecuteCmdlet(powershell, new Command("Get-AddressList")
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
					throw new ApplicationException("Get-AddressList return no result");
				}
				string text3 = (string)collection[0].Properties["DisplayName"].Value;
				if (string.Compare(text2, text3, true) != 0)
				{
					throw new ApplicationException(string.Format("DisplayName is not set, Expected:{0} Actual:{1}", text2, text3));
				}
			}
			finally
			{
				base.RemoveObject(powershell, "AddressList", text);
			}
		}
	}
}
