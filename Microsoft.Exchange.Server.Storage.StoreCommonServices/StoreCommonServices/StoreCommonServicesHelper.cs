using System;
using System.Reflection;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class StoreCommonServicesHelper
	{
		public static Version GetAssemblyVersion()
		{
			return Assembly.GetExecutingAssembly().GetName().Version;
		}

		public static string GetAssemblyName()
		{
			return Assembly.GetExecutingAssembly().GetName().Name;
		}
	}
}
