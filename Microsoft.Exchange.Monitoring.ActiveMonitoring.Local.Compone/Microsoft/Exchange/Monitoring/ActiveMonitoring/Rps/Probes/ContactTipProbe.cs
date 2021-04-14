using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class ContactTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			try
			{
				string uniqueName = base.GetUniqueName("Contact");
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
				text = collection[0].GetStringValue("Name");
				string text2 = "+861059175000";
				base.ExecuteCmdlet(powershell, new Command("Set-Contact")
				{
					Parameters = 
					{
						{
							"Identity",
							text
						},
						{
							"Manager",
							base.Definition.Account
						},
						{
							"Phone",
							text2
						}
					}
				});
				collection = base.ExecuteCmdlet(powershell, new Command("Get-Contact")
				{
					Parameters = 
					{
						{
							"Identity",
							text
						}
					}
				});
				if (collection.Count != 1)
				{
					throw new ApplicationException("Get-Contact returns no result");
				}
				string text3 = collection[0].Properties["Phone"].Value as string;
				string text4 = collection[0].Properties["Manager"].Value as string;
				if (string.Compare(text3, text2, true) != 0 || !base.Definition.Account.Contains(text4) || string.IsNullOrEmpty(text4))
				{
					throw new ApplicationException(string.Format("Set-Contact failed, expected Manager={0}, Phone={1}, actual Manager={2} Phone={3}", new object[]
					{
						base.Definition.Account,
						text2,
						text4,
						text3
					}));
				}
			}
			finally
			{
				base.RemoveObject(powershell, "MailContact", text);
			}
		}
	}
}
