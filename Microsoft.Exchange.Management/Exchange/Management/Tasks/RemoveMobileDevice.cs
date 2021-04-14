using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Remove", "MobileDevice", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class RemoveMobileDevice : RemoveSystemConfigurationObjectTask<MobileDeviceIdParameter, MobileDevice>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.IsRemoteWipePending())
				{
					return Strings.ConfirmationMessageRemoveMobileDeviceWhileRemoteWipeIsPending(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageRemoveMobileDevice(this.Identity.ToString());
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.mailboxSession != null)
			{
				this.mailboxSession.Dispose();
				this.mailboxSession = null;
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
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 133, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\AirSync\\RemoveMobileDevice.cs");
			}
			configurationSession.UseConfigNC = false;
			configurationSession.UseGlobalCatalog = (base.DomainController == null && base.ServerSettings.ViewEntireForest);
			return configurationSession;
		}

		protected override void InternalValidate()
		{
			MailboxIdParameter mailboxIdParameter = null;
			try
			{
				base.InternalValidate();
			}
			catch (ManagementObjectNotFoundException)
			{
				if (this.Identity == null || !this.TryGetMailboxIdFromIdentity(this.Identity.ToString(), out mailboxIdParameter))
				{
					throw;
				}
				this.deviceExistsinAD = false;
			}
			if (base.HasErrors)
			{
				return;
			}
			if (MobileDeviceTaskHelper.IsRunningUnderMyOptionsRole(this, base.TenantGlobalCatalogSession, base.SessionSettings))
			{
				ADObjectId adobjectId;
				if (!base.TryGetExecutingUserId(out adobjectId))
				{
					throw new ExecutingUserPropertyNotFoundException("executingUserid");
				}
				if (!base.DataObject.Id.Parent.Parent.Equals(adobjectId) || (mailboxIdParameter != null && !mailboxIdParameter.Equals(adobjectId)))
				{
					base.WriteError(new LocalizedException(Strings.ErrorObjectNotFound(this.Identity.ToString())), ErrorCategory.InvalidArgument, null);
				}
			}
			Exception ex = null;
			if (mailboxIdParameter == null)
			{
				mailboxIdParameter = this.Identity.GetMailboxId();
			}
			if (mailboxIdParameter == null && base.DataObject != null)
			{
				this.Identity = new MobileDeviceIdParameter(base.DataObject);
				mailboxIdParameter = this.Identity.GetMailboxId();
			}
			if (mailboxIdParameter == null)
			{
				base.WriteError(new LocalizedException(Strings.ErrorObjectNotFound(this.Identity.ToString())), ErrorCategory.InvalidArgument, null);
			}
			this.principal = MobileDeviceTaskHelper.GetExchangePrincipal(base.SessionSettings, this.CreateTenantGlobalCatalogSession(base.SessionSettings), mailboxIdParameter, "Remove-MobileDevice", out ex);
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidArgument, null);
			}
			if (DeviceInfo.IsThrottlingLimitsExceeded((IConfigurationSession)base.DataSession, this.principal, out this.activeSyncDevices))
			{
				uint maxDeviceDeleteCount = 0U;
				using (IBudget budget = StandardBudget.Acquire(this.principal.Sid, BudgetType.Eas, this.principal.MailboxInfo.OrganizationId.ToADSessionSettings()))
				{
					IThrottlingPolicy throttlingPolicy = budget.ThrottlingPolicy;
					maxDeviceDeleteCount = throttlingPolicy.EasMaxDeviceDeletesPerMonth.Value;
				}
				base.WriteError(new LocalizedException(Strings.MaxDeviceDeletesPerMonthReached(this.activeSyncDevices.ObjectsDeletedThisPeriod, maxDeviceDeleteCount)), ErrorCategory.WriteError, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				this.OpenMailboxSession();
				DeviceIdentity deviceIdentity = new DeviceIdentity(this.Identity.DeviceId, this.Identity.DeviceType, this.Identity.ClientType);
				if (!DeviceInfo.RemoveDeviceFromMailbox(this.mailboxSession, deviceIdentity) && !this.deviceExistsinAD)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorObjectNotFound(this.Identity.ToString())), ErrorCategory.WriteError, null);
				}
				if (this.deviceExistsinAD)
				{
					base.InternalProcessRecord();
				}
				DeviceInfo.UpdateThrottlingData((IConfigurationSession)base.DataSession, this.activeSyncDevices);
			}
			catch (DataSourceTransientException ex)
			{
				TaskLogger.LogError(ex);
				base.WriteError(ex, ErrorCategory.ReadError, this.principal);
			}
			catch (StorageTransientException ex2)
			{
				TaskLogger.LogError(ex2);
				base.WriteError(ex2, ErrorCategory.ReadError, this.principal);
			}
			catch (DataSourceOperationException ex3)
			{
				TaskLogger.LogError(ex3);
				base.WriteError(ex3, ErrorCategory.InvalidOperation, this.principal);
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
				if (this.mailboxSession != null)
				{
					this.mailboxSession.Dispose();
					this.mailboxSession = null;
				}
			}
		}

		private void OpenMailboxSession()
		{
			if (this.mailboxSession == null)
			{
				this.mailboxSession = MailboxSession.OpenAsAdmin(this.principal, CultureInfo.InvariantCulture, "Client=Management;Action=Remove-ActiveSyncDevice");
			}
		}

		private bool TryGetMailboxIdFromIdentity(string identity, out MailboxIdParameter mailboxId)
		{
			mailboxId = null;
			if (string.IsNullOrEmpty(identity))
			{
				return false;
			}
			int num = identity.IndexOf('/');
			if (num <= 0)
			{
				return false;
			}
			int num2 = identity.IndexOf("ExchangeActiveSyncDevices");
			if (num2 <= 0)
			{
				return false;
			}
			string text = identity.Substring(0, num2 - 1);
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				text = text.Substring(text.LastIndexOf('/') + 1);
			}
			mailboxId = new MailboxIdParameter(text);
			return true;
		}

		private bool IsRemoteWipePending()
		{
			bool result;
			try
			{
				this.OpenMailboxSession();
				DeviceInfo deviceInfo = MobileDeviceTaskHelper.GetDeviceInfo(this.mailboxSession, this.Identity);
				if (deviceInfo == null)
				{
					result = false;
				}
				else
				{
					result = ((deviceInfo.WipeRequestTime != null || deviceInfo.WipeSentTime != null) && deviceInfo.WipeAckTime == null);
				}
			}
			catch
			{
				if (this.mailboxSession != null)
				{
					this.mailboxSession.Dispose();
					this.mailboxSession = null;
				}
				throw;
			}
			return result;
		}

		private const char ElementSeparatorChar = '/';

		private ExchangePrincipal principal;

		private MailboxSession mailboxSession;

		private ActiveSyncDevices activeSyncDevices;

		private bool deviceExistsinAD = true;
	}
}
