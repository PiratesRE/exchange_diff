using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class TestProviderResult<TProvider> where TProvider : IPListProvider, new()
	{
		public TProvider Provider;

		public IPAddress[] ProviderResult;

		public bool Matched;
	}
}
