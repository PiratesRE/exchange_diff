using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class MailboxPermissionTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			string text2 = null;
			try
			{
				PSObject psObject = base.CreateMailbox(powershell, "mailbox1");
				text = psObject.GetStringValue("Name");
				PSObject psObject2 = base.CreateMailbox(powershell, "mailbox2");
				text2 = psObject2.GetStringValue("Name");
				psObject2.GetStringValue("SamAccountName");
				Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("Add-MailboxPermission")
				{
					Parameters = 
					{
						{
							"Identity",
							text
						},
						{
							"User",
							text2
						},
						{
							"AccessRights",
							"FullAccess"
						}
					}
				});
				if (collection.Count <= 0)
				{
					throw new ApplicationException("Add-MailboxPermission return no result");
				}
				collection = base.ExecuteCmdlet(powershell, new Command("Get-MailboxPermission")
				{
					Parameters = 
					{
						{
							"Identity",
							text
						},
						{
							"User",
							text2
						},
						{
							"ErrorAction",
							"SilentlyContinue"
						}
					}
				});
				if (collection.Count <= 0)
				{
					throw new ApplicationException("Get-MailboxPermission -Identity -User return no result");
				}
				string stringValue = collection[0].GetStringValue("AccessRights");
				if (string.Compare(stringValue, "FullAccess", true) != 0)
				{
					throw new ApplicationException(string.Format("Add-MailboxPermission failed, Expected AccessRights=FullAccess, Actual = {0}", stringValue));
				}
				base.ExecuteCmdlet(powershell, new Command("Remove-MailboxPermission")
				{
					Parameters = 
					{
						{
							"Identity",
							text
						},
						{
							"User",
							text2
						},
						{
							"AccessRights",
							"FullAccess"
						}
					}
				});
			}
			finally
			{
				base.RemoveObject(powershell, "Mailbox", text);
				base.RemoveObject(powershell, "Mailbox", text2);
			}
		}
	}
}
