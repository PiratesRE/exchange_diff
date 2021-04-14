using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct SchemaConverterTags
	{
		public const int SchemaState = 0;

		public const int Common = 1;

		public const int Xso = 2;

		public const int AirSync = 3;

		public const int Protocol = 4;

		public const int Conversion = 5;

		public const int MethodEnterExit = 6;

		public static Guid guid = new Guid("{7569BC27-E1CA-11D9-88B7-000D9DFFC66E}");
	}
}
