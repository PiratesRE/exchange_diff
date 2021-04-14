using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MobileDriverTags
	{
		public const int Xso = 0;

		public const int Core = 1;

		public const int Transport = 2;

		public const int Session = 3;

		public const int Service = 4;

		public const int Applicationlogic = 5;

		public static Guid guid = new Guid("344A3E26-44B9-45b3-B5EC-623311EAA0AA");
	}
}
