using System;

namespace Microsoft.Exchange.AirSync
{
	internal class DiagnosticsHelper
	{
		internal static CallType GetCallType(string argument, out string newArg)
		{
			if (string.IsNullOrEmpty(argument))
			{
				newArg = null;
				return CallType.Metadata;
			}
			argument = argument.Trim();
			if (argument.ToLower() == "dumpcache")
			{
				newArg = null;
				return CallType.DumpCache;
			}
			if (argument.ToLower().StartsWith("emailaddress="))
			{
				newArg = argument.Substring("emailaddress=".Length);
				return CallType.EmailAddress;
			}
			if (argument.ToLower().StartsWith("deviceid="))
			{
				newArg = argument.Substring("deviceid=".Length);
				return CallType.DeviceId;
			}
			throw new ArgumentException("Unknown argument: " + argument);
		}

		internal const string DumpCacheArgument = "dumpcache";

		internal const string EmailAddressArgument = "emailaddress=";

		internal const string DeviceIdArgument = "deviceid=";
	}
}
