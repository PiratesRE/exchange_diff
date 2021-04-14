using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MSExchangeManagementWebServiceTags
	{
		public const int MessageInspector = 0;

		public const int KnownException = 1;

		public const int PowerShell = 2;

		public const int RbacAuthorization = 3;

		public static Guid guid = new Guid("0df9c122-5f11-416d-9ed1-7b6dd48beb8e");
	}
}
