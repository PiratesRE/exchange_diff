using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct SecurityTags
	{
		public const int Authentication = 0;

		public const int PartnerToken = 1;

		public const int X509CertAuth = 2;

		public const int OAuth = 3;

		public const int FaultInjection = 4;

		public const int BackendRehydration = 5;

		public static Guid guid = new Guid("5ce0dc7e-6229-4bd9-9464-c92d7813bc3b");
	}
}
