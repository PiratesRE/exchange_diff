using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class MailboxQuarantineTaskBase : DataAccessTask<ADUser>
	{
		internal Database Database
		{
			get
			{
				return this.database;
			}
			set
			{
				this.database = value;
			}
		}

		internal Guid ExchangeGuid
		{
			get
			{
				return this.exchangeGuid;
			}
			set
			{
				this.exchangeGuid = value;
			}
		}

		internal string Server
		{
			get
			{
				return this.server;
			}
			set
			{
				this.server = value;
			}
		}

		internal DatabaseLocationInfo DbLocationInfo
		{
			get
			{
				return this.dbLocationInfo;
			}
			set
			{
				this.dbLocationInfo = value;
			}
		}

		internal RegistryKey RegistryKeyHive
		{
			get
			{
				return this.registryKeyHive;
			}
			set
			{
				this.registryKeyHive = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public GeneralMailboxIdParameter Identity
		{
			get
			{
				return (GeneralMailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || exception is StorageTransientException || DataAccessHelper.IsDataAccessKnownException(exception) || base.IsKnownException(exception);
		}

		protected override IConfigDataProvider CreateSession()
		{
			TaskLogger.LogEnter();
			ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(null, null);
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, rootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			sessionSettings = ADSessionSettings.RescopeToSubtree(sessionSettings);
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 176, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\Mailbox\\MailboxQuarantineTaskBase.cs");
			this.recipientSession.UseGlobalCatalog = true;
			this.systemConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 186, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\Mailbox\\MailboxQuarantineTaskBase.cs");
			TaskLogger.LogExit();
			return this.systemConfigSession;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			ADObjectId adobjectId = null;
			ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(this.Identity, this.recipientSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(this.Identity.ToString())));
			ADUser aduser = adrecipient as ADUser;
			if (aduser != null)
			{
				this.exchangeGuid = aduser.ExchangeGuid;
				adobjectId = aduser.Database;
			}
			else
			{
				ADSystemMailbox adsystemMailbox = adrecipient as ADSystemMailbox;
				if (adsystemMailbox != null)
				{
					this.exchangeGuid = adsystemMailbox.ExchangeGuid;
					adobjectId = adsystemMailbox.Database;
				}
			}
			if (adobjectId == null)
			{
				base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidObjectMissingCriticalProperty(adrecipient.GetType().Name, this.Identity.ToString(), IADMailStorageSchema.Database.Name)), ErrorCategory.InvalidArgument, adrecipient);
			}
			DatabaseIdParameter id = new DatabaseIdParameter(adobjectId);
			this.database = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(id, this.systemConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(adobjectId.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(adobjectId.ToString())));
			try
			{
				this.dbLocationInfo = ActiveManager.GetActiveManagerInstance().GetServerForDatabase(this.database.Id.ObjectGuid);
				this.server = this.DbLocationInfo.ServerFqdn.Split(new char[]
				{
					'.'
				})[0];
			}
			catch (DatabaseNotFoundException exception)
			{
				base.WriteError(exception, ExchangeErrorCategory.ServerOperation, null);
			}
			catch (ObjectNotFoundException exception2)
			{
				base.WriteError(exception2, ExchangeErrorCategory.ServerOperation, null);
			}
			try
			{
				if (this.registryKeyHive == null)
				{
					this.registryKeyHive = this.OpenHive(this.dbLocationInfo.ServerFqdn);
				}
			}
			catch (IOException ex)
			{
				base.WriteError(new FailedMailboxQuarantineException(this.Identity.ToString(), ex.ToString()), ErrorCategory.ObjectNotFound, null);
			}
			catch (SecurityException ex2)
			{
				base.WriteError(new FailedMailboxQuarantineException(this.Identity.ToString(), ex2.ToString()), ErrorCategory.SecurityError, null);
			}
			catch (UnauthorizedAccessException ex3)
			{
				base.WriteError(new FailedMailboxQuarantineException(this.Identity.ToString(), ex3.ToString()), ErrorCategory.PermissionDenied, null);
			}
			TaskLogger.LogExit();
		}

		internal RegistryKey OpenHive(string server)
		{
			if (string.IsNullOrEmpty(server))
			{
				throw new ArgumentNullException("server");
			}
			return RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, server);
		}

		internal bool GetMailboxQuarantineStatus()
		{
			if (this.mapiSession == null)
			{
				this.mapiSession = new MapiAdministrationSession(this.DbLocationInfo.ServerLegacyDN, Fqdn.Parse(this.DbLocationInfo.ServerFqdn));
			}
			StoreMailboxIdParameter id = StoreMailboxIdParameter.Parse(this.exchangeGuid.ToString());
			bool isQuarantined;
			using (MailboxStatistics mailboxStatistics = (MailboxStatistics)base.GetDataObject<MailboxStatistics>(id, this.mapiSession, MapiTaskHelper.ConvertDatabaseADObjectToDatabaseId(this.database), new LocalizedString?(Strings.ErrorStoreMailboxNotFound(this.Identity.ToString(), this.Database.Identity.ToString())), new LocalizedString?(Strings.ErrorStoreMailboxNotUnique(this.Identity.ToString(), this.Database.Identity.ToString()))))
			{
				isQuarantined = mailboxStatistics.IsQuarantined;
			}
			return isQuarantined;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.registryKeyHive != null)
				{
					this.registryKeyHive.Close();
					this.RegistryKeyHive = null;
				}
				if (this.mapiSession != null)
				{
					this.mapiSession.Dispose();
					this.mapiSession = null;
				}
			}
			base.Dispose(disposing);
		}

		internal const string ParameterIdentity = "Identity";

		private Database database;

		private Guid exchangeGuid;

		private string server;

		private DatabaseLocationInfo dbLocationInfo;

		private RegistryKey registryKeyHive;

		private IRecipientSession recipientSession;

		private IConfigurationSession systemConfigSession;

		private MapiAdministrationSession mapiSession;

		internal static readonly string QuarantineBaseRegistryKey = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS";
	}
}
