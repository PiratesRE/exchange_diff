using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MdbResourceKey : ResourceKey
	{
		public MdbResourceKey(ResourceMetricType metric, Guid databaseGuid) : base(metric, MdbResourceKey.GetId(databaseGuid))
		{
			if (databaseGuid == Guid.Empty)
			{
				throw new ArgumentException("Guid.Empty is not a valid MdbGuid value", "mdbGuid");
			}
			this.DatabaseGuid = databaseGuid;
		}

		public Guid DatabaseGuid { get; private set; }

		private static string GetId(Guid databaseGuid)
		{
			if (databaseGuid == Guid.Empty)
			{
				throw new ArgumentException("Guid.Empty is not a valid MdbGuid value", "mdbGuid");
			}
			IDatabaseInformation databaseInformation = DatabaseInformationCache.Singleton.Get(databaseGuid);
			if (databaseInformation == null)
			{
				return databaseGuid.ToString();
			}
			return databaseInformation.DatabaseName;
		}
	}
}
