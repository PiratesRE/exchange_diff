using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Update", "SiteMailbox", SupportsShouldProcess = true)]
	public sealed class UpdateSiteMailbox : TeamMailboxDiagnosticsBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUpdateTeamMailbox(base.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		[ValidateNotNullOrEmpty]
		public string Server
		{
			get
			{
				return (string)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		public SwitchParameter FullSync
		{
			get
			{
				return (SwitchParameter)(base.Fields["FullSync"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["FullSync"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TeamMailboxDiagnosticsBase.TargetType? Target
		{
			get
			{
				return (TeamMailboxDiagnosticsBase.TargetType?)base.Fields["Target"];
			}
			set
			{
				base.Fields["Target"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			foreach (KeyValuePair<ADUser, ExchangePrincipal> keyValuePair in base.TMPrincipals)
			{
				ADUser key = keyValuePair.Key;
				ExchangePrincipal value = keyValuePair.Value;
				this.Target = new TeamMailboxDiagnosticsBase.TargetType?(this.Target ?? TeamMailboxDiagnosticsBase.TargetType.All);
				if (this.Target == TeamMailboxDiagnosticsBase.TargetType.Document || this.Target == TeamMailboxDiagnosticsBase.TargetType.All)
				{
					EnqueueResult enqueueResult = RpcClientWrapper.EnqueueTeamMailboxSyncRequest((!string.IsNullOrEmpty(this.Server)) ? this.Server : value.MailboxInfo.Location.ServerFqdn, value.MailboxInfo.MailboxGuid, QueueType.TeamMailboxDocumentSync, key.OrganizationId, "UpdateTMCMD_" + base.ExecutingUserIdentityName, base.DomainController, this.FullSync ? SyncOption.FullSync : SyncOption.Default);
					enqueueResult.Type = QueueType.TeamMailboxDocumentSync;
					base.WriteObject(enqueueResult);
				}
				if (this.Target == TeamMailboxDiagnosticsBase.TargetType.Membership || this.Target == TeamMailboxDiagnosticsBase.TargetType.All)
				{
					EnqueueResult enqueueResult2 = RpcClientWrapper.EnqueueTeamMailboxSyncRequest((!string.IsNullOrEmpty(this.Server)) ? this.Server : value.MailboxInfo.Location.ServerFqdn, value.MailboxInfo.MailboxGuid, QueueType.TeamMailboxMembershipSync, key.OrganizationId, "UpdateTMCMD_" + base.ExecutingUserIdentityName, base.DomainController, this.FullSync ? SyncOption.FullSync : SyncOption.Default);
					enqueueResult2.Type = QueueType.TeamMailboxMembershipSync;
					base.WriteObject(enqueueResult2);
				}
				if (this.Target == TeamMailboxDiagnosticsBase.TargetType.Maintenance || this.Target == TeamMailboxDiagnosticsBase.TargetType.All)
				{
					EnqueueResult enqueueResult3 = RpcClientWrapper.EnqueueTeamMailboxSyncRequest((!string.IsNullOrEmpty(this.Server)) ? this.Server : value.MailboxInfo.Location.ServerFqdn, value.MailboxInfo.MailboxGuid, QueueType.TeamMailboxMaintenanceSync, key.OrganizationId, "UpdateTMCMD_" + base.ExecutingUserIdentityName, base.DomainController, this.FullSync ? SyncOption.FullSync : SyncOption.Default);
					enqueueResult3.Type = QueueType.TeamMailboxMaintenanceSync;
					base.WriteObject(enqueueResult3);
				}
			}
		}
	}
}
