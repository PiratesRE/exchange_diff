using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Update", "StoreMailboxState", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
	public sealed class UpdateStoreMailboxState : DataAccessTask<ADUser>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity")]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity")]
		public StoreMailboxIdParameter Identity
		{
			get
			{
				return (StoreMailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUpdateStoreMailboxStateMailboxIdentity(this.Database.ToString(), this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 92, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\Mailbox\\UpdateStoreMailboxState.cs");
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				Database database = null;
				database = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.Database, base.DataSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.Database.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.Database.ToString())), ExchangeErrorCategory.Client);
				if (database.Recovery)
				{
					base.WriteError(new TaskArgumentException(Strings.ErrorInvalidOperationOnRecoveryMailboxDatabase(this.Database.ToString())), ExchangeErrorCategory.Client, this.Identity);
				}
				DatabaseLocationInfo databaseLocationInfo = null;
				try
				{
					databaseLocationInfo = ActiveManager.GetActiveManagerInstance().GetServerForDatabase(database.Id.ObjectGuid);
				}
				catch (ObjectNotFoundException exception)
				{
					base.WriteError(exception, ExchangeErrorCategory.ServerOperation, null);
				}
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseConnectionAdminRpcInterface(databaseLocationInfo.ServerFqdn));
				}
				this.mapiSession = new MapiAdministrationSession(databaseLocationInfo.ServerLegacyDN, Fqdn.Parse(databaseLocationInfo.ServerFqdn));
				this.guidMdb = database.Guid;
				if (!Guid.TryParse(this.Identity.ToString(), out this.guidMailbox))
				{
					base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidGuidFormat), ErrorCategory.InvalidArgument, this.Identity.ToString());
				}
			}
			catch (MapiPermanentException exception2)
			{
				base.WriteError(exception2, ExchangeErrorCategory.ServerOperation, this.Identity);
			}
			catch (MapiRetryableException exception3)
			{
				base.WriteError(exception3, ExchangeErrorCategory.ServerTransient, this.Identity);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProvisioningValidation()
		{
			ProvisioningValidationError[] array = ProvisioningLayer.Validate(this, null);
			if (array != null && array.Length > 0)
			{
				foreach (ProvisioningValidationError provisioningValidationError in array)
				{
					ProvisioningValidationException exception = new ProvisioningValidationException(provisioningValidationError.Description, provisioningValidationError.AgentName, provisioningValidationError.Exception);
					this.WriteError(exception, (ErrorCategory)provisioningValidationError.ErrorCategory, null, false);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.VerboseUpdateStoreMailboxState(this.guidMailbox.ToString(), this.guidMdb.ToString()));
				}
				this.mapiSession.Administration.SyncMailboxWithDS(this.guidMdb, this.guidMailbox);
			}
			catch (MapiPermanentException exception)
			{
				base.WriteError(exception, ExchangeErrorCategory.ServerOperation, this.Identity);
			}
			catch (MapiRetryableException exception2)
			{
				base.WriteError(exception2, ExchangeErrorCategory.ServerTransient, this.Identity);
			}
			finally
			{
				this.DisposeObjects();
				TaskLogger.LogExit();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DisposeObjects();
			}
			base.Dispose(disposing);
		}

		private void DisposeObjects()
		{
			if (this.mapiSession != null)
			{
				this.mapiSession.Dispose();
				this.mapiSession = null;
			}
		}

		protected override void InternalEndProcessing()
		{
			this.DisposeObjects();
		}

		protected override void InternalStopProcessing()
		{
			this.DisposeObjects();
		}

		private const string ParameterDatabase = "Database";

		private const string ParameterStoreMailboxIdentity = "Identity";

		private MapiAdministrationSession mapiSession;

		private Guid guidMdb = Guid.Empty;

		private Guid guidMailbox = Guid.Empty;
	}
}
