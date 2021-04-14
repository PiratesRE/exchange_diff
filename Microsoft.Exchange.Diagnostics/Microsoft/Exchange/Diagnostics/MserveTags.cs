using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MserveTags
	{
		public const int Provider = 0;

		public const int TargetConnection = 1;

		public const int Config = 2;

		public const int DeltaSyncAPI = 3;

		public const int MserveCacheService = 5;

		public static Guid guid = new Guid("86790e72-3e66-4b27-b3e1-66faaa21840f");
	}
}
