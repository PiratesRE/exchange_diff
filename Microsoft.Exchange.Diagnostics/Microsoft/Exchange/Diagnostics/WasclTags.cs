using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct WasclTags
	{
		public const int General = 0;

		public const int Core = 1;

		public const int Verdict = 2;

		public const int ExternalCall = 3;

		public const int API = 4;

		public const int CryptoHelper = 5;

		public const int OSE = 6;

		public static Guid guid = new Guid("48076FB3-30FE-460B-975D-934742F529EA");
	}
}
