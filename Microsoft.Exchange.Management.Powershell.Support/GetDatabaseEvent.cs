using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "DatabaseEvent")]
	public sealed class GetDatabaseEvent : GetMapiObjectTask<DatabaseIdParameter, DatabaseEvent>
	{
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "Database")]
		public override DatabaseIdParameter Identity
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

		[Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "Server")]
		public override ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Server")]
		public SwitchParameter IncludePassive
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludePassive"] ?? false);
			}
			set
			{
				base.Fields["IncludePassive"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Database")]
		[ValidateNotNullOrEmpty]
		public ServerIdParameter CopyOnServer
		{
			get
			{
				return (ServerIdParameter)base.Fields["CopyOnServer"];
			}
			set
			{
				base.Fields["CopyOnServer"] = value;
			}
		}

		[ValidateRange(1L, 9223372036854775807L)]
		[Parameter(Mandatory = false)]
		public long StartCounter
		{
			get
			{
				return this.startCounter;
			}
			set
			{
				this.startCounter = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid MailboxGuid
		{
			get
			{
				return (Guid)(base.Fields["MailboxGuid"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["MailboxGuid"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseEventNames EventNames
		{
			get
			{
				return (DatabaseEventNames)(base.Fields["EventNames"] ?? 0);
			}
			set
			{
				base.Fields["EventNames"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeMoveDestinationEvents
		{
			get
			{
				return this.includeMoveDestinationEvents;
			}
			set
			{
				this.includeMoveDestinationEvents = value;
			}
		}

		protected override Unlimited<uint> DefaultResultSize
		{
			get
			{
				return GetDatabaseEvent.defaultResultSize;
			}
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetTaskBaseModuleFactory();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if (!this.ResultSize.IsUnlimited && (this.ResultSize.Value == 0U || 2147483647U < this.ResultSize.Value))
			{
				base.ThrowTerminatingError(new InvalidOperationException(Strings.ErrorResultSizeOutOfRange(1.ToString(), int.MaxValue.ToString())), ErrorCategory.InvalidArgument, null);
			}
			base.InternalBeginProcessing();
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			TaskLogger.LogEnter();
			if (this.mapiSession == null)
			{
				this.mapiSession = new MapiAdministrationSession(this.server.ExchangeLegacyDN, Fqdn.Parse(this.server.Fqdn));
			}
			else
			{
				this.mapiSession.RedirectServer(this.server.ExchangeLegacyDN, Fqdn.Parse(this.server.Fqdn));
			}
			TaskLogger.LogExit();
			return this.mapiSession;
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

		private void ResolveDatabaseAndServer()
		{
			ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
			if (this.Server != null)
			{
				this.server = MapiTaskHelper.GetMailboxServer(this.Server, (ITopologyConfigurationSession)this.ConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			else if (this.Identity != null)
			{
				DatabaseIdParameter identity = this.Identity;
				identity.AllowLegacy = false;
				Database database = (Database)base.GetDataObject<Database>(identity, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(identity.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(identity.ToString())));
				ServerIdParameter serverIdParameter;
				if (this.CopyOnServer != null)
				{
					serverIdParameter = this.CopyOnServer;
				}
				else
				{
					if (database.Server == null)
					{
						base.WriteError(new MdbAdminTaskException(Strings.ErrorInvalidObjectMissingCriticalProperty(typeof(Database).Name, identity.ToString(), DatabaseSchema.Server.Name)), ErrorCategory.InvalidArgument, database);
					}
					DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(database.Guid);
					serverIdParameter = ServerIdParameter.Parse(serverForDatabase.ServerFqdn);
				}
				this.server = (Server)base.GetDataObject<Server>(serverIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
				if (!this.server.IsExchange2007OrLater || !this.server.IsMailboxServer)
				{
					base.WriteError(new MdbAdminTaskException(Strings.ErrorLocalServerIsNotMailboxServer), ErrorCategory.InvalidArgument, this.server);
				}
				this.databases = new List<Database>(new Database[]
				{
					database
				});
			}
			else
			{
				ServerIdParameter serverIdParameter2 = new ServerIdParameter();
				this.server = (Server)base.GetDataObject<Server>(serverIdParameter2, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorLocalMachineIsNotExchangeServer), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter2.ToString())));
				if (!this.server.IsExchange2007OrLater || !this.server.IsMailboxServer)
				{
					base.WriteError(new MdbAdminTaskException(Strings.ErrorLocalServerIsNotMailboxServer), ErrorCategory.InvalidArgument, this.server);
				}
			}
			if (this.databases.Count == 0)
			{
				this.databases = StoreCommon.PopulateDatabasesFromServer(activeManagerInstance, this.server, this.IncludePassive);
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.server = null;
			this.FreeMapiSession();
			this.ResolveDatabaseAndServer();
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			Restriction restriction = this.BuildRestriction();
			int num = (int)(this.ResultSize.IsUnlimited ? uint.MaxValue : this.ResultSize.Value);
			if ((0 < num && 2147483647 > num) || -1 == num)
			{
				foreach (Database database in this.databases)
				{
					try
					{
						IEnumerable<DatabaseEvent> enumerable = this.ReadEvents(this.mapiSession, database, this.startCounter, restriction, this.includeMoveDestinationEvents, num);
						foreach (DatabaseEvent dataObject in enumerable)
						{
							this.WriteResult(dataObject);
						}
					}
					catch (DatabaseUnavailableException ex)
					{
						if (this.Identity == null)
						{
							base.WriteWarning(ex.Message);
						}
						else
						{
							base.WriteError(ex, ErrorCategory.ResourceUnavailable, database);
						}
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || DataAccessHelper.IsDataAccessKnownException(e);
		}

		private Restriction BuildRestriction()
		{
			if (this.IsFieldSet("MailboxGuid") && this.IsFieldSet("EventNames"))
			{
				return Restriction.And(new Restriction[]
				{
					Restriction.EQ(PropTag.EventMailboxGuid, this.MailboxGuid.ToByteArray()),
					Restriction.BitMaskNonZero(PropTag.EventMask, (int)this.EventNames)
				});
			}
			if (this.IsFieldSet("MailboxGuid"))
			{
				return Restriction.EQ(PropTag.EventMailboxGuid, this.MailboxGuid.ToByteArray());
			}
			if (this.IsFieldSet("EventNames"))
			{
				return Restriction.BitMaskNonZero(PropTag.EventMask, (int)this.EventNames);
			}
			return null;
		}

		private bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
		}

		internal IEnumerable<DatabaseEvent> ReadEvents(MapiSession mapiSession, Database database, long startCounter, Restriction restriction, bool includeMoveDestinationEvents, int resultSize)
		{
			if (mapiSession == null)
			{
				throw new ArgumentException("mapiSession");
			}
			if (database == null)
			{
				throw new ArgumentException("database");
			}
			int count = 0;
			long endCounter = (0L < startCounter) ? (startCounter - 1L) : 0L;
			MapiEvent[] events = null;
			DatabaseId databaseId = MapiTaskHelper.ConvertDatabaseADObjectToDatabaseId(database);
			MapiEventManager eventManager = MapiEventManager.Create(mapiSession.Administration, Guid.NewGuid(), databaseId.Guid);
			for (;;)
			{
				startCounter = endCounter + 1L;
				ReadEventsFlags flags = includeMoveDestinationEvents ? ReadEventsFlags.IncludeMoveDestinationEvents : ReadEventsFlags.None;
				mapiSession.InvokeWithWrappedException(delegate()
				{
					events = eventManager.ReadEvents(startCounter, (0 < resultSize) ? resultSize : 1000, 0, restriction, flags, out endCounter);
				}, Strings.ErrorCannotReadDatabaseEvents(databaseId.ToString()), databaseId);
				foreach (MapiEvent mapiEvent in events)
				{
					yield return new DatabaseEvent(mapiEvent, databaseId, this.server, database.Server.ObjectGuid == this.server.Guid);
					count++;
					if (0 < resultSize && count == resultSize)
					{
						goto Block_6;
					}
				}
				if (endCounter == startCounter)
				{
					goto Block_8;
				}
			}
			Block_6:
			yield break;
			Block_8:
			yield break;
		}

		private const string ParameterMailboxGuid = "MailboxGuid";

		private const string ParameterEventNames = "EventNames";

		private static readonly Unlimited<uint> defaultResultSize = new Unlimited<uint>(1000U);

		private MapiAdministrationSession mapiSession;

		private Server server;

		private long startCounter = 1L;

		private bool includeMoveDestinationEvents;

		private List<Database> databases = new List<Database>();
	}
}
