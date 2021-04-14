using System;
using System.Collections;
using System.Globalization;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Assistants;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("Update", "PublicFolderMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "InvokeMailboxAssistant")]
	public sealed class UpdatePublicFolderMailbox : RecipientObjectActionTask<MailboxIdParameter, ADRecipient>
	{
		[Parameter(Mandatory = true, ParameterSetName = "InvokeMailboxAssistant", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(Mandatory = true, ParameterSetName = "InvokeSynchronizer", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "InvokeSynchronizer")]
		public SwitchParameter InvokeSynchronizer
		{
			get
			{
				return (SwitchParameter)(base.Fields["InvokeSynchronizer"] ?? false);
			}
			set
			{
				base.Fields["InvokeSynchronizer"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "InvokeSynchronizer")]
		public SwitchParameter FullSync
		{
			get
			{
				return (SwitchParameter)(base.Fields["InvokeFullSync"] ?? false);
			}
			set
			{
				base.Fields["InvokeFullSync"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "InvokeSynchronizer")]
		public SwitchParameter SuppressStatus
		{
			get
			{
				return (SwitchParameter)(base.Fields["SuppressStatus"] ?? false);
			}
			set
			{
				base.Fields["SuppressStatus"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "InvokeSynchronizer")]
		public SwitchParameter ReconcileFolders
		{
			get
			{
				return (SwitchParameter)(base.Fields["ReconcileFolders"] ?? false);
			}
			set
			{
				base.Fields["ReconcileFolders"] = value;
			}
		}

		public UpdatePublicFolderMailbox()
		{
			if (ExEnvironment.IsTest)
			{
				UpdatePublicFolderMailbox.timeToWaitInMilliseconds = 600000;
				return;
			}
			UpdatePublicFolderMailbox.timeToWaitInMilliseconds = 60000;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUpdatePublicFolderMailbox(this.Identity.ToString());
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is PublicFolderSyncTransientException || exception is PublicFolderSyncPermanentException || base.IsKnownException(exception);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if (this.FullSync && !this.InvokeSynchronizer)
			{
				base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorInvalidMandatoryParameterForPublicFolderTasks("InvokeFullSync", "InvokeSynchronizer")), ExchangeErrorCategory.Client, null);
			}
			else if (this.SuppressStatus && !this.InvokeSynchronizer)
			{
				base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorInvalidMandatoryParameterForPublicFolderTasks("SuppressStatus", "InvokeSynchronizer")), ExchangeErrorCategory.Client, null);
			}
			else if (this.ReconcileFolders && !this.InvokeSynchronizer)
			{
				base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorInvalidMandatoryParameterForPublicFolderTasks("ReconcileFolders", "InvokeSynchronizer")), ExchangeErrorCategory.Client, null);
			}
			base.InternalBeginProcessing();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				ADUser aduser = this.DataObject as ADUser;
				if (aduser == null || aduser.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
				{
					base.WriteError(new ObjectNotFoundException(Strings.PublicFolderMailboxNotFound), ExchangeErrorCategory.Client, aduser);
				}
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(base.SessionSettings, aduser);
				string serverFqdn = exchangePrincipal.MailboxInfo.Location.ServerFqdn;
				if (this.InvokeSynchronizer)
				{
					TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(aduser.OrganizationId);
					Organization orgContainer = this.ConfigurationSession.GetOrgContainer();
					if (orgContainer.DefaultPublicFolderMailbox.HierarchyMailboxGuid != value.GetHierarchyMailboxInformation().HierarchyMailboxGuid || value.GetLocalMailboxRecipient(aduser.ExchangeGuid) == null)
					{
						TenantPublicFolderConfigurationCache.Instance.RemoveValue(aduser.OrganizationId);
					}
					if (aduser.ExchangeGuid == value.GetHierarchyMailboxInformation().HierarchyMailboxGuid)
					{
						base.WriteError(new TaskArgumentException(Strings.ErrorSecondaryMailboxIdRequired), ExchangeErrorCategory.Client, exchangePrincipal);
					}
					if (this.FullSync)
					{
						using (PublicFolderSession publicFolderSession = PublicFolderSession.OpenAsAdmin(this.DataObject.OrganizationId, null, aduser.ExchangeGuid, null, CultureInfo.CurrentCulture, "Client=Management;Action=UpdatePublicFolderMailbox", null))
						{
							using (Folder folder = Folder.Bind(publicFolderSession, publicFolderSession.GetTombstonesRootFolderId()))
							{
								using (UserConfiguration configuration = UserConfiguration.GetConfiguration(folder, new UserConfigurationName("PublicFolderSyncInfo", ConfigurationNameKind.Name), UserConfigurationTypes.Dictionary))
								{
									IDictionary dictionary = configuration.GetDictionary();
									dictionary["SyncState"] = null;
									configuration.Save();
								}
							}
						}
					}
					PublicFolderSyncJobState publicFolderSyncJobState = PublicFolderSyncJobRpc.StartSyncHierarchy(exchangePrincipal, this.ReconcileFolders);
					if (!this.SuppressStatus)
					{
						base.WriteObject(new UpdatePublicFolderMailboxResult(Strings.StatusMessageStartUpdatePublicFolderMailbox(this.Identity.ToString())));
						int num = 0;
						while (publicFolderSyncJobState.JobStatus != PublicFolderSyncJobState.Status.Completed && publicFolderSyncJobState.LastError == null && num++ < UpdatePublicFolderMailbox.timeToWaitInMilliseconds / UpdatePublicFolderMailbox.QueryIntervalInMilliseconds)
						{
							base.WriteObject(new UpdatePublicFolderMailboxResult(Strings.StatusMessageUpdatePublicFolderMailboxUnderProgress(publicFolderSyncJobState.JobStatus.ToString())));
							Thread.Sleep(UpdatePublicFolderMailbox.QueryIntervalInMilliseconds);
							publicFolderSyncJobState = PublicFolderSyncJobRpc.QueryStatusSyncHierarchy(exchangePrincipal);
						}
						if (publicFolderSyncJobState.LastError != null)
						{
							base.WriteError(publicFolderSyncJobState.LastError, ExchangeErrorCategory.ServerOperation, publicFolderSyncJobState);
						}
						if (publicFolderSyncJobState.JobStatus == PublicFolderSyncJobState.Status.Completed)
						{
							base.WriteObject(new UpdatePublicFolderMailboxResult(Strings.StatusMessageUpdatePublicFolderMailboxCompleted));
						}
						else
						{
							base.WriteObject(new UpdatePublicFolderMailboxResult(Strings.StatusMessageSynchronizerRunningInBackground));
						}
					}
				}
				else if (aduser.ExchangeGuid != Guid.Empty)
				{
					MailboxDatabase mailboxDatabase = base.GlobalConfigSession.Read<MailboxDatabase>(aduser.Database);
					if (mailboxDatabase == null)
					{
						base.WriteError(new TaskArgumentException(Strings.ElcMdbNotFound(this.Identity.ToString())), ExchangeErrorCategory.Client, null);
					}
					this.EnsureMailboxExistsOnDatabase(aduser.ExchangeGuid);
					AssistantsRpcClient assistantsRpcClient = new AssistantsRpcClient(serverFqdn);
					try
					{
						assistantsRpcClient.Start("PublicFolderAssistant", aduser.ExchangeGuid, mailboxDatabase.Guid);
					}
					catch (RpcException ex)
					{
						base.WriteError(new TaskException(RpcUtility.MapRpcErrorCodeToMessage(ex.ErrorCode, serverFqdn)), ExchangeErrorCategory.Client, null);
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void EnsureMailboxExistsOnDatabase(Guid mailboxGuid)
		{
			TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(this.DataObject.OrganizationId);
			if (value.GetLocalMailboxRecipient(mailboxGuid) == null)
			{
				TenantPublicFolderConfigurationCache.Instance.RemoveValue(this.DataObject.OrganizationId);
			}
			using (PublicFolderSession.OpenAsAdmin(this.DataObject.OrganizationId, null, mailboxGuid, null, CultureInfo.CurrentCulture, "Client=Management;Action=UpdatePublicFolderMailbox", null))
			{
			}
		}

		private const string SyncState = "SyncState";

		private const string ParameterSetSynchronizer = "InvokeSynchronizer";

		private const string InvokeSynchronizerField = "InvokeSynchronizer";

		private const string ParameterSetAssistant = "InvokeMailboxAssistant";

		private const string InvokeFullSyncField = "InvokeFullSync";

		private const string SuppressStatusField = "SuppressStatus";

		private const string ReconcileFoldersField = "ReconcileFolders";

		private static int timeToWaitInMilliseconds;

		private static int QueryIntervalInMilliseconds = 3000;
	}
}
