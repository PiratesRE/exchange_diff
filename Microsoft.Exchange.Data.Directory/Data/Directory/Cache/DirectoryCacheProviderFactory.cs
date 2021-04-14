using System;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal sealed class DirectoryCacheProviderFactory
	{
		private DirectoryCacheProviderFactory()
		{
		}

		internal static DirectoryCacheProviderFactory Default
		{
			get
			{
				return DirectoryCacheProviderFactory.instance;
			}
		}

		internal IDirectoryCacheProvider CreateNewDirectoryCacheProvider()
		{
			return new ExchangeDirectoryCacheProvider();
		}

		private static readonly DirectoryCacheProviderFactory instance = new DirectoryCacheProviderFactory();
	}
}
