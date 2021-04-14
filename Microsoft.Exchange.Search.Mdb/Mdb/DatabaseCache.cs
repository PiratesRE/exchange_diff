using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class DatabaseCache
	{
		private DatabaseCache(IDiagnosticsSession diagnosticsSession)
		{
			this.existanceDictionary = new Dictionary<Guid, bool>(40);
			this.diagnosticsSession = diagnosticsSession;
		}

		public static DatabaseCache Create(IDiagnosticsSession diagnosticsSession)
		{
			return new DatabaseCache(diagnosticsSession);
		}

		public bool DatabaseExists(Guid mdbGuid)
		{
			if (ExEnvironment.IsTest)
			{
				return this.GetDatabaseExistance(mdbGuid);
			}
			bool databaseExistance;
			lock (this.existanceDictionary)
			{
				if (!this.existanceDictionary.TryGetValue(mdbGuid, out databaseExistance))
				{
					databaseExistance = this.GetDatabaseExistance(mdbGuid);
					this.existanceDictionary[mdbGuid] = databaseExistance;
				}
			}
			return databaseExistance;
		}

		private bool GetDatabaseExistance(Guid guid)
		{
			AdDataProvider adDataProvider = AdDataProvider.Create(this.diagnosticsSession);
			Database database = adDataProvider.FindDatabase(guid);
			return database != null;
		}

		private readonly Dictionary<Guid, bool> existanceDictionary;

		private readonly IDiagnosticsSession diagnosticsSession;
	}
}
