using System;

namespace Microsoft.Exchange.Security.Authentication
{
	internal static class CookieNames
	{
		public static class Common
		{
			public const string BackEndServerCookieName = "X-BackEndCookie";
		}

		public static class Owa
		{
			public const string DFPOWAVdirCookie = "X-DFPOWA-Vdir";
		}

		public static class Ecp
		{
			internal const string TargetServerParameter = "TargetServer";

			internal const string VersionParameter = "ExchClientVer";
		}

		public static class Fba
		{
			internal const string CadataCookie = "cadata";

			internal const string CadataKeyCookie = "cadataKey";

			internal const string CadataIVCookie = "cadataIV";

			internal const string CadataTTLCookie = "cadataTTL";

			internal const string CadataSigCookie = "cadataSig";
		}
	}
}
