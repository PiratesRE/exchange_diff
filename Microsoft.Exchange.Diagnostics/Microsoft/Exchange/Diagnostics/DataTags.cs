using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct DataTags
	{
		public const int PropertyBag = 0;

		public const int Validation = 1;

		public const int Serialization = 2;

		public const int ValueConvertor = 3;

		public static Guid guid = new Guid("E7FE6E6D-7B3D-4942-B672-BBFD89AC4DC5");
	}
}
