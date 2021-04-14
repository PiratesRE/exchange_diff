using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ObjectModelTags
	{
		public const int DataSourceInfo = 0;

		public const int DataSourceManager = 1;

		public const int DataSourceSession = 2;

		public const int Field = 3;

		public const int ConfigObject = 4;

		public const int PropertyBag = 5;

		public const int QueryParser = 6;

		public const int SchemaManager = 7;

		public const int SecurityManger = 8;

		public const int ConfigObjectReader = 9;

		public const int Identity = 10;

		public const int RoleBasedStringMapping = 11;

		public static Guid guid = new Guid("b643f45b-9d4a-4186-ba92-05d5c229d692");
	}
}
