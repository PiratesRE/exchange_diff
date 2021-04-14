using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Services.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ExchangePrincipalPerformanceLogger : IPerformanceDataLogger
	{
		internal ExchangePrincipalPerformanceLogger(RequestDetailsLogger logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			this.logger = logger;
		}

		public void Log(string marker, string counter, TimeSpan dataPoint)
		{
			ExTraceGlobals.ExchangePrincipalCacheTracer.TraceDebug<string, string, double>((long)this.GetHashCode(), "PerfLog: {0}.{1}={2}ms", marker, counter, dataPoint.TotalMilliseconds);
			if (dataPoint.TotalMilliseconds < 50.0)
			{
				return;
			}
			this.MapToMetadataAndLog(marker, counter, dataPoint.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
		}

		public void Log(string marker, string counter, uint dataPoint)
		{
			ExTraceGlobals.ExchangePrincipalCacheTracer.TraceDebug<string, string, uint>((long)this.GetHashCode(), "PerfLog: {0}.{1}={2}", marker, counter, dataPoint);
			if (dataPoint < 50U)
			{
				return;
			}
			this.MapToMetadataAndLog(marker, counter, dataPoint.ToString(CultureInfo.InvariantCulture));
		}

		public void Log(string marker, string counter, string dataPoint)
		{
			throw new NotImplementedException();
		}

		private void MapToMetadataAndLog(string marker, string counter, string dataPoint)
		{
			string key;
			if (!ExchangePrincipalPerformanceLogger.MarkerAndCounterToMetadataMap.TryGetValue(Tuple.Create<string, string>(marker, counter), out key))
			{
				return;
			}
			if (this.logger != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendDetailedExchangePrincipalLatency(this.logger, key, dataPoint);
			}
		}

		public void ReleaseReferenceToRequestDetailsLogger()
		{
			this.logger = null;
		}

		private const string ElapsedTime = "ElapsedTime";

		private const uint MinLatencyLogThresholdMilliseconds = 50U;

		private static readonly Dictionary<Tuple<string, string>, string> MarkerAndCounterToMetadataMap = new Dictionary<Tuple<string, string>, string>
		{
			{
				Tuple.Create<string, string>("UpdateDatabaseLocationInfo", "ElapsedTime"),
				"EPUpdateDbLocInfoLatency"
			},
			{
				Tuple.Create<string, string>("UpdateCrossPremiseStatus", "ElapsedTime"),
				"EPUpdateXPremiseStatusLatency"
			},
			{
				Tuple.Create<string, string>("UpdateCrossPremiseStatusFindByExchangeGuidIncludingAlternate", "ElapsedTime"),
				"EPUpdateXPremiseStatusFindByExchangeGuidIncludingAlternateLatency"
			},
			{
				Tuple.Create<string, string>("UpdateCrossPremiseStatusFindByLegacyExchangeDN", "ElapsedTime"),
				"EPUpdateXPremiseStatusFindByLegacyExchangeDNLatency"
			},
			{
				Tuple.Create<string, string>("UpdateCrossPremiseStatusRemoteMailbox", "ElapsedTime"),
				"EPUpdateXPremiseStatusRemoteMailboxLatency"
			},
			{
				Tuple.Create<string, string>("UpdateDelegationTokenRequest", "ElapsedTime"),
				"EPUpdateDelegationTokenRequestLatency"
			},
			{
				Tuple.Create<string, string>("GetUserSKUCapability", "ElapsedTime"),
				"EPGetUserSKUCapabilityLatency"
			},
			{
				Tuple.Create<string, string>("GetIsLicensingEnforcedInOrg", "ElapsedTime"),
				"EPGetIsLicensingEnforcedInOrgLatency"
			},
			{
				Tuple.Create<string, string>("GetServerForDatabaseGetServerInformationForDatabase", "ElapsedTime"),
				"EPUpdateDbLocInfoGetServerForDbGetServerInformationForDbLatency"
			},
			{
				Tuple.Create<string, string>("GetServerForDatabaseGetServerNameForDatabase", "ElapsedTime"),
				"EPUpdateDbLocInfoGetServerForDbGetServerNameForDbLatency"
			},
			{
				Tuple.Create<string, string>("GetServerInformationForDatabaseGetDatabaseByGuidEx", "ElapsedTime"),
				"EPUpdateDbLocInfoGetServerInformationForDbGetDbByGuidExLatency"
			},
			{
				Tuple.Create<string, string>("GetServerNameForDatabaseGetDatabaseByGuidEx", "ElapsedTime"),
				"EPUpdateDbLocInfoGetServerNameForDbGetDbByGuidExLatency"
			},
			{
				Tuple.Create<string, string>("GetServerNameForDatabaseLookupDatabaseAndPossiblyPopulateCache", "ElapsedTime"),
				"EPUpdateDbLocInfoGetServerNameForDbLookupDbAndPossiblyPopulateCacheLatency"
			},
			{
				Tuple.Create<string, string>("ADQuery", "ElapsedTime"),
				"EPADQueryLatency"
			}
		};

		private RequestDetailsLogger logger;
	}
}
