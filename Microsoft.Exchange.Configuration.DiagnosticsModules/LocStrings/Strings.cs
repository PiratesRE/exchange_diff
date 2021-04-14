using System;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.Configuration.DiagnosticsModules.LocStrings
{
	internal static class Strings
	{
		public static string UnhandledException(string error)
		{
			return string.Format(Strings.ResourceManager.GetString("UnhandledException"), error);
		}

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.Configuration.DiagnosticsModules.LocStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			UnhandledException
		}
	}
}
