using System;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DatabaseLocationProvider : IDatabaseLocationProvider
	{
		public DatabaseLocationProvider() : this(ActiveManager.GetCachingActiveManagerInstance(), NullPerformanceDataLogger.Instance)
		{
		}

		public DatabaseLocationProvider(ActiveManager activeManager, IPerformanceDataLogger performanceDataLogger)
		{
			this.activeManager = activeManager;
			this.performanceDataLogger = performanceDataLogger;
		}

		public DatabaseLocationInfo GetLocationInfo(Guid mdbGuid, bool bypassCache, bool ignoreSiteBoundary)
		{
			GetServerForDatabaseFlags getServerForDatabaseFlags = GetServerForDatabaseFlags.None;
			if (ignoreSiteBoundary)
			{
				getServerForDatabaseFlags |= GetServerForDatabaseFlags.IgnoreAdSiteBoundary;
			}
			if (bypassCache)
			{
				getServerForDatabaseFlags |= GetServerForDatabaseFlags.ReadThrough;
			}
			DatabaseLocationInfo serverForDatabase;
			try
			{
				serverForDatabase = this.activeManager.GetServerForDatabase(mdbGuid, getServerForDatabaseFlags, this.performanceDataLogger);
			}
			catch (DatabaseNotFoundException innerException)
			{
				throw new MailboxInfoStaleException(string.Format("Mailbox database guid: {0}", mdbGuid), innerException);
			}
			if (serverForDatabase == null || serverForDatabase.RequestResult == DatabaseLocationInfoResult.Unknown)
			{
				throw new DatabaseLocationUnavailableException(ServerStrings.DatabaseLocationNotAvailable(mdbGuid));
			}
			ExTraceGlobals.SessionTracer.TraceDebug<GetServerForDatabaseFlags, DatabaseLocationInfoResult, string>((long)this.GetHashCode(), "DatabaseLocationProvider::GetLocationInfo. flags {0}, result {1}, {2}", getServerForDatabaseFlags, serverForDatabase.RequestResult, serverForDatabase.ServerFqdn);
			return serverForDatabase;
		}

		private readonly ActiveManager activeManager;

		private readonly IPerformanceDataLogger performanceDataLogger;
	}
}
