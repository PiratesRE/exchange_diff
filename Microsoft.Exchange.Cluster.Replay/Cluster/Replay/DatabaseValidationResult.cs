using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseValidationResult : IHealthValidationResult, IHealthValidationResultMinimal
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		public DatabaseValidationResult(string databaseName, Guid databaseGuid, AmServerName targetServer, int numHealthyCopiesMin) : this(databaseName, databaseGuid, targetServer, numHealthyCopiesMin, 0)
		{
		}

		public DatabaseValidationResult(string databaseName, Guid databaseGuid, AmServerName targetServer, int numHealthyCopiesMin, int numHealthyPassiveCopiesMin)
		{
			this.m_databaseName = databaseName;
			this.m_databaseGuid = databaseGuid;
			this.m_targetServerName = targetServer;
			this.m_numHealthyCopiesMin = numHealthyCopiesMin;
			this.m_numHealthyPassiveCopiesMin = numHealthyPassiveCopiesMin;
		}

		public Guid IdentityGuid
		{
			get
			{
				return this.m_databaseGuid;
			}
		}

		public string Identity
		{
			get
			{
				return this.m_databaseName;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.m_databaseGuid;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.m_databaseName;
			}
		}

		public int MinimumNumHealthyCopies
		{
			get
			{
				return this.m_numHealthyCopiesMin;
			}
		}

		public int MinimumNumHealthyPassiveCopies
		{
			get
			{
				return this.m_numHealthyPassiveCopiesMin;
			}
		}

		public bool IsSiteValidationSuccessful
		{
			get
			{
				if (this.m_isSiteValidationSuccessful != null)
				{
					return this.m_isSiteValidationSuccessful.Value;
				}
				this.m_isSiteValidationSuccessful = new bool?(this.IsSiteValidationSuccessfulImpl());
				return this.m_isSiteValidationSuccessful.Value;
			}
		}

		public bool IsTargetCopyHealthy { get; private set; }

		public bool IsAnyCachedCopyStatusStale { get; set; }

		public bool IsActiveCopyHealthy { get; private set; }

		public int HealthyCopiesCount { get; private set; }

		public int HealthyPassiveCopiesCount { get; private set; }

		public int TotalPassiveCopiesCount { get; private set; }

		public bool IsValidationSuccessful { get; private set; }

		public CopyStatusClientCachedEntry TargetCopyStatus { get; set; }

		public CopyStatusClientCachedEntry ActiveCopyStatus { get; set; }

		public string ErrorMessage { get; set; }

		public string ErrorMessageWithoutFullStatus { get; set; }

		public void ReportHealthyCopy(AmServerName specificServer, string adSiteName)
		{
			DatabaseValidationResult.Tracer.TraceDebug<AmServerName>((long)this.GetHashCode(), "ReportHealthy: {0}", specificServer);
			this.HealthyCopiesCount++;
			if (specificServer.Equals(this.m_targetServerName))
			{
				this.IsTargetCopyHealthy = true;
			}
			if (this.ActiveCopyStatus != null && specificServer.Equals(this.ActiveCopyStatus.ActiveServer))
			{
				this.IsActiveCopyHealthy = true;
			}
			else
			{
				this.HealthyPassiveCopiesCount++;
				this.TotalPassiveCopiesCount++;
			}
			if (this.HealthyCopiesCount < this.m_numHealthyCopiesMin || this.HealthyPassiveCopiesCount < this.m_numHealthyPassiveCopiesMin)
			{
				this.IsValidationSuccessful = false;
			}
			else
			{
				this.IsValidationSuccessful = true;
			}
			if (!string.IsNullOrEmpty(adSiteName))
			{
				this.IncrementHealthyCountInSite(adSiteName, true);
			}
		}

		public void ReportFailedCopy(AmServerName specificServer, string adSiteName)
		{
			DatabaseValidationResult.Tracer.TraceDebug<AmServerName>((long)this.GetHashCode(), "ReportFailed: {0}", specificServer);
			if (this.ActiveCopyStatus == null || !specificServer.Equals(this.ActiveCopyStatus.ActiveServer))
			{
				this.TotalPassiveCopiesCount++;
			}
			if (!string.IsNullOrEmpty(adSiteName))
			{
				this.IncrementHealthyCountInSite(adSiteName, false);
			}
		}

		private void IncrementHealthyCountInSite(string adSiteName, bool isCopyHealthy)
		{
			if (this.m_healthyCountPerSite == null)
			{
				this.m_healthyCountPerSite = new Dictionary<string, int>(5);
			}
			int num = isCopyHealthy ? 1 : 0;
			int num2;
			this.m_healthyCountPerSite.TryGetValue(adSiteName, out num2);
			this.m_healthyCountPerSite[adSiteName] = num2 + num;
		}

		private bool IsSiteValidationSuccessfulImpl()
		{
			if (this.m_healthyCountPerSite == null || this.m_healthyCountPerSite.Count == 0)
			{
				return false;
			}
			int num = 0;
			foreach (KeyValuePair<string, int> keyValuePair in this.m_healthyCountPerSite)
			{
				if (keyValuePair.Value >= 1)
				{
					num++;
				}
				if (num >= 2)
				{
					break;
				}
			}
			return num >= 2;
		}

		private readonly int m_numHealthyCopiesMin;

		private readonly int m_numHealthyPassiveCopiesMin;

		private readonly string m_databaseName;

		private readonly Guid m_databaseGuid;

		private readonly AmServerName m_targetServerName;

		private Dictionary<string, int> m_healthyCountPerSite;

		private bool? m_isSiteValidationSuccessful;
	}
}
