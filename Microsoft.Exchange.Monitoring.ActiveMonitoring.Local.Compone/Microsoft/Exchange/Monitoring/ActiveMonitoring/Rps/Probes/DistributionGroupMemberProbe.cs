using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class DistributionGroupMemberProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string text = null;
			string text2 = null;
			try
			{
				PSObject psObject = base.CreateMailbox(powershell, "mailbox");
				text = psObject.GetStringValue("Name");
				PSObject psObject2 = base.NewDistributionGroup(powershell, "DG");
				text2 = psObject2.GetStringValue("Name");
				string accountUserName = base.AccountUserName;
				Command command = new Command("Add-DistributionGroupMember");
				command.Parameters.Add("Identity", text2);
				command.Parameters.Add("Member", accountUserName);
				base.ExecuteCmdlet(powershell, command);
				this.GetDistributionGroupMember(powershell, command, text2, accountUserName);
				command = new Command("Update-DistributionGroupMember");
				command.Parameters.Add("Identity", text2);
				command.Parameters.Add("Members", text);
				base.ExecuteCmdlet(powershell, command);
				this.GetDistributionGroupMember(powershell, command, text2, text);
				base.ExecuteCmdlet(powershell, new Command("Remove-DistributionGroupMember")
				{
					Parameters = 
					{
						{
							"Identity",
							text2
						},
						{
							"Member",
							text
						}
					}
				});
			}
			finally
			{
				base.RemoveObject(powershell, "Mailbox", text);
				base.RemoveObject(powershell, "DistributionGroup", text2);
			}
		}

		private void GetDistributionGroupMember(PowerShell powershell, Command command, string distributionGroupName, string memberName)
		{
			Collection<PSObject> collection = base.ExecuteCmdlet(powershell, new Command("Get-DistributionGroupMember")
			{
				Parameters = 
				{
					{
						"Identity",
						distributionGroupName
					}
				}
			});
			if (collection.Count <= 0)
			{
				throw new ApplicationException("Get-DistributionGroupMember didn't return any result");
			}
			string text = (string)collection[0].Properties["Identity"].Value;
			if (!Regex.IsMatch(text, string.Format("{0}*", memberName)))
			{
				throw new ApplicationException(string.Format("Expected member name is '{0}' but actual is '{1}'", memberName, text));
			}
		}
	}
}
