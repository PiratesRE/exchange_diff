using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ActiveManagerPerformanceMarkers
	{
		public const string GetServerForDatabaseGetServerNameForDatabase = "GetServerForDatabaseGetServerNameForDatabase";

		public const string GetServerForDatabaseGetServerInformationForDatabase = "GetServerForDatabaseGetServerInformationForDatabase";

		public const string GetServerNameForDatabaseGetDatabaseByGuidEx = "GetServerNameForDatabaseGetDatabaseByGuidEx";

		public const string GetServerNameForDatabaseLookupDatabaseAndPossiblyPopulateCache = "GetServerNameForDatabaseLookupDatabaseAndPossiblyPopulateCache";

		public const string GetServerInformationForDatabaseGetDatabaseByGuidEx = "GetServerInformationForDatabaseGetDatabaseByGuidEx";

		public const string ActiveDirectoryQueryLatency = "ADQuery";
	}
}
