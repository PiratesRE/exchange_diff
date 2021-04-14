using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage.ResourceHealth;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class BaseServerIdInfo
	{
		protected BaseServerIdInfo(string serverFQDN, Guid mdbGuid, int serverVersion, string cafeFQDN, bool isFromDifferentResourceForest)
		{
			this.CafeFQDN = cafeFQDN;
			this.ServerFQDN = serverFQDN;
			this.MdbGuid = mdbGuid;
			this.serverVersion = serverVersion;
			this.IsFromDifferentResourceForest = isFromDifferentResourceForest;
		}

		public bool IsLocalServer
		{
			get
			{
				return !this.IsFromDifferentResourceForest && string.Equals(BaseServerIdInfo.LocalServerFQDN, this.ServerFQDN, StringComparison.OrdinalIgnoreCase);
			}
		}

		public bool IsFromDifferentResourceForest { get; private set; }

		public int ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		public string ServerFQDN { get; private set; }

		public string CafeFQDN { get; private set; }

		public Guid MdbGuid { get; private set; }

		public virtual ResourceKey[] ToResourceKey(bool writeOperation)
		{
			if (!writeOperation)
			{
				return new ResourceKey[]
				{
					new MdbResourceHealthMonitorKey(this.MdbGuid)
				};
			}
			if (CallContext.Current.IsLongRunningScenario)
			{
				return new ResourceKey[]
				{
					new MdbResourceHealthMonitorKey(this.MdbGuid),
					new MdbReplicationResourceHealthMonitorKey(this.MdbGuid),
					new CiAgeOfLastNotificationResourceKey(this.MdbGuid)
				};
			}
			return new ResourceKey[]
			{
				new MdbResourceHealthMonitorKey(this.MdbGuid),
				new MdbReplicationResourceHealthMonitorKey(this.MdbGuid)
			};
		}

		private int serverVersion = BaseServerIdInfo.InvalidServerVersion;

		internal static readonly int InvalidServerVersion = 0;

		internal static readonly string LocalServerFQDN = NativeHelpers.GetLocalComputerFqdn(true);
	}
}
