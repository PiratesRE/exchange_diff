using System;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal interface IDirectoryCacheProvider
	{
		TObject Get<TObject>(DirectoryCacheRequest cacheRequest) where TObject : ADRawEntry, new();

		void Put(AddDirectoryCacheRequest cacheRequest);

		void Remove(RemoveDirectoryCacheRequest cacheRequest);

		void TestOnlyResetAllCaches();

		ADCacheResultState ResultState { get; }

		bool IsNewProxyObject { get; }

		int RetryCount { get; }
	}
}
