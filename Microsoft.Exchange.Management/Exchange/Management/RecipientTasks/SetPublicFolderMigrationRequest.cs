using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "PublicFolderMigrationRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetPublicFolderMigrationRequest : SetRequest<PublicFolderMigrationRequestIdParameter>
	{
		[Parameter(Mandatory = false)]
		public bool PreventCompletion
		{
			get
			{
				return (bool)(base.Fields["PreventCompletion"] ?? true);
			}
			set
			{
				base.Fields["PreventCompletion"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string RemoteMailboxLegacyDN
		{
			get
			{
				return (string)base.Fields["RemoteMailboxLegacyDN"];
			}
			set
			{
				base.Fields["RemoteMailboxLegacyDN"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string RemoteMailboxServerLegacyDN
		{
			get
			{
				return (string)base.Fields["RemoteMailboxServerLegacyDN"];
			}
			set
			{
				base.Fields["RemoteMailboxServerLegacyDN"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public Fqdn OutlookAnywhereHostName
		{
			get
			{
				return (Fqdn)base.Fields["OutlookAnywhereHostName"];
			}
			set
			{
				base.Fields["OutlookAnywhereHostName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AuthenticationMethod AuthenticationMethod
		{
			get
			{
				return (AuthenticationMethod)(base.Fields["AuthenticationMethod"] ?? AuthenticationMethod.Basic);
			}
			set
			{
				base.Fields["AuthenticationMethod"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "MigrationOutlookAnywherePublicFolder")]
		public new PSCredential RemoteCredential
		{
			get
			{
				return base.RemoteCredential;
			}
			set
			{
				base.RemoteCredential = value;
			}
		}

		protected override void ModifyRequest(TransactionalRequestJob requestJob)
		{
			base.ModifyRequest(requestJob);
			if (base.IsFieldSet("RemoteMailboxLegacyDN"))
			{
				requestJob.RemoteMailboxLegacyDN = this.RemoteMailboxLegacyDN;
			}
			if (base.IsFieldSet("RemoteMailboxServerLegacyDN"))
			{
				requestJob.RemoteMailboxServerLegacyDN = this.RemoteMailboxServerLegacyDN;
			}
			if (base.IsFieldSet("OutlookAnywhereHostName"))
			{
				requestJob.OutlookAnywhereHostName = this.OutlookAnywhereHostName;
			}
			if (base.IsFieldSet("AuthenticationMethod"))
			{
				requestJob.AuthenticationMethod = new AuthenticationMethod?(this.AuthenticationMethod);
			}
			if (base.IsFieldSet("RemoteCredential"))
			{
				requestJob.RemoteCredential = RequestTaskHelper.GetNetworkCredential(this.RemoteCredential, requestJob.AuthenticationMethod);
			}
			if (base.IsFieldSet("PreventCompletion"))
			{
				if (!this.PreventCompletion)
				{
					if (requestJob.Status != RequestStatus.AutoSuspended)
					{
						base.WriteError(new PreventCompletionCannotSetException(), ExchangeErrorCategory.Client, this.Identity);
					}
					requestJob.Priority = RequestPriority.High;
				}
				else if (!requestJob.PreventCompletion)
				{
					base.WriteError(new InvalidValueForPreventCompletionException(), ExchangeErrorCategory.Client, this.Identity);
				}
				requestJob.PreventCompletion = this.PreventCompletion;
				requestJob.SuspendWhenReadyToComplete = this.PreventCompletion;
				requestJob.AllowedToFinishMove = !this.PreventCompletion;
			}
		}

		protected override void CheckIndexEntry()
		{
		}

		private const string ParameterPreventCompletion = "PreventCompletion";
	}
}
