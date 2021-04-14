using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class LegacyResourceHealthMonitorKey : ResourceKey
	{
		public LegacyResourceHealthMonitorKey(Guid databaseGuid) : base(ResourceMetricType.Remote, null)
		{
			if (databaseGuid == Guid.Empty)
			{
				throw new ArgumentException("Guid.Empty is not a valid MdbGuid value", "mdbGuid");
			}
			this.cachedHashCode = ("LegacyMdblCi({0})".GetHashCode() ^ databaseGuid.GetHashCode());
			this.DatabaseGuid = databaseGuid;
		}

		private bool InitializeIdentity()
		{
			DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(this.DatabaseGuid, null, null, FindServerFlags.AllowMissing);
			if (databaseInformation.IsMissing)
			{
				this.databaseName = string.Format("LegacyMdblCiUnresolved({0}{1})", this.DatabaseGuid.ToString(), databaseInformation.ForestFqdn);
			}
			else
			{
				this.databaseName = string.Format("LegacyMdblCi({0})", databaseInformation.DatabaseName);
			}
			return !databaseInformation.IsMissing;
		}

		protected internal override CacheableResourceHealthMonitor CreateMonitor()
		{
			return new LegacyResourceHealthMonitor(this);
		}

		public Guid DatabaseGuid { get; private set; }

		public override string ToString()
		{
			if (this.databaseName == null)
			{
				this.InitializeIdentity();
			}
			return this.databaseName;
		}

		public override bool Equals(object obj)
		{
			LegacyResourceHealthMonitorKey legacyResourceHealthMonitorKey = obj as LegacyResourceHealthMonitorKey;
			return legacyResourceHealthMonitorKey != null && legacyResourceHealthMonitorKey.DatabaseGuid == this.DatabaseGuid;
		}

		public override int GetHashCode()
		{
			return this.cachedHashCode;
		}

		private const string FormatString = "LegacyMdblCi({0})";

		private const string FormatStringUnresolved = "LegacyMdblCiUnresolved({0}{1})";

		private readonly int cachedHashCode;

		private string databaseName;
	}
}
