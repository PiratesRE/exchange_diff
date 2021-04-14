using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.Hygiene.Data.DataProvider
{
	internal abstract class BloomFilterProviderFactory
	{
		internal static BloomFilterProviderFactory Default
		{
			get
			{
				if (BloomFilterProviderFactory.defaultInstance == null)
				{
					Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.CacheDataProvider");
					Type type = assembly.GetType("Microsoft.Exchange.Hygiene.Cache.DataProvider.AutoUpdatingBloomFilterDataProvider+Factory");
					BloomFilterProviderFactory.defaultInstance = (BloomFilterProviderFactory)Activator.CreateInstance(type);
				}
				return BloomFilterProviderFactory.defaultInstance;
			}
			set
			{
				BloomFilterProviderFactory.defaultInstance = value;
			}
		}

		internal abstract IBloomFilterDataProvider Create(IEnumerable<Type> supportedTypes, TimeSpan autoUpdateFrequency, bool tracerTokenCheckEnabled);

		private static BloomFilterProviderFactory defaultInstance;
	}
}
