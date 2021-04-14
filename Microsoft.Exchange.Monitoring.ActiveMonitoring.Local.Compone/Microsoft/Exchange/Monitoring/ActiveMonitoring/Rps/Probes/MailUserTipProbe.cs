using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class MailUserTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			string text2 = null;
			try
			{
				PSObject psObject = base.CreateMailbox(powershell, "mailbox");
				text = psObject.GetStringValue("Name");
				string uniqueName = base.GetUniqueName("MailUser");
				Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("New-MailUser")
				{
					Parameters = 
					{
						{
							"Name",
							uniqueName
						},
						{
							base.LiveIDParameterName,
							uniqueName + "@" + base.DomainName
						},
						{
							"ExternalEmailAddress",
							uniqueName + "@" + base.DomainName
						},
						{
							"Password",
							base.Definition.AccountPassword.ConvertToSecureString()
						}
					}
				});
				if (collection.Count <= 0)
				{
					throw new ApplicationException("New-MailUser returns no result");
				}
				text2 = uniqueName;
				base.ExecuteCmdlet(powershell, new Command("Set-MailUser")
				{
					Parameters = 
					{
						{
							"Identity",
							text2
						},
						{
							"ModerationEnabled",
							true
						},
						{
							"ModeratedBy",
							text
						}
					}
				});
				collection = base.ExecuteCmdlet(powershell, new Command("Get-MailUser")
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
					throw new ApplicationException("Get-MailUser returns no result");
				}
				bool propertyValue = collection[0].GetPropertyValue("ModerationEnabled", false);
				string stringValue = collection[0].GetStringValue("ModeratedBy");
				if (string.IsNullOrEmpty(stringValue) || !stringValue.Contains(text) || !propertyValue)
				{
					throw new ApplicationException(string.Format("Set-MailUser failed, Expected ModerationEnabled=true, ModeratedBy={0}, Actual ModerationEnabled={1}, ModeratedBy={2}", base.Definition.Account, propertyValue, stringValue));
				}
			}
			finally
			{
				base.RemoveObject(powershell, "MailUser", text2);
				base.RemoveObject(powershell, "Mailbox", text);
			}
		}
	}
}
