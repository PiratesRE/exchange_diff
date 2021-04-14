using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.HttpProxy
{
	internal class BackEndDatabaseResourceForestCookieEntry : BackEndDatabaseCookieEntry
	{
		public BackEndDatabaseResourceForestCookieEntry(Guid database, string domainName, string resourceForest) : this(database, domainName, resourceForest, ExDateTime.UtcNow + BackEndCookieEntryBase.LongLivedBackEndServerCookieLifeTime)
		{
		}

		public BackEndDatabaseResourceForestCookieEntry(Guid database, string domainName, string resourceForest, ExDateTime expiryTime) : base(database, domainName, expiryTime)
		{
			this.ResourceForest = resourceForest;
		}

		public string ResourceForest { get; private set; }

		public override string ToString()
		{
			return base.ToString() + '~' + this.ResourceForest;
		}
	}
}
