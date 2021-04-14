using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	public abstract class GetFolderObjectBase<TDataObject> : GetTaskBase<TDataObject> where TDataObject : IConfigurable, new()
	{
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["DatabaseId"];
			}
			set
			{
				base.Fields["DatabaseId"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public MapiEntryId FolderEntryId
		{
			get
			{
				return (MapiEntryId)base.Fields["FolderEntryId"];
			}
			set
			{
				base.Fields["FolderEntryId"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public Guid MailboxGuid
		{
			get
			{
				if (!base.Fields.IsModified("MailboxGuid"))
				{
					return Guid.Empty;
				}
				return (Guid)base.Fields["MailboxGuid"];
			}
			set
			{
				base.Fields["MailboxGuid"] = value;
			}
		}

		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(MapiPermanentException).IsInstanceOfType(exception) || typeof(MapiRetryableException).IsInstanceOfType(exception);
		}

		internal override IConfigurationSession ConfigurationSession
		{
			get
			{
				return this.configurationSession;
			}
		}

		internal MapiAdministrationSession MapiSession
		{
			get
			{
				return this.mapiSession;
			}
		}

		internal Server TargetServer
		{
			get
			{
				return this.targetServer;
			}
		}

		internal Database TargetDatabase
		{
			get
			{
				return this.targetDatabase;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			TaskLogger.LogEnter();
			if (this.mapiSession == null)
			{
				this.mapiSession = new MapiAdministrationSession(this.TargetServer.ExchangeLegacyDN, Fqdn.Parse(this.TargetServer.Fqdn));
			}
			else
			{
				this.mapiSession.RedirectServer(this.TargetServer.ExchangeLegacyDN, Fqdn.Parse(this.TargetServer.Fqdn));
			}
			TaskLogger.LogExit();
			return this.mapiSession;
		}

		private void ResolveDatabaseAndServer()
		{
			DatabaseIdParameter database = this.Database;
			database.AllowLegacy = false;
			Database database2 = (Database)base.GetDataObject<Database>(database, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(database.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(database.ToString())));
			if (database2.Server == null)
			{
				base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidObjectMissingCriticalProperty(typeof(Database).Name, database.ToString(), DatabaseSchema.Server.Name)), ErrorCategory.InvalidArgument, database2);
			}
			ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
			DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(database2.Guid);
			ServerIdParameter serverIdParameter = ServerIdParameter.Parse(serverForDatabase.ServerFqdn);
			this.targetServer = (Server)base.GetDataObject<Server>(serverIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
			if (!this.TargetServer.IsE14OrLater || !this.TargetServer.IsMailboxServer)
			{
				base.WriteError(new MdbAdminTaskException(Strings.ErrorLocalServerIsNotMailboxServer), ErrorCategory.InvalidArgument, this.TargetServer);
			}
			this.targetDatabase = database2;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.FreeMapiSession();
			}
			base.Dispose(disposing);
		}

		private void FreeMapiSession()
		{
			if (this.mapiSession != null)
			{
				this.mapiSession.Dispose();
				this.mapiSession = null;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 236, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Support\\Store\\GetFolderObjectBase.cs");
			TaskLogger.LogExit();
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.targetServer = null;
			this.targetDatabase = null;
			this.FreeMapiSession();
			this.ResolveDatabaseAndServer();
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		private const string ParameterDatabaseId = "DatabaseId";

		private const string ParameterFolderEntryId = "FolderEntryId";

		private const string ParameterMailboxId = "MailboxGuid";

		private MapiAdministrationSession mapiSession;

		private IConfigurationSession configurationSession;

		private Server targetServer;

		private Database targetDatabase;
	}
}
