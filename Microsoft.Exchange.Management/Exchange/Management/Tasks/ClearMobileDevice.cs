using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.AirSync;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Clear", "MobileDevice", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class ClearMobileDevice : SystemConfigurationObjectActionTask<MobileDeviceIdParameter, MobileDevice>
	{
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> NotificationEmailAddresses
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["NotificationEmailAddresses"];
			}
			set
			{
				base.Fields["NotificationEmailAddresses"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Cancel
		{
			get
			{
				return (SwitchParameter)(base.Fields["Cancel"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Cancel"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (!this.Cancel)
				{
					return Strings.ConfirmationMessageClearMobileDevice(this.Identity.ToString());
				}
				return base.ConfirmationMessage;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession configurationSession;
			if (!MobileDeviceTaskHelper.IsRunningUnderMyOptionsRole(this, base.TenantGlobalCatalogSession, base.SessionSettings))
			{
				configurationSession = (IConfigurationSession)base.CreateSession();
			}
			else
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 116, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\AirSync\\ClearMobileDevice.cs");
			}
			configurationSession.UseConfigNC = false;
			configurationSession.UseGlobalCatalog = (base.DomainController == null && base.ServerSettings.ViewEntireForest);
			return configurationSession;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (!base.HasErrors)
				{
					if (MobileDeviceTaskHelper.IsRunningUnderMyOptionsRole(this, base.TenantGlobalCatalogSession, base.SessionSettings))
					{
						ADObjectId id;
						if (!base.TryGetExecutingUserId(out id))
						{
							throw new ExecutingUserPropertyNotFoundException("executingUserid");
						}
						if (!this.DataObject.Id.Parent.Parent.Equals(id))
						{
							base.WriteError(new LocalizedException(Strings.ErrorObjectNotFound(this.Identity.ToString())), ErrorCategory.InvalidArgument, null);
						}
					}
					IRecipientSession recipientSession = this.CreateTenantGlobalCatalogSession(base.SessionSettings);
					Exception ex = null;
					MailboxIdParameter mailboxId = this.Identity.GetMailboxId();
					if (mailboxId == null && this.DataObject != null)
					{
						this.Identity = new MobileDeviceIdParameter(this.DataObject);
						mailboxId = this.Identity.GetMailboxId();
					}
					if (mailboxId == null)
					{
						base.WriteError(new LocalizedException(Strings.ErrorObjectNotFound(this.Identity.ToString())), ErrorCategory.InvalidArgument, null);
					}
					ADUser adObject = null;
					this.principal = MobileDeviceTaskHelper.GetExchangePrincipal(base.SessionSettings, recipientSession, mailboxId, "Clear-MobileDevice", out ex, out adObject);
					if (ex != null)
					{
						base.WriteError(ex, ErrorCategory.InvalidArgument, null);
					}
					IList<LocalizedString> list = null;
					ADObjectId adobjectId = null;
					base.TryGetExecutingUserId(out adobjectId);
					this.validatedAddresses = MobileDeviceTaskHelper.ValidateAddresses(recipientSession, adobjectId, this.NotificationEmailAddresses, out list);
					if (list != null)
					{
						foreach (LocalizedString text in list)
						{
							this.WriteWarning(text);
						}
					}
					base.VerifyIsWithinScopes((IDirectorySession)base.DataSession, adObject, true, new DataAccessTask<MobileDevice>.ADObjectOutOfScopeString(Strings.ErrorCannotChangeMailboxOutOfWriteScope));
					if (adobjectId != null)
					{
						ExchangePrincipal exchangePrincipal = MobileDeviceTaskHelper.GetExchangePrincipal(base.SessionSettings, recipientSession, new MailboxIdParameter(adobjectId), "Clear-MobileDevice", out ex);
						if (ex != null)
						{
							base.WriteWarning(ex.ToString());
						}
						if (exchangePrincipal != null)
						{
							this.wipeRequestorSMTP = exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
						}
						else
						{
							this.wipeRequestorSMTP = null;
						}
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			MailboxSession mailboxSession = null;
			try
			{
				mailboxSession = MailboxSession.OpenAsAdmin(this.principal, CultureInfo.InvariantCulture, "Client=Management;Action=Get-ActiveSyncDeviceStatistics");
				DeviceInfo deviceInfo = MobileDeviceTaskHelper.GetDeviceInfo(mailboxSession, this.Identity);
				if (deviceInfo == null)
				{
					base.WriteError(new MobileDeviceNotExistException(this.Identity.ToString()), ErrorCategory.InvalidOperation, this.Identity);
				}
				using (SyncStateStorage syncStateStorage = SyncStateStorage.Bind(mailboxSession, deviceInfo.DeviceIdentity, null))
				{
					if (syncStateStorage == null)
					{
						base.WriteError(new MobileDeviceNotExistException(this.Identity.ToString()), ErrorCategory.InvalidOperation, this.Identity);
					}
					using (CustomSyncState orCreateGlobalSyncState = AirSyncUtility.GetOrCreateGlobalSyncState(syncStateStorage))
					{
						if (this.Cancel)
						{
							if (!DeviceInfo.CancelRemoteWipeFromMailbox(orCreateGlobalSyncState))
							{
								this.WriteWarning(Strings.CannotCancelWipe(this.Identity.ToString()));
							}
						}
						else
						{
							DeviceInfo.StartRemoteWipeFromMailbox(syncStateStorage, orCreateGlobalSyncState, ExDateTime.UtcNow, this.validatedAddresses, this.wipeRequestorSMTP);
						}
						orCreateGlobalSyncState.Commit();
					}
				}
				base.InternalProcessRecord();
			}
			catch (FolderSaveException exception)
			{
				base.WriteError(exception, ErrorCategory.WriteError, this.Identity);
			}
			catch (CorruptSyncStateException ex)
			{
				TaskLogger.LogError(ex);
				base.WriteError(ex, ErrorCategory.ReadError, this.principal);
			}
			catch (InvalidSyncStateVersionException ex2)
			{
				TaskLogger.LogError(ex2);
				base.WriteError(ex2, ErrorCategory.ReadError, this.principal);
			}
			catch (StorageTransientException ex3)
			{
				TaskLogger.LogError(ex3);
				base.WriteError(ex3, ErrorCategory.ReadError, this.principal);
			}
			catch (StoragePermanentException ex4)
			{
				TaskLogger.LogError(ex4);
				base.WriteError(ex4, ErrorCategory.InvalidOperation, this.principal);
			}
			catch (InvalidOperationException ex5)
			{
				TaskLogger.LogError(ex5);
				base.WriteError(ex5, ErrorCategory.InvalidOperation, this.principal);
			}
			finally
			{
				if (mailboxSession != null)
				{
					mailboxSession.Dispose();
				}
				TaskLogger.LogExit();
			}
		}

		private List<string> validatedAddresses;

		private ExchangePrincipal principal;

		private string wipeRequestorSMTP;
	}
}
