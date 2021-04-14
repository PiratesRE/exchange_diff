using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal static class AdminLogCmdletSkipList
	{
		internal static int Count
		{
			get
			{
				return AdminLogCmdletSkipList.GetSkippedCmdletList().Count;
			}
		}

		private static HashSet<string> GetSkippedCmdletList()
		{
			if (AdminLogCmdletSkipList.cmdletsToBeSkipped == null)
			{
				HashSet<string> defaultValue = AdminLogCmdletSkipList.PopulateBlockedCmdletList();
				Interlocked.CompareExchange<Hookable<HashSet<string>>>(ref AdminLogCmdletSkipList.cmdletsToBeSkipped, Hookable<HashSet<string>>.Create(true, defaultValue), null);
			}
			return AdminLogCmdletSkipList.cmdletsToBeSkipped.Value;
		}

		internal static bool ShouldSkipCmdlet(string cmdlet)
		{
			return AdminLogCmdletSkipList.GetSkippedCmdletList().Contains(cmdlet);
		}

		internal static IDisposable SetCmdletBlockListTestHook(HashSet<string> cmdletList)
		{
			return AdminLogCmdletSkipList.cmdletsToBeSkipped.SetTestHook(cmdletList);
		}

		private static HashSet<string> PopulateBlockedCmdletList()
		{
			return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"New-MoveRequest",
				"Set-CentralAdminDropBoxMachineEntry",
				"New-CentralAdminOperation",
				"Remove-CentralAdminParameter",
				"Remove-CentralAdminParameterDefinition",
				"New-CentralAdminParameterDefinition",
				"Add-CentralAdminParameter",
				"Set-CentralAdminMachine",
				"New-ServiceAlert",
				"Set-CentralAdminDropBoxPodEntry",
				"Set-ServiceAlert",
				"Set-CentralAdminDropBoxFailedMachineEntry",
				"Remove-MoveRequest",
				"Set-CentralAdminVlan",
				"Set-CentralAdminRouter",
				"New-CentralAdminAlertMapping",
				"New-CentralAdminLoadBalancerVirtualServer",
				"Remove-CentralAdminLock",
				"Set-CentralAdminPod",
				"New-CentralAdminPod",
				"New-CentralAdminIPDefinition",
				"Set-CentralAdminIPDefinition",
				"New-CentralAdminRouter",
				"Resume-MoveRequest",
				"New-CentralAdminLock",
				"New-ServiceChangeRequest",
				"New-CentralAdminRack",
				"Set-Organization",
				"New-GroupMailbox",
				"Set-GroupMailbox",
				"Set-MailboxPlan",
				"Set-CASMailboxPlan",
				"Set-OrganizationFlags",
				"Set-MServSyncConfigFlags",
				"New-ApprovalApplication",
				"Install-GlobalAddressLists",
				"install-TransportConfigContainer",
				"install-PerimeterConfigContainer",
				"Install-EmailAddressPolicy",
				"Install-GlobalSettingsContainer",
				"Install-InternetMessageFormat",
				"New-MicrosoftExchangeRecipient",
				"Install-FederationContainer",
				"Install-ActiveSyncDeviceClassContainer",
				"New-ApprovalApplicationContainer",
				"Set-PerimeterConfig",
				"Set-ManagementSiteLink",
				"Install-CannedAddressLists",
				"Install-DlpPolicyCollection",
				"Remove-SyncUser",
				"Remove-ArbitrationMailbox",
				"Set-TenantRelocationRequest",
				"New-TenantRelocationRequest",
				"install-Container",
				"Install-RuleCollection",
				"Set-CalendarNotification",
				"Set-CalendarProcessing",
				"Set-MailboxCalendarConfiguration",
				"Set-MailboxAutoReplyConfiguration",
				"Set-MailboxJunkEmailConfiguration",
				"Set-MailboxMessageConfiguration",
				"New-HotmailSubscription",
				"New-ImapSubscription",
				"New-PopSubscription",
				"New-Subscription",
				"Remove-Subscription",
				"Set-HotmailSubscription",
				"Set-ImapSubscription",
				"Set-PopSubscription",
				"Import-ContactList",
				"New-ConnectSubscription",
				"Remove-ConnectSubscription",
				"Set-ConnectSubscription",
				"Remove-UserPhoto",
				"Set-UserPhoto",
				"Set-MailboxRegionalConfiguration",
				"Disable-UMCallAnsweringRule",
				"Enable-UMCallAnsweringRule",
				"New-UMCallAnsweringRule",
				"Remove-UMCallAnsweringRule",
				"Set-UMCallAnsweringRule",
				"Set-UMMailbox",
				"Set-UMMailboxConfiguration",
				"Start-UMPhoneSession",
				"Stop-UMPhoneSession"
			};
		}

		private static Hookable<HashSet<string>> cmdletsToBeSkipped;
	}
}
