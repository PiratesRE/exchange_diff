using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MapiProviderTags
	{
		public const int MapiSession = 0;

		public const int MapiObject = 1;

		public const int PropertyBag = 2;

		public const int MessageStore = 3;

		public const int Folder = 4;

		public const int LogonStatistics = 5;

		public const int Convertor = 6;

		public static Guid guid = new Guid("C9AAFFBB-C5D9-4e08-B398-7733BC04D45E");
	}
}
