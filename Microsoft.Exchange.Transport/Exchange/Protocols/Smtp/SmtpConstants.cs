using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class SmtpConstants
	{
		public const int DefaultMaxConnectionRatePerMinute = 1200;

		public const int MaxCommandLineSize = 4000;

		public const int MinTLSCipherStrength = 128;

		public const int BreadcrumbsLength = 64;

		public const int MaxCommandLength = 32768;

		public const string AnonymousName = "anonymous";

		public const string PartnerName = "partner";

		public static readonly TimeSpan EventTimeout = TimeSpan.FromMinutes(10.0);

		public static readonly SecurityIdentifier AnonymousSecurityIdentifier = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null);
	}
}
