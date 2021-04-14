using System;
using System.Collections.Generic;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IIndexManager
	{
		void CreateCatalog(string indexName, string databasePath, bool databaseCopyActive, RefinerUsage refinersToEnable);

		void RemoveCatalog(string indexName);

		void FlushCatalog(string indexName);

		bool CatalogExists(string indexName);

		CatalogState GetCatalogState(string indexName, out string seedingSource, out int? failureCode, out string failureReason);

		HashSet<string> GetCatalogs();

		void UpdateConfiguration();

		bool EnsureCatalog(string indexName, bool databaseCopyActive, bool suspended, RefinerUsage refinersToEnable);

		string GetTransportIndexSystem();

		bool SuspendCatalog(string indexName);

		bool ResumeCatalog(string indexName);

		string GetRootDirectory(string indexName);
	}
}
