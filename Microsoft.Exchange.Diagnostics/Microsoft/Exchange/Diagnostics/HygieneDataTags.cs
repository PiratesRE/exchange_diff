using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct HygieneDataTags
	{
		public const int WebstoreProvider = 0;

		public const int FaultInjection = 1;

		public const int DomainSession = 2;

		public const int GLSQuery = 3;

		public const int WebServiceProvider = 4;

		public static Guid guid = new Guid("4B65DA35-2EAC-4452-B7B7-375D986BCA91");
	}
}
