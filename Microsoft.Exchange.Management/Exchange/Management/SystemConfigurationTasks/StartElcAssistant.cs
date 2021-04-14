using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common.ELC;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Assistants;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Start", "ManagedFolderAssistant", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public class StartElcAssistant : RecipientObjectActionTask<MailboxOrMailUserIdParameter, ADUser>
	{
		public StartElcAssistant()
		{
			this.InternalStateResetAction = new Action(base.InternalStateReset);
			this.InactiveMailboxEnabled = VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.InactiveMailbox.Enabled;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageStartELCAssistant;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true)]
		public override MailboxOrMailUserIdParameter Identity
		{
			get
			{
				return (MailboxOrMailUserIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter]
		public SwitchParameter HoldCleanup
		{
			get
			{
				return (SwitchParameter)(base.Fields["HoldCleanup"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["HoldCleanup"] = value;
			}
		}

		[Parameter]
		public SwitchParameter EHAHiddenFolderCleanup
		{
			get
			{
				return (SwitchParameter)(base.Fields["EHAHiddenFolderCleanup"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["EHAHiddenFolderCleanup"] = value;
			}
		}

		[Parameter]
		public SwitchParameter InactiveMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["InactiveMailbox"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["InactiveMailbox"] = value;
			}
		}

		internal bool InactiveMailboxEnabled { get; set; }

		internal Action InternalStateResetAction { get; set; }

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			if (this.InactiveMailboxEnabled && this.InactiveMailbox)
			{
				using (new ADSessionSettingsFactory.InactiveMailboxVisibilityEnabler())
				{
					this.InternalStateResetAction();
				}
				base.OptionalIdentityData.AdditionalFilter = StartElcAssistant.InactiveMailboxFilter;
			}
			else
			{
				this.InternalStateResetAction();
			}
			TaskLogger.LogExit();
		}

		internal override IConfigurationSession CreateConfigurationSession(ADSessionSettings sessionSettings)
		{
			if (sessionSettings.ConfigScopes == ConfigScopes.TenantLocal)
			{
				return base.CreateConfigurationSession(ADSessionSettings.RescopeToSubtree(sessionSettings));
			}
			return base.CreateConfigurationSession(sessionSettings);
		}

		internal override IRecipientSession CreateTenantGlobalCatalogSession(ADSessionSettings sessionSettings)
		{
			if (sessionSettings.ConfigScopes == ConfigScopes.TenantLocal)
			{
				return base.CreateTenantGlobalCatalogSession(ADSessionSettings.RescopeToSubtree(sessionSettings));
			}
			return base.CreateTenantGlobalCatalogSession(sessionSettings);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			string mailboxAddress = this.Identity.ToString();
			ADUser dataObject = this.DataObject;
			ELCTaskHelper.VerifyIsInScopes(dataObject, base.ScopeSet, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (dataObject.ExchangeGuid != Guid.Empty && dataObject.RecipientType != RecipientType.MailUser && dataObject.Database != null)
			{
				MailboxDatabase mailboxDatabase = base.GlobalConfigSession.Read<MailboxDatabase>(dataObject.Database);
				if (mailboxDatabase == null)
				{
					base.WriteError(new ArgumentException(Strings.ElcMdbNotFound(mailboxAddress), "Mailbox"), ErrorCategory.InvalidArgument, null);
				}
				this.InternalProcessOneMailbox(dataObject.ExchangeGuid, mailboxDatabase.Guid);
			}
			if (dataObject.ArchiveState == ArchiveState.Local)
			{
				ADObjectId entryId = dataObject.ArchiveDatabase ?? dataObject.Database;
				MailboxDatabase mailboxDatabase2 = base.GlobalConfigSession.Read<MailboxDatabase>(entryId);
				if (mailboxDatabase2 == null)
				{
					base.WriteError(new ArgumentException(Strings.ElcMdbNotFound(mailboxAddress), "Archive Mailbox"), ErrorCategory.InvalidArgument, null);
				}
				if (StoreRetentionPolicyTagHelper.HasOnPremArchiveMailbox(dataObject))
				{
					try
					{
						StoreRetentionPolicyTagHelper.SyncOptionalTagsFromPrimaryToArchive(dataObject);
					}
					catch (ElcUserConfigurationException)
					{
						this.WriteWarning(Strings.WarningArchiveMailboxNotReachable(mailboxAddress));
					}
				}
				this.InternalProcessOneMailbox(dataObject.ArchiveGuid, mailboxDatabase2.Guid);
			}
			if (!this.processed)
			{
				this.WriteWarning(Strings.ElcNoLocalMbxOrArchive(mailboxAddress));
			}
			TaskLogger.LogExit();
		}

		private void InternalProcessOneMailbox(Guid mailboxGuid, Guid mdbGuid)
		{
			ExchangePrincipal exchangePrincipal = null;
			try
			{
				ADSessionSettings orgWideSessionSettings = base.OrgWideSessionSettings;
				if (VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.InactiveMailbox.Enabled)
				{
					orgWideSessionSettings.IncludeInactiveMailbox = true;
				}
				exchangePrincipal = ExchangePrincipal.FromMailboxGuid(orgWideSessionSettings, mailboxGuid, null);
			}
			catch (ObjectNotFoundException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, this.Identity);
			}
			string serverFqdn = exchangePrincipal.MailboxInfo.Location.ServerFqdn;
			AssistantsRpcClient client = new AssistantsRpcClient(serverFqdn);
			this.InternalProcessOneRequest(client, serverFqdn, mailboxGuid, mdbGuid);
			this.processed = true;
		}

		private void InternalProcessOneRequest(AssistantsRpcClient client, string serverName, Guid mailboxGuid, Guid mdbGuid)
		{
			int num = 3;
			try
			{
				IL_02:
				client.StartWithParams("ElcAssistant", mailboxGuid, mdbGuid, this.GetElcParameters().ToString());
			}
			catch (RpcException ex)
			{
				num--;
				if ((ex.ErrorCode == 1753 || ex.ErrorCode == 1727) && num > 0)
				{
					goto IL_02;
				}
				base.WriteError(new TaskException(RpcUtility.MapRpcErrorCodeToMessage(ex.ErrorCode, serverName)), ErrorCategory.InvalidOperation, null);
				goto IL_02;
			}
		}

		private ElcParameters GetElcParameters()
		{
			ElcParameters elcParameters = ElcParameters.None;
			if (this.HoldCleanup)
			{
				elcParameters |= ElcParameters.HoldCleanup;
			}
			if (this.EHAHiddenFolderCleanup)
			{
				elcParameters |= ElcParameters.EHAHiddenFolderCleanup;
			}
			return elcParameters;
		}

		private static readonly QueryFilter InactiveMailboxFilter = new BitMaskAndFilter(ADRecipientSchema.RecipientSoftDeletedStatus, 8UL);

		private bool processed;
	}
}
