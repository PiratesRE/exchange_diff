using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class DatabaseBadVersion : StoreException
	{
		public DatabaseBadVersion(LID lid, Guid mdbGuid, ComponentVersion expectedVersion, ComponentVersion foundVersion) : base(lid, ErrorCodeValue.DatabaseBadVersion, string.Format("Database with Guid {0} expected version {1} and found version {2}", mdbGuid, expectedVersion, foundVersion))
		{
		}

		public DatabaseBadVersion(LID lid, Guid mdbGuid, ComponentVersion expectedVersion, ComponentVersion foundVersion, Exception innerException) : base(lid, ErrorCodeValue.DatabaseBadVersion, string.Format("Database with Guid {0} expected version {1} and found version {2}", mdbGuid, expectedVersion, foundVersion), innerException)
		{
		}

		private const string DatabaseBadVersionMessage = "Database with Guid {0} expected version {1} and found version {2}";
	}
}
