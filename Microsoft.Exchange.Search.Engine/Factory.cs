using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Engine
{
	internal class Factory
	{
		protected Factory()
		{
		}

		internal static Hookable<Factory> Instance
		{
			get
			{
				return Factory.instance;
			}
		}

		internal static Factory Current
		{
			get
			{
				return Factory.instance.Value;
			}
		}

		internal virtual IExecutable CreateFeedingController(ISearchServiceConfig config, MdbInfo mdbInfo, IIndexStatusStore indexStatusStore, IIndexManager indexManager, IDocumentTracker tracker)
		{
			return new SearchFeedingController(config, mdbInfo, indexStatusStore, indexManager, tracker);
		}

		internal virtual ISearchServiceConfig CreateSearchServiceConfig()
		{
			return new FlightingSearchConfig();
		}

		internal virtual ISearchServiceConfig CreateSearchServiceConfig(Guid mdbGuid)
		{
			return new FlightingSearchConfig(mdbGuid);
		}

		internal virtual IDocumentTracker CreateDocumentTracker()
		{
			return new DocumentTracker();
		}

		internal virtual IIndexStatusStore CreateIndexStatusStore()
		{
			return IndexStatusStore.Instance;
		}

		private static readonly Hookable<Factory> instance = Hookable<Factory>.Create(true, new Factory());
	}
}
