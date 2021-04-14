using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class RecipientTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			try
			{
				PSObject psObject = base.CreateMailbox(powershell, "recipient");
				text = psObject.GetStringValue("Name");
				Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("Get-Recipient")
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
					throw new ApplicationException("Get-Recipient returns no result");
				}
			}
			finally
			{
				if (!string.IsNullOrEmpty(text))
				{
					base.RemoveObject(powershell, "Mailbox", text);
				}
			}
		}
	}
}
