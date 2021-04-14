using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	public static class Globals
	{
		public const byte MaxDatabasesMountedStandard = 5;

		public const byte MaxRecoveryDatabasesMounted = 1;

		public const byte MaxRecoveryDatabasesMountedStandardSKU = 1;

		public static readonly byte MaxDatabasesMountedEnterprise = DefaultSettings.Get.MaximumMountedDatabases;
	}
}
