using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class MdbCacheQuery : MdbCache.IQuery
	{
		public static MdbCacheQuery GetInstance()
		{
			if (MdbCacheQuery.singleton == null)
			{
				MdbCacheQuery.singleton = new MdbCacheQuery();
			}
			return MdbCacheQuery.singleton;
		}

		public bool TryGetDatabasePaths(out Dictionary<Guid, string> paths)
		{
			RequestDetailsLogger getRequestDetailsLogger = OwaApplication.GetRequestDetailsLogger;
			if (getRequestDetailsLogger != null)
			{
				return this.TryExecuteWithExistingLogger(getRequestDetailsLogger, out paths);
			}
			return this.TryExecuteWithNewLogger(out paths);
		}

		private bool TryExecuteWithExistingLogger(RequestDetailsLogger logger, out Dictionary<Guid, string> result)
		{
			Dictionary<Guid, string> paths = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				paths = this.ExecuteQuery(logger);
			});
			result = paths;
			return result != null;
		}

		private bool TryExecuteWithNewLogger(out Dictionary<Guid, string> result)
		{
			Dictionary<Guid, string> paths = null;
			SimulatedWebRequestContext.ExecuteWithoutUserContext("WAC.MdbCacheUpdate", delegate(RequestDetailsLogger logger)
			{
				WacUtilities.SetEventId(logger, "WAC.MdbCacheUpdate");
				ADNotificationAdapter.RunADOperation(delegate()
				{
					paths = this.ExecuteQuery(logger);
				});
			});
			result = paths;
			return result != null;
		}

		private Dictionary<Guid, string> ExecuteQuery(RequestDetailsLogger logger)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			ADTopologyConfigurationSession adtopologyConfigurationSession = new ADTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Server server = adtopologyConfigurationSession.ReadLocalServer();
			Database[] databases = server.GetDatabases();
			stopwatch.Stop();
			logger.ActivityScope.SetProperty(WacRequestHandlerMetadata.MdbCacheReloadTime, stopwatch.ElapsedMilliseconds.ToString());
			logger.ActivityScope.SetProperty(WacRequestHandlerMetadata.MdbCacheSize, databases.Length.ToString());
			Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>(databases.Length);
			foreach (Database database in databases)
			{
				string directoryName = Path.GetDirectoryName(database.EdbFilePath.PathName);
				string value = Path.Combine(directoryName, "OwaCobalt");
				dictionary.Add(database.Guid, value);
			}
			return dictionary;
		}

		private static MdbCacheQuery singleton;
	}
}
