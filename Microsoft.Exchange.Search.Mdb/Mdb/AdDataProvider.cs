using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class AdDataProvider
	{
		private AdDataProvider(IDiagnosticsSession diagnosticsSession)
		{
			this.adSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 41, ".ctor", "f:\\15.00.1497\\sources\\dev\\Search\\src\\Mdb\\AdDataProvider.cs");
			this.diagnosticsSession = diagnosticsSession;
		}

		public static AdDataProvider Create(IDiagnosticsSession diagnosticsSession)
		{
			return new AdDataProvider(diagnosticsSession);
		}

		public ADNotificationRequestCookie RegisterChangeNotification(ADNotificationCallback callback)
		{
			this.diagnosticsSession.TraceDebug("Registrering for a change notification.", new object[0]);
			ADObjectId databaseRootId = this.GetDatabasesContainerId();
			ADNotificationRequestCookie cookie = null;
			this.RunAdOperation(delegate
			{
				cookie = ADNotificationAdapter.RegisterChangeNotification<DatabaseCopy>(databaseRootId, callback);
			}, false, 0, Strings.FailedToRegisterDatabaseChangeNotification);
			return cookie;
		}

		public void UnRegisterChangeNotification(ADNotificationRequestCookie cookie)
		{
			this.diagnosticsSession.TraceDebug<ADNotificationRequestCookie>("Unregistering from a database change notification with a cookie {0}.", cookie);
			this.RunAdOperation(delegate
			{
				ADNotificationAdapter.UnregisterChangeNotification(cookie);
			}, true, 0, Strings.FailedToUnRegisterDatabaseChangeNotification);
		}

		public MailboxDatabase[] GetLocalMailboxDatabases(Server server)
		{
			this.diagnosticsSession.TraceDebug<string>("Retrieving all mailbox databases on server {0}", server.Fqdn);
			MailboxDatabase[] databases = null;
			this.RunAdOperation(delegate
			{
				databases = server.GetMailboxDatabases();
			}, true, 0, Strings.FailedToGetMailboxDatabases);
			return databases;
		}

		public MiniServer[] GetServers(ICollection<Guid> serverIds, int maxResults)
		{
			List<QueryFilter> list = new List<QueryFilter>(serverIds.Count);
			foreach (Guid guid in serverIds)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid));
			}
			QueryFilter queryFilter;
			switch (list.Count)
			{
			case 0:
				return new MiniServer[0];
			case 1:
				queryFilter = list[0];
				break;
			default:
				queryFilter = new OrFilter(list.ToArray());
				break;
			}
			this.diagnosticsSession.TraceDebug<QueryFilter>("Getting servers in the dag using query filter:{0}", queryFilter);
			MiniServer[] servers = null;
			this.RunAdOperation(delegate
			{
				servers = this.adSession.FindMiniServer(null, QueryScope.SubTree, queryFilter, null, maxResults, null);
			}, true, 3, Strings.AdOperationFailed);
			return servers ?? new MiniServer[0];
		}

		public ADObjectId GetDatabasesContainerId()
		{
			this.diagnosticsSession.TraceDebug("Getting databases container id.", new object[0]);
			ADObjectId databaseRootId = null;
			this.RunAdOperation(delegate
			{
				databaseRootId = this.adSession.GetDatabasesContainerId();
			}, true, 0, Strings.FailedToGetDatabasesContainerId);
			return databaseRootId;
		}

		public Database FindDatabase(Guid databaseGuid)
		{
			this.diagnosticsSession.TraceDebug<Guid>("Looking for a database with mdbGuid {0}", databaseGuid);
			Database database = null;
			this.RunAdOperation(delegate
			{
				database = this.adSession.FindDatabaseByGuid<Database>(databaseGuid);
			}, true, 0, Strings.AdOperationFailed);
			return database;
		}

		public Server GetLocalServer()
		{
			this.diagnosticsSession.TraceDebug("Getting a local server.", new object[0]);
			Server server = null;
			this.RunAdOperation(delegate
			{
				server = LocalServer.GetServer();
			}, true, 0, Strings.FailedToGetLocalServer);
			if (server == null)
			{
				throw new AdDataProvider.AdTransientException(Strings.FailedToGetLocalServer);
			}
			return server;
		}

		private void RunAdOperation(Action adAction, bool wrapAdOperation, int retryCount, LocalizedString exceptionString)
		{
			if (!wrapAdOperation)
			{
				adAction();
				return;
			}
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				adAction();
			}, retryCount);
			this.diagnosticsSession.TraceDebug<ADOperationResult, Exception>("Finished Ad operation. Result:{0}. Exception:{1}", adoperationResult, adoperationResult.Exception);
			switch (adoperationResult.ErrorCode)
			{
			case ADOperationErrorCode.Success:
				return;
			case ADOperationErrorCode.RetryableError:
				throw new AdDataProvider.AdTransientException(exceptionString, adoperationResult.Exception);
			case ADOperationErrorCode.PermanentError:
				throw new AdDataProvider.AdPermanentException(exceptionString, adoperationResult.Exception);
			default:
				throw new ArgumentException(string.Format("Unknown result error code {0}", adoperationResult.ErrorCode));
			}
		}

		private readonly ITopologyConfigurationSession adSession;

		private readonly IDiagnosticsSession diagnosticsSession;

		public class AdTransientException : ComponentFailedTransientException
		{
			public AdTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
			{
			}

			public AdTransientException(LocalizedString message) : base(message)
			{
			}
		}

		public class AdPermanentException : ComponentFailedPermanentException
		{
			public AdPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
			{
			}
		}
	}
}
