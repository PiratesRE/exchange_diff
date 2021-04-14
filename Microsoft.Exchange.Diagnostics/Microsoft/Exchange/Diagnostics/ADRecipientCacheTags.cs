using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ADRecipientCacheTags
	{
		public const int ADLookup = 0;

		public const int CacheLookup = 1;

		public static Guid guid = new Guid("48868D1B-4502-4c8e-8293-E81776D01BCE");
	}
}
