using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Threading;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rps.Probes
{
	public class AutogroupTipProbe : RpsTipProbeBase
	{
		protected override void ExecuteTipScenarioes(PowerShell powershell)
		{
			string approvedMbxName = null;
			string rejectedMbxName = null;
			string text = null;
			string text2 = null;
			try
			{
				PSObject psObject = base.CreateMailbox(powershell, "ApprovedMbx");
				approvedMbxName = psObject.GetStringValue("Name");
				PSObject psObject2 = base.CreateMailbox(powershell, "RejectedMbx");
				rejectedMbxName = psObject2.GetStringValue("Name");
				PSObject psObject3 = base.CreateMailbox(powershell, "DecisionMbx");
				text = psObject3.GetStringValue("Name");
				string decisionMbxLiveId = psObject3.GetStringValue(base.LiveIDParameterName);
				text2 = base.GetUniqueName("DistributionGroup");
				Command command = new Command("New-DistributionGroup");
				command.Parameters.Add("Name", text2);
				command.Parameters.Add("MemberJoinRestriction", "ApprovalRequired");
				command.Parameters.Add("ManagedBy", text);
				Collection<PSObject> results = base.ExecuteCmdlet(powershell, command);
				if (results.Count <= 0)
				{
					throw new ApplicationException("New-DistributionGroup returns no result");
				}
				PSCommand psCommand = new PSCommand();
				psCommand.AddCommand("Add-DistributionGroupMember");
				psCommand.AddParameter("Identity", text2);
				this.Retry(12, 20, delegate
				{
					RemotePowerShell remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCredential(new Uri(this.Definition.Endpoint), approvedMbxName + "@" + this.DomainName, this.Definition.AccountPassword, false);
					remotePowerShell.InvokePSCommand(psCommand);
					return true;
				});
				this.Retry(7, 20, delegate
				{
					RemotePowerShell remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCredential(new Uri(this.Definition.Endpoint), rejectedMbxName + "@" + this.DomainName, this.Definition.AccountPassword, false);
					remotePowerShell.InvokePSCommand(psCommand);
					return true;
				});
				bool approved = false;
				bool rejected = false;
				this.Retry(7, 20, delegate
				{
					approved = AutogroupTipProbe.ProcessApprovalRequest(this.HostName, decisionMbxLiveId, this.Definition.AccountPassword, approvedMbxName + "@" + this.DomainName, true);
					return approved;
				});
				if (!approved)
				{
					throw new ApplicationException("Cannot find or approve group join request mail in 2 minutes.");
				}
				this.Retry(7, 20, delegate
				{
					rejected = AutogroupTipProbe.ProcessApprovalRequest(this.HostName, decisionMbxLiveId, this.Definition.AccountPassword, rejectedMbxName + "@" + this.DomainName, false);
					return rejected;
				});
				if (!rejected)
				{
					throw new ApplicationException("Cannot find or reject group join request mail in 2 minutes.");
				}
				bool foundApprovedMailbox = false;
				bool foundRejectedMailbox = false;
				command = new Command("Get-DistributionGroupMember");
				command.Parameters.Add("Identity", text2);
				this.Retry(7, 20, delegate
				{
					results = this.ExecuteCmdlet(powershell, command);
					foundApprovedMailbox = results.Any((PSObject x) => string.Compare(x.Properties["Name"].Value as string, approvedMbxName, true) == 0);
					foundRejectedMailbox = results.Any((PSObject x) => string.Compare(x.Properties["Name"].Value as string, rejectedMbxName, true) == 0);
					return foundApprovedMailbox;
				});
				if (!foundApprovedMailbox)
				{
					throw new ApplicationException("Failed to join group!");
				}
				if (foundRejectedMailbox)
				{
					throw new ApplicationException("Failed to reject group joining request!");
				}
			}
			finally
			{
				base.RemoveObject(powershell, "Mailbox", approvedMbxName);
				base.RemoveObject(powershell, "Mailbox", rejectedMbxName);
				base.RemoveObject(powershell, "DistributionGroup", text2);
				base.RemoveObject(powershell, "Mailbox", text);
			}
		}

		internal static bool ProcessApprovalRequest(string hostName, string account, string password, string requestor, bool approve)
		{
			ExchangeService exchangeService = new ExchangeService(1);
			exchangeService.Url = new Uri(string.Format("https://{0}/ews/exchange.asmx", hostName));
			exchangeService.Credentials = new NetworkCredential(account, password);
			FolderId folderId = new FolderId(4, new Mailbox
			{
				Address = account,
				RoutingType = "SMTP"
			});
			Folder folder = Folder.Bind(exchangeService, folderId);
			SearchFilter searchFilter = new SearchFilter.IsEqualTo(ItemSchema.ItemClass, "IPM.Note.Microsoft.Approval.Request");
			ExtendedPropertyDefinition extendedPropertyDefinition = new ExtendedPropertyDefinition(5, "x-ms-exchange-organization-approval-requestor", 25);
			ExtendedPropertyDefinition extendedPropertyDefinition2 = new ExtendedPropertyDefinition(2, 34084, 25);
			ExtendedPropertyDefinition extendedPropertyDefinition3 = new ExtendedPropertyDefinition(3613, 25);
			ExtendedPropertyDefinition extendedPropertyDefinition4 = new ExtendedPropertyDefinition(49, 2);
			PropertySet propertySet = new PropertySet(1);
			propertySet.Add(extendedPropertyDefinition);
			propertySet.Add(extendedPropertyDefinition3);
			propertySet.Add(extendedPropertyDefinition4);
			FindItemsResults<Item> findItemsResults = folder.FindItems(searchFilter, new ItemView(1000)
			{
				PropertySet = propertySet
			});
			foreach (Item item in findItemsResults)
			{
				EmailMessage emailMessage = (EmailMessage)item;
				string text;
				emailMessage.TryGetProperty<string>(extendedPropertyDefinition, ref text);
				string str;
				emailMessage.TryGetProperty<string>(extendedPropertyDefinition3, ref str);
				byte[] array;
				emailMessage.TryGetProperty<byte[]>(extendedPropertyDefinition4, ref array);
				emailMessage.Load();
				if (text.Equals(requestor, StringComparison.OrdinalIgnoreCase))
				{
					EmailMessage emailMessage2 = new EmailMessage(exchangeService);
					emailMessage2.ToRecipients.Add(emailMessage.Sender.Address);
					emailMessage2.SetExtendedProperty(extendedPropertyDefinition4, array);
					if (approve)
					{
						emailMessage2.Subject = "Approve: " + str;
						emailMessage2.ItemClass = "IPM.Note.Microsoft.Approval.Reply.Approve";
						emailMessage2.SetExtendedProperty(extendedPropertyDefinition2, "Approve");
					}
					else
					{
						emailMessage2.Subject = "Reject: " + str;
						emailMessage2.ItemClass = "IPM.Note.Microsoft.Approval.Reply.Reject";
						emailMessage2.SetExtendedProperty(extendedPropertyDefinition2, "Reject");
					}
					emailMessage2.SendAndSaveCopy();
					emailMessage.Delete(2);
					return true;
				}
			}
			return false;
		}

		private void Retry(int retryCount, int waitSeconds, Func<bool> operationDelegate)
		{
			for (int i = 0; i < retryCount; i++)
			{
				try
				{
					bool flag = operationDelegate();
					if (flag)
					{
						break;
					}
				}
				catch (Exception ex)
				{
					if (i == retryCount - 1)
					{
						throw ex;
					}
				}
				Thread.Sleep(TimeSpan.FromSeconds((double)waitSeconds));
			}
		}
	}
}
