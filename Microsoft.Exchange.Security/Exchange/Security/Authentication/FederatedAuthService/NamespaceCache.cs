using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class NamespaceCache
	{
		public NamespaceCache(int hrdBuckets, int hrdBucketSize, TimeSpan hrdLifetime, int statSize, TimeSpan statLifetime)
		{
			this.hrdCache = new MruDictionary<string, DomainConfig>(hrdBuckets * hrdBucketSize, StringComparer.OrdinalIgnoreCase, null);
			this.negoConfigCache = new TimeoutCache<string, int>(hrdBuckets, hrdBucketSize, false);
			this.confirmedNamespaceStats = new MruDictionaryCache<string, NamespaceStats>(statSize, (int)statLifetime.TotalMinutes);
			this.unconfirmedStats = new MruDictionaryCache<string, NamespaceStats>(statSize, (int)statLifetime.TotalMinutes);
			this.blacklist = new LinkedList<NamespaceStats>();
			this.configLifetime = hrdLifetime;
		}

		public LiveIdBasicAuthenticationCountersInstance PerfCounters { get; set; }

		public bool TryGetValue(string fqdn, out DomainConfig domainConfig)
		{
			return this.hrdCache.TryGetValue(fqdn.ToLowerInvariant(), out domainConfig);
		}

		public void Insert(string fqdn, DomainConfig domainConfig)
		{
			domainConfig.LastUpdateTime = DateTime.UtcNow;
			this.hrdCache.Add(fqdn.ToLowerInvariant(), domainConfig);
		}

		public bool TryGetNegoSetting(string fqdn, out int negoConfig)
		{
			return this.negoConfigCache.TryGetValue(fqdn.ToLowerInvariant(), out negoConfig);
		}

		public void InsertNegoSetting(string fqdn, int negoConfig)
		{
			this.negoConfigCache.InsertAbsolute(fqdn.ToLowerInvariant(), negoConfig, this.configLifetime, null);
		}

		public int Count
		{
			get
			{
				return this.hrdCache.Count;
			}
		}

		public int CountNegoSettings
		{
			get
			{
				return this.negoConfigCache.Count;
			}
		}

		private bool EvaluateStatistic(DomainConfig namespaceInfo, NamespaceStats stats, AuthServiceStaticConfig config, int value, int upperBound, int lowerBound, ExEventLog.EventTuple limitReachedEvent, ExEventLog.EventTuple blacklistEvent, bool blacklistEnabled, string statName, int traceId)
		{
			if (value * 100 >= upperBound * stats.Count)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)traceId, "Namespace '{0}' has reached upper threshold for {1}.", stats.Fqdn, statName);
				this.TarpitNamespace(config, stats, traceId);
				AuthServiceHelper.EventLogger.LogEvent(namespaceInfo.OrgId, limitReachedEvent, stats.Fqdn, new object[]
				{
					stats.Created,
					upperBound,
					stats.Fqdn,
					namespaceInfo.AuthURL,
					stats.User
				});
				if (blacklistEnabled && this.AddToBlacklist(config, stats, traceId))
				{
					AuthServiceHelper.EventLogger.LogEvent(namespaceInfo.OrgId, blacklistEvent, stats.Fqdn, stats.Fqdn, config.blacklistTime, namespaceInfo.AuthURL, stats.User);
					ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)traceId, "Blacklisted namespace '{0}' due to too many {1}", stats.Fqdn, statName);
				}
				return true;
			}
			if (value * 100 >= lowerBound * stats.Count)
			{
				this.TarpitNamespace(config, stats, traceId);
				ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)traceId, "Namespace '{0}' has reached lower threshold for {1}", stats.Fqdn, statName);
				AuthServiceHelper.EventLogger.LogEvent(namespaceInfo.OrgId, limitReachedEvent, stats.Fqdn, new object[]
				{
					stats.Created,
					lowerBound,
					stats.Fqdn,
					namespaceInfo.AuthURL,
					stats.User
				});
				return true;
			}
			return false;
		}

		public void EvaluateStatistics(DomainConfig namespaceInfo, NamespaceStats stats, int traceId)
		{
			AuthServiceStaticConfig config = AuthServiceStaticConfig.Config;
			if (stats == null || namespaceInfo == null)
			{
				return;
			}
			string fqdn = stats.Fqdn;
			if (stats.IsBlacklisted)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "Not evaluating statistics for namespace '{0}'. Already blacklisted", fqdn);
				return;
			}
			if (stats.Count < config.blacklistThreshold || config.blacklistThreshold == 0)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "Not enough statistics gathered for namespace '{0}' to result in blacklist detection.", fqdn);
				return;
			}
			bool flag = !namespaceInfo.OrgId.Equals(OrganizationId.ForestWideOrgId);
			this.EvaluateStatistic(namespaceInfo, stats, config, stats.TimedOut, config.timedOutResponseUpper, config.timedOutResponseLower, flag ? SecurityEventLogConstants.Tuple_OrgWarningManyTimedOutRequests_TenantAlert : SecurityEventLogConstants.Tuple_OrgWarningManyTimedOutRequests_Forensic, flag ? SecurityEventLogConstants.Tuple_OrgBlacklistedTooManyTimedOutRequests_TenantAlert : SecurityEventLogConstants.Tuple_OrgBlacklistedTooManyTimedOutRequests_Forensic, config.enableTimedOutBlacklist, "timed out responses", traceId);
			this.EvaluateStatistic(namespaceInfo, stats, config, stats.BadPassword, config.badUserPasswordUpper, config.badUserPasswordLower, flag ? SecurityEventLogConstants.Tuple_OrgWarningManyBadCreds_TenantAlert : SecurityEventLogConstants.Tuple_OrgWarningManyBadCreds_Forensic, flag ? SecurityEventLogConstants.Tuple_OrgBlacklistedTooManyBadCreds_TenantAlert : SecurityEventLogConstants.Tuple_OrgBlacklistedTooManyBadCreds_Forensic, config.enableBadUserPasswordBlacklist, "bad user creds", traceId);
			this.EvaluateStatistic(namespaceInfo, stats, config, stats.TokenSize, config.tokenSizeExceededUpper, config.tokenSizeExceededLower, flag ? SecurityEventLogConstants.Tuple_OrgWarningTokensTooLarge_TenantAlert : SecurityEventLogConstants.Tuple_OrgWarningTokensTooLarge_Forensic, flag ? SecurityEventLogConstants.Tuple_OrgBlacklistedTokensTooLarge_TenantAlert : SecurityEventLogConstants.Tuple_OrgBlacklistedTokensTooLarge_Forensic, config.enableTokenSizeExceededBlacklist, "tokens exceeding size limit", traceId);
			this.EvaluateStatistic(namespaceInfo, stats, config, stats.Failed, config.failedResponseUpper, config.failedResponseLower, flag ? SecurityEventLogConstants.Tuple_OrgWarningManyFailedResponses_TenantAlert : SecurityEventLogConstants.Tuple_OrgWarningManyFailedResponses_Forensic, flag ? SecurityEventLogConstants.Tuple_OrgBlacklistedTooManyFailedResponses_TenantAlert : SecurityEventLogConstants.Tuple_OrgBlacklistedTooManyFailedResponses_Forensic, config.enableFailedResponseBlacklist, "failed responses", traceId);
		}

		public void UpdatePerfCounters(AuthServiceStaticConfig config)
		{
			this.PerfCounters.NamespaceWhitelistSize.RawValue = (long)config.trustedNamespaces.Count;
			int num = 0;
			lock (this.blacklist)
			{
				LinkedListNode<NamespaceStats> linkedListNode = this.blacklist.First;
				while (linkedListNode != null)
				{
					LinkedListNode<NamespaceStats> linkedListNode2 = linkedListNode;
					linkedListNode = linkedListNode.Next;
					if (linkedListNode2.Value.IsBlacklisted)
					{
						num++;
					}
					else
					{
						this.blacklist.Remove(linkedListNode2);
					}
				}
			}
			this.PerfCounters.NamespaceBlacklistSize.RawValue = (long)num;
		}

		public NamespaceStats GetStatistics(string fqdn, int traceId)
		{
			AuthServiceStaticConfig config = AuthServiceStaticConfig.Config;
			NamespaceStats namespaceStats = this.FindStatistics(fqdn, traceId);
			if (namespaceStats.IsBlacklisted || namespaceStats.IsTarpitted)
			{
				return namespaceStats;
			}
			if (namespaceStats.IsExpired)
			{
				if (namespaceStats.ClaimExpiration())
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "namespace statistics for '{0}' have expired. Clearing statstics.", namespaceStats.Fqdn);
					this.ClearStatistic(fqdn, traceId);
				}
				namespaceStats = this.FindStatistics(fqdn, traceId);
			}
			else if (namespaceStats.ClaimBlacklistExpired())
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "blacklisting for namespace '{0}' has expired. Clearing statistics.", namespaceStats.Fqdn);
				this.ClearStatistic(fqdn, traceId);
				namespaceStats = this.FindStatistics(fqdn, traceId);
			}
			else if (namespaceStats.ClaimTarpitExpired())
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "tar pitting for namespace '{0}' has expired. Clearing statistics.", namespaceStats.Fqdn);
				this.ClearStatistic(fqdn, traceId);
				namespaceStats = this.FindStatistics(fqdn, traceId);
			}
			return namespaceStats;
		}

		public NamespaceStats TryGetStatistics(string fqdn, int traceId)
		{
			fqdn = fqdn.ToLowerInvariant();
			NamespaceStats result = null;
			if (this.confirmedNamespaceStats.TryGetValue(fqdn, out result))
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "Retrieved statistics for namespace '{0}' from confirmed namespace stats cache", fqdn);
			}
			else if (this.unconfirmedStats.TryGetValue(fqdn, out result))
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "Retrieved statistics for namespace '{0}' from unconfirmed stats cache", fqdn);
			}
			else
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "Could not find statistics for namespace '{0}' in cache", fqdn);
			}
			return result;
		}

		public void MarkNamespaceVerified(string fqdn, int traceId)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)traceId, "Entered MarkNamespaceValidated");
			fqdn = fqdn.ToLowerInvariant();
			NamespaceStats namespaceStats = null;
			if (this.unconfirmedStats.TryGetValue(fqdn, out namespaceStats))
			{
				lock (this)
				{
					if (this.unconfirmedStats.TryGetValue(fqdn, out namespaceStats))
					{
						ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "Moving statistics for namespace '{0}' to confirmed namespace stats cache", fqdn);
						namespaceStats.VerifiedNamespace = true;
						this.unconfirmedStats.Remove(fqdn);
						this.confirmedNamespaceStats.Add(fqdn, namespaceStats);
					}
					goto IL_98;
				}
			}
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "statistics for namespace '{0}' not found in unconfirmed namespace stats cache", fqdn);
			IL_98:
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)traceId, "Leaving MarkNamespaceValidated");
		}

		private NamespaceStats FindStatistics(string fqdn, int traceId)
		{
			NamespaceStats namespaceStats = this.TryGetStatistics(fqdn, traceId);
			if (namespaceStats == null)
			{
				lock (this)
				{
					namespaceStats = this.TryGetStatistics(fqdn, traceId);
					if (namespaceStats == null)
					{
						namespaceStats = new NamespaceStats();
						namespaceStats.Fqdn = fqdn.ToLowerInvariant();
						namespaceStats.Expires = DateTime.UtcNow + TimeSpan.FromMinutes((double)AuthServiceStaticConfig.Config.statisticLifetime);
						this.unconfirmedStats.Add(namespaceStats.Fqdn, namespaceStats);
						ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "Created new statistics entry for namespace '{0}' in unconfirmed stats cache", namespaceStats.Fqdn);
					}
				}
			}
			return namespaceStats;
		}

		private void ClearStatistic(string fqdn, int traceId)
		{
			fqdn = fqdn.ToLowerInvariant();
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "Removing statistics for namespace '{0}' from cache", fqdn);
			this.unconfirmedStats.Remove(fqdn);
			this.confirmedNamespaceStats.Remove(fqdn);
		}

		private bool AddToBlacklist(AuthServiceStaticConfig config, NamespaceStats stats, int traceId)
		{
			if (config.trustedNamespaces.ContainsKey(stats.Fqdn))
			{
				ExTraceGlobals.AuthenticationTracer.Information<string>((long)traceId, "Namespace '{0}' is trusted and cannot be blacklisted", stats.Fqdn);
				return false;
			}
			if (!stats.VerifiedNamespace)
			{
				this.MarkNamespaceVerified(stats.Fqdn, traceId);
			}
			bool flag = false;
			lock (this.blacklist)
			{
				if (!stats.IsBlacklisted)
				{
					flag = true;
					this.blacklist.AddLast(stats);
					stats.BlacklistExpires = DateTime.UtcNow.AddMinutes((double)config.blacklistTime);
				}
			}
			if (flag)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string, int>((long)traceId, "Blacklisting namespace '{0}' for {1} minute(s)", stats.Fqdn, config.blacklistTime);
				this.UpdatePerfCounters(config);
			}
			else
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "Namespace '{0}' is already blacklisted", stats.Fqdn);
			}
			return flag;
		}

		private bool TarpitNamespace(AuthServiceStaticConfig config, NamespaceStats stats, int traceId)
		{
			bool result = false;
			if (!stats.IsTarpitted)
			{
				if (!stats.VerifiedNamespace)
				{
					this.MarkNamespaceVerified(stats.Fqdn, traceId);
				}
				result = true;
				stats.TarpitExpires = DateTime.UtcNow.AddMinutes((double)config.tarpitTime);
				ExTraceGlobals.AuthenticationTracer.Information<string, int>((long)traceId, "Tar pitting namespace '{0}' for {1} minute(s)", stats.Fqdn, config.tarpitTime);
			}
			else
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)traceId, "Namespace '{0}' is already tar pitted", stats.Fqdn);
			}
			return result;
		}

		private MruDictionary<string, DomainConfig> hrdCache;

		private TimeoutCache<string, int> negoConfigCache;

		private MruDictionaryCache<string, NamespaceStats> confirmedNamespaceStats;

		private MruDictionaryCache<string, NamespaceStats> unconfirmedStats;

		private TimeSpan configLifetime;

		private LinkedList<NamespaceStats> blacklist;
	}
}
