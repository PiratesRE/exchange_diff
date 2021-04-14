using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ServerValidationResult : IHealthValidationResultMinimal
	{
		public ServerValidationResult(string serverName, Guid serverGuid)
		{
			this.m_serverName = serverName;
			this.m_serverGuid = serverGuid;
		}

		public Guid IdentityGuid
		{
			get
			{
				return this.m_serverGuid;
			}
		}

		public string Identity
		{
			get
			{
				return this.m_serverName;
			}
		}

		public int HealthyCopiesCount { get; set; }

		public int HealthyPassiveCopiesCount { get; set; }

		public int TotalPassiveCopiesCount { get; set; }

		public bool IsValidationSuccessful { get; set; }

		public bool IsSiteValidationSuccessful { get; set; }

		public bool IsAnyCachedCopyStatusStale { get; set; }

		public string ErrorMessage { get; set; }

		public string ErrorMessageWithoutFullStatus { get; set; }

		private readonly string m_serverName;

		private readonly Guid m_serverGuid;
	}
}
