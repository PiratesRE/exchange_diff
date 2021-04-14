using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MonadIntegrationTags
	{
		public const int Default = 0;

		public const int Integration = 1;

		public const int Verbose = 2;

		public const int Data = 3;

		public const int Host = 4;

		public static Guid guid = new Guid("b47bd400-78af-479f-aeff-39d4d6c54559");
	}
}
