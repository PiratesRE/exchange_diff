using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class UserTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			try
			{
				text = base.GetUniqueName("TestMailUser");
				Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("New-MailUser")
				{
					Parameters = 
					{
						{
							"Name",
							text
						},
						{
							base.LiveIDParameterName,
							text + "@" + base.DomainName
						},
						{
							"Password",
							base.Definition.AccountPassword.ConvertToSecureString()
						}
					}
				});
				if (collection.Count <= 0)
				{
					throw new ApplicationException("New-MailUser return no result");
				}
				collection = base.ExecuteCmdlet(powershell, new Command("Get-User")
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
				string text2 = "Set DisplayName " + text;
				base.ExecuteCmdlet(powershell, new Command("Set-User")
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
				collection = base.ExecuteCmdlet(powershell, new Command("Get-User")
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
				string text3 = (string)collection[0].Properties["DisplayName"].Value;
				if (string.Compare(text2, text3, true) != 0)
				{
					throw new ApplicationException(string.Format("DisplayName is not set, Expected:{0} Actual:{1}", text2, text3));
				}
			}
			finally
			{
				base.RemoveObject(powershell, "MailUser", text);
			}
		}
	}
}
