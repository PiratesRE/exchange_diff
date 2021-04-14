using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct OrgIdAuthTags
	{
		public const int OrgIdAuthentication = 0;

		public const int OrgIdBasicAuth = 1;

		public const int OrgIdConfiguration = 2;

		public const int OrgIdUserValidation = 3;

		public static Guid guid = new Guid("BD7A7CA1-6659-4EB0-A477-8F89F9A7D983");
	}
}
