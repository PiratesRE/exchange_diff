using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MessengerTags
	{
		public const int Core = 0;

		public const int MSNP = 1;

		public const int ABCH = 2;

		public const int Sharing = 3;

		public const int Storage = 4;

		public static Guid guid = new Guid("5099defc-8a21-405a-ba04-e0857dd8d94e");
	}
}
