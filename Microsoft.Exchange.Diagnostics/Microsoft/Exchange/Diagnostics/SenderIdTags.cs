using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct SenderIdTags
	{
		public const int Validation = 0;

		public const int Parsing = 1;

		public const int MacroExpansion = 2;

		public const int Agent = 3;

		public const int Other = 4;

		public static Guid guid = new Guid("AA6A0F4B-6EC1-472d-84BA-FDCB84F20449");
	}
}
