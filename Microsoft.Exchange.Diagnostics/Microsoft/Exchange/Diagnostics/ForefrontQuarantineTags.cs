using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ForefrontQuarantineTags
	{
		public const int Agent = 0;

		public const int Store = 1;

		public const int Manager = 2;

		public const int Cleanup = 3;

		public const int SpamDigestWS = 4;

		public const int SpamDigestCommon = 5;

		public const int SpamDigestGenerator = 6;

		public const int SpamDigestBackgroundJob = 7;

		public const int Common = 8;

		public static Guid guid = new Guid("10B884FD-372F-490D-A233-7C2C4CB8F104");
	}
}
