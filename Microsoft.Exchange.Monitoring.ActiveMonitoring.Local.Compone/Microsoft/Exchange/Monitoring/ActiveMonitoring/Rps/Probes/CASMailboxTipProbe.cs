using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class CASMailboxTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			try
			{
				PSObject psObject = base.CreateMailbox(powershell, "casMailbox");
				text = psObject.GetStringValue("Name");
				base.ExecuteCmdlet(powershell, new Command("Set-CASMailbox")
				{
					Parameters = 
					{
						{
							"Identity",
							text
						},
						{
							"OWAEnabled",
							false
						}
					}
				});
				Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("Get-CASMailbox")
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
					throw new ApplicationException("New-Mailbox return no result");
				}
				bool propertyValue = collection[0].GetPropertyValue("OWAEnabled", true);
				if (propertyValue)
				{
					throw new ApplicationException("Set-CASMailbox failed. Expected OWAEnabled =false, Actual = true");
				}
			}
			finally
			{
				base.RemoveObject(powershell, "Mailbox", text);
			}
		}
	}
}
