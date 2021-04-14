using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public static class Globals
	{
		public static IDirectory Directory
		{
			get
			{
				return Globals.directory;
			}
		}

		public static bool IsInitialized
		{
			get
			{
				return Globals.directory != null;
			}
		}

		private static Globals.DirectoryCreatorDelegate DirectoryFactory
		{
			get
			{
				return Globals.directoryFactory.Value;
			}
		}

		public static void Initialize(ICachePerformanceCounters mailboxCacheCounters, ICachePerformanceCounters foreignMailboxCacheCounters, ICachePerformanceCounters addressCacheCounters, ICachePerformanceCounters foreignAddressCacheCounters, ICachePerformanceCounters databaseCacheCounters, ICachePerformanceCounters orgConatinerCacheCounters, ICachePerformanceCounters distributionListMembershipCacheCountes)
		{
			ADExecutionTracker.Initialize();
			Globals.directory = Globals.DirectoryFactory(mailboxCacheCounters, foreignMailboxCacheCounters, addressCacheCounters, foreignAddressCacheCounters, databaseCacheCounters, orgConatinerCacheCounters, distributionListMembershipCacheCountes);
		}

		public static void Terminate()
		{
			Globals.directory = null;
		}

		public static uint GetCurrentServerCPUUsagePercentage()
		{
			return CpuUsage.GetCurrentUsagePercentage();
		}

		internal static IDisposable SetTestHook(Globals.DirectoryCreatorDelegate testHook)
		{
			return Globals.directoryFactory.SetTestHook(testHook);
		}

		private const string ErrorDirectoryServicesLayerIsNotInitialized = "The DirectoryServices layer has not yet been initialized.";

		private static IDirectory directory;

		private static Hookable<Globals.DirectoryCreatorDelegate> directoryFactory = Hookable<Globals.DirectoryCreatorDelegate>.Create(true, new Globals.DirectoryCreatorDelegate(Microsoft.Exchange.Server.Storage.DirectoryServices.Directory.Create));

		internal delegate IDirectory DirectoryCreatorDelegate(ICachePerformanceCounters mailboxCacheCounters, ICachePerformanceCounters foreignMailboxCacheCounters, ICachePerformanceCounters addressCacheCounters, ICachePerformanceCounters foreignAddressCacheCounters, ICachePerformanceCounters databaseCacheCounters, ICachePerformanceCounters orgContainerCacheCounters, ICachePerformanceCounters distributionListMembershipCacheCountes);
	}
}
