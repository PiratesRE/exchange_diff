using System;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal static class Constants
	{
		internal const string DirectoryCacheEnableKey = "Directory Cache Enable";

		internal const string ExcludedDirectoryCacheProcessesKey = "Excluded Directory Cache Processes";

		internal const int CheckCacheKeyEveryNMilliseconds = 300000;

		internal const int CheckCacheKeyEveryNMillisecondsInTest = 15000;

		internal const int DefaultObjectTypeTimeout = 2147483646;

		internal const int VolatileObjectsCacheTimeInNSeconds = 30;

		internal const int DefaultNewObjectCacheTimeoutInMinutes = 15;

		internal const int DefaultNewObjectInclusionThresholdInMinutes = 15;

		internal const string DirectoryCacheNamedPipeURI = "net.pipe://localhost/DirectoryCache/service.svc";

		internal const int WCFTimeoutInSeconds = 15;

		internal const int MaxMessageRecievedSize = 10485760;

		internal const int MaxBufferSize = 10485760;
	}
}
