using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Management.Tasks.MailboxSearch;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Stop", "MailboxSearch", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class StopMailboxSearch : ObjectActionTenantADTask<EwsStoreObjectIdParameter, MailboxDiscoverySearch>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageStopMailboxSearch(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = Utils.CreateRecipientSession(base.DomainController, base.SessionSettings);
			this.recipientSession = recipientSession;
			return new DiscoverySearchDataProvider(base.CurrentOrganizationId);
		}

		protected override IConfigurable PrepareDataObject()
		{
			try
			{
				return base.PrepareDataObject();
			}
			catch (CorruptDataException ex)
			{
				base.WriteError(new TaskException(ex.LocalizedString, ex), ErrorCategory.InvalidOperation, null);
			}
			catch (StorageTransientException innerException)
			{
				base.WriteError(new TaskException(Strings.ErrorMailboxSearchStorageTransient, innerException), ErrorCategory.WriteError, null);
			}
			catch (StoragePermanentException innerException2)
			{
				base.WriteError(new TaskException(Strings.ErrorMailboxSearchStoragePermanent, innerException2), ErrorCategory.WriteError, null);
			}
			return null;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.ExchangeRunspaceConfig == null)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
			}
			ExchangePrincipal exchangePrincipal = null;
			try
			{
				if (!string.IsNullOrEmpty(this.DataObject.Target))
				{
					exchangePrincipal = ExchangePrincipal.FromLegacyDN(this.recipientSession.SessionSettings, this.DataObject.Target, RemotingOptions.AllowCrossSite);
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			if (exchangePrincipal == null)
			{
				base.WriteError(new ObjectNotFoundException(Strings.ExceptionTargetMailboxNotFound(this.DataObject.Target, this.DataObject.Name)), ErrorCategory.InvalidOperation, null);
			}
			string serverFqdn = exchangePrincipal.MailboxInfo.Location.ServerFqdn;
			if (string.IsNullOrEmpty(serverFqdn))
			{
				base.WriteError(new ObjectNotFoundException(Strings.ExceptionUserObjectNotFound(exchangePrincipal.MailboxInfo.Location.ServerLegacyDn)), ErrorCategory.InvalidOperation, null);
			}
			Utils.CreateMailboxDiscoverySearchRequest((DiscoverySearchDataProvider)base.DataSession, this.DataObject.Name, ActionRequestType.Stop, base.ExchangeRunspaceConfig.GetRbacContext().ToString());
			SearchEventLogger.Instance.LogDiscoverySearchStopRequestedEvent(this.DataObject, base.ExchangeRunspaceConfig.GetRbacContext().ToString());
			TaskLogger.LogExit();
		}

		private IRecipientSession recipientSession;
	}
}
