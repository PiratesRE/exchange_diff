using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class MailboxTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			try
			{
				PSObject psObject = base.CreateMailbox(powershell, "mailbox");
				text = psObject.GetStringValue("Name");
				string text2 = "Set DisplayName " + text;
				Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("Set-Mailbox")
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
				collection = base.ExecuteCmdlet(powershell, new Command("Get-Mailbox")
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
					throw new ApplicationException("Get-Mailbox return no result");
				}
				string text3 = (string)collection[0].Properties["DisplayName"].Value;
				if (string.Compare(text2, text3, true) != 0)
				{
					throw new ApplicationException(string.Format("DisplayName is not set, Expected:{0} Actual:{1}", text2, text3));
				}
				collection = base.ExecuteCmdlet(powershell, new Command("Disable-Mailbox")
				{
					Parameters = 
					{
						{
							"Identity",
							text
						},
						"Archive"
					}
				});
				collection = base.ExecuteCmdlet(powershell, new Command("Get-Mailbox")
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
					throw new ApplicationException("Get-User return no result");
				}
				string text4 = (string)collection[0].Properties["ArchiveDatabase"].Value;
				if (!string.IsNullOrEmpty(text4))
				{
					throw new ApplicationException(string.Format("'Disable-Mailbox -Archive' failed, ArchiveDatabase:{0}", text4));
				}
				collection = base.ExecuteCmdlet(powershell, new Command("Enable-Mailbox")
				{
					Parameters = 
					{
						{
							"Identity",
							text
						},
						"Archive"
					}
				});
				collection = base.ExecuteCmdlet(powershell, new Command("Get-Mailbox")
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
					throw new ApplicationException("Get-User return no result");
				}
				text4 = (string)collection[0].Properties["ArchiveDatabase"].Value;
				if (string.IsNullOrEmpty(text4))
				{
					throw new ApplicationException("'Enable-Mailbox -Archive' failed, ArchiveDatabase is Empty");
				}
			}
			finally
			{
				base.RemoveObject(powershell, "Mailbox", text);
			}
		}
	}
}
