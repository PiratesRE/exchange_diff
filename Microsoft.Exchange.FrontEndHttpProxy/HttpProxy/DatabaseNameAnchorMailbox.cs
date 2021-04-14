using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.HttpProxy
{
	internal class DatabaseNameAnchorMailbox : DatabaseBasedAnchorMailbox
	{
		public DatabaseNameAnchorMailbox(string databaseName, IRequestContext requestContext) : base(AnchorSource.DatabaseName, databaseName, requestContext)
		{
			base.NotFoundExceptionCreator = (() => new DatabaseNotFoundException(this.DatabaseName));
		}

		public string DatabaseName
		{
			get
			{
				return (string)base.SourceObject;
			}
		}

		protected override AnchorMailboxCacheEntry RefreshCacheEntry()
		{
			IConfigurationSession session = DirectoryHelper.GetConfigurationSession();
			MailboxDatabase[] array = DirectoryHelper.InvokeResourceForest(base.RequestContext.LatencyTracker, () => session.Find<MailboxDatabase>(session.GetExchangeConfigurationContainer().Id, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, DatabaseSchema.Name, this.DatabaseName), null, 1));
			if (array.Length == 0)
			{
				base.CheckForNullAndThrowIfApplicable<ADObjectId>(null);
				return new AnchorMailboxCacheEntry();
			}
			return new AnchorMailboxCacheEntry
			{
				Database = array[0].Id
			};
		}

		protected override AnchorMailboxCacheEntry LoadCacheEntryFromIncomingCookie()
		{
			BackEndDatabaseCookieEntry backEndDatabaseCookieEntry = base.IncomingCookieEntry as BackEndDatabaseCookieEntry;
			if (backEndDatabaseCookieEntry != null)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<DatabaseNameAnchorMailbox, BackEndDatabaseCookieEntry>((long)this.GetHashCode(), "[DatabaseNameAnchorMailbox::LoadCacheEntryFromCookie]: Anchor mailbox {0} using cookie entry {1} as cache entry.", this, backEndDatabaseCookieEntry);
				BackEndDatabaseResourceForestCookieEntry backEndDatabaseResourceForestCookieEntry = base.IncomingCookieEntry as BackEndDatabaseResourceForestCookieEntry;
				return new AnchorMailboxCacheEntry
				{
					Database = new ADObjectId(backEndDatabaseCookieEntry.Database, (backEndDatabaseResourceForestCookieEntry == null) ? null : backEndDatabaseResourceForestCookieEntry.ResourceForest)
				};
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<DatabaseNameAnchorMailbox>((long)this.GetHashCode(), "[DatabaseNameAnchorMailbox::LoadCacheEntryFromCookie]: Anchor mailbox {0} had no BackEndDatabaseCookie.", this);
			return null;
		}
	}
}
