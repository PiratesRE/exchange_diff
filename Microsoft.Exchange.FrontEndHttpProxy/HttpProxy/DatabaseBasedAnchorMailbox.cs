using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.HttpProxy
{
	internal abstract class DatabaseBasedAnchorMailbox : AnchorMailbox
	{
		public DatabaseBasedAnchorMailbox(AnchorSource anchorSource, object sourceObject, IRequestContext requestContext) : base(anchorSource, sourceObject, requestContext)
		{
		}

		public bool UseServerCookie { get; set; }

		public virtual ADObjectId GetDatabase()
		{
			return base.GetCacheEntry().Database;
		}

		public override BackEndCookieEntryBase BuildCookieEntryForTarget(BackEndServer routingTarget, bool proxyToDownLevel, bool useResourceForest)
		{
			if (routingTarget == null)
			{
				throw new ArgumentNullException("routingTarget");
			}
			if (!proxyToDownLevel && !this.UseServerCookie)
			{
				ADObjectId database = this.GetDatabase();
				if (database != null)
				{
					if (useResourceForest)
					{
						return new BackEndDatabaseResourceForestCookieEntry(database.ObjectGuid, string.Empty, database.PartitionFQDN);
					}
					return new BackEndDatabaseCookieEntry(database.ObjectGuid, string.Empty);
				}
			}
			return base.BuildCookieEntryForTarget(routingTarget, proxyToDownLevel, useResourceForest);
		}
	}
}
