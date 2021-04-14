using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Serializable]
	internal sealed class LeaseFileEntry : ISerializable
	{
		public LeaseFileEntry(string serverName, string serviceInstanceId, int ridMasterVersion, DateTime lockExpirationTime, Guid siteGuid)
		{
			this.ServerName = serverName;
			this.ServiceInstanceId = serviceInstanceId;
			this.RidMasterVersion = ridMasterVersion;
			this.LockExpirationTimeUtc = lockExpirationTime;
			this.SiteGuid = siteGuid;
		}

		public LeaseFileEntry(SerializationInfo info, StreamingContext context)
		{
			this.ServerName = info.GetString("ServerName");
			this.ServiceInstanceId = info.GetString("ServiceInstanceId");
			this.RidMasterVersion = info.GetInt32("RidMasterVersion");
			this.LockExpirationTimeUtc = DateTime.FromFileTimeUtc(long.Parse(info.GetString("LockExpirationTimeUtc")));
			this.SiteGuid = new Guid(info.GetString("SiteGuid"));
		}

		public string ServerName { get; private set; }

		public string ServiceInstanceId { get; private set; }

		public Guid SiteGuid { get; private set; }

		public int RidMasterVersion { get; private set; }

		public DateTime LockExpirationTimeUtc { get; private set; }

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ServerName", this.ServerName);
			info.AddValue("ServiceInstanceId", this.ServiceInstanceId);
			info.AddValue("RidMasterVersion", this.RidMasterVersion);
			info.AddValue("LockExpirationTimeUtc", this.LockExpirationTimeUtc.ToFileTimeUtc());
			info.AddValue("SiteGuid", this.SiteGuid.ToString());
		}

		public override string ToString()
		{
			return string.Format("ServerName: {0}, ServiceInstanceId: {1}, RidMasterVersion: {2}, LockExpirationTimeUtc: {3}, SiteGuid: {4}", new object[]
			{
				this.ServerName,
				this.ServiceInstanceId,
				this.RidMasterVersion,
				this.LockExpirationTimeUtc,
				this.SiteGuid.ToString()
			});
		}

		private const string ServerNameElement = "ServerName";

		private const string ServiceInstanceIdElement = "ServiceInstanceId";

		private const string SiteGuidElement = "SiteGuid";

		private const string RidMasterVersionElement = "RidMasterVersion";

		private const string LockExpirationTimeUtcElement = "LockExpirationTimeUtc";
	}
}
