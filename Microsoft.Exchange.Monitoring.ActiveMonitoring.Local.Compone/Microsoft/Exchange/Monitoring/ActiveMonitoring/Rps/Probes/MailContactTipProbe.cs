using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class MailContactTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			try
			{
				string uniqueName = base.GetUniqueName("MailContact");
				Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("New-MailContact")
				{
					Parameters = 
					{
						{
							"Name",
							uniqueName
						},
						{
							"ExternalEmailAddress",
							uniqueName + "@external.com"
						}
					}
				});
				if (collection.Count != 1)
				{
					throw new ApplicationException("New-MailContact returns no result");
				}
				text = uniqueName;
				string text2 = Guid.NewGuid().ToString();
				base.ExecuteCmdlet(powershell, new Command("Set-MailContact")
				{
					Parameters = 
					{
						{
							"Identity",
							text
						},
						{
							"CustomAttribute1",
							text2
						}
					}
				});
				collection = base.ExecuteCmdlet(powershell, new Command("Get-MailContact")
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
					throw new ApplicationException("Get-MailContact returns no result");
				}
				string stringValue = collection[0].GetStringValue("CustomAttribute1");
				if (string.Compare(text2, stringValue) != 0)
				{
					throw new ApplicationException(string.Format("Set-MailContact failed, Expected CustomAttribute1={0} Actual = {1}", text2, stringValue));
				}
			}
			finally
			{
				base.RemoveObject(powershell, "MailContact", text);
			}
		}
	}
}
