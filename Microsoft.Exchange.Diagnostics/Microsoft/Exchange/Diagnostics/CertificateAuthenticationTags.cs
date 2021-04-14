using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct CertificateAuthenticationTags
	{
		public const int CertAuth = 0;

		public const int FaultInjection = 1;

		public static Guid guid = new Guid("39942ef4-b83c-426d-b492-ba040437091a");
	}
}
