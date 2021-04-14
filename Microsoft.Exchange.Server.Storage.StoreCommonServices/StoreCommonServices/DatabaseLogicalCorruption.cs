using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class DatabaseLogicalCorruption : StoreException
	{
		public DatabaseLogicalCorruption(LID lid, Guid mdbGuid) : base(lid, ErrorCodeValue.CorruptStore, string.Format("Database with Guid {0} has logical corruption", mdbGuid))
		{
		}

		public DatabaseLogicalCorruption(LID lid, Guid mdbGuid, Exception innerException) : base(lid, ErrorCodeValue.CorruptStore, string.Format("Database with Guid {0} has logical corruption", mdbGuid), innerException)
		{
		}

		private const string DatabaseLogicalCorruptionMessage = "Database with Guid {0} has logical corruption";
	}
}
