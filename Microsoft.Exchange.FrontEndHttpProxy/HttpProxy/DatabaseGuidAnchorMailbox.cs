using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.HttpProxy
{
	internal class DatabaseGuidAnchorMailbox : DatabaseBasedAnchorMailbox
	{
		public DatabaseGuidAnchorMailbox(Guid databaseGuid, IRequestContext requestContext) : base(AnchorSource.DatabaseGuid, databaseGuid, requestContext)
		{
			base.NotFoundExceptionCreator = (() => new DatabaseNotFoundException(this.DatabaseGuid.ToString()));
		}

		public Guid DatabaseGuid
		{
			get
			{
				return (Guid)base.SourceObject;
			}
		}

		protected override AnchorMailboxCacheEntry RefreshCacheEntry()
		{
			return new AnchorMailboxCacheEntry
			{
				Database = new ADObjectId(Guid.Empty, (Guid)base.SourceObject)
			};
		}
	}
}
