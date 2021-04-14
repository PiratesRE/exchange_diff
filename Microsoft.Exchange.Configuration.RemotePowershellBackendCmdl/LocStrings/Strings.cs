using System;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.Configuration.RemotePowershellBackendCmdletProxy.LocStrings
{
	internal static class Strings
	{
		public static string ErrorWhenParsingCommonAccessToken(string message)
		{
			return string.Format(Strings.ResourceManager.GetString("ErrorWhenParsingCommonAccessToken"), message);
		}

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.Configuration.RemotePowershellBackendCmdletProxy.LocStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			ErrorWhenParsingCommonAccessToken
		}
	}
}
