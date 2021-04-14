using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HygieneData;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class RegionTagRetriever
	{
		static RegionTagRetriever()
		{
			RegionTagRetriever.regionTagCache = RegionTagCache.GetInstance();
		}

		public RegionTagRetriever(GlsCallerId callerId)
		{
			ArgumentValidator.ThrowIfNull("callerId", callerId);
			this.glsCallerId = callerId;
			lock (RegionTagRetriever.glsSessionMapLock)
			{
				if (RegionTagRetriever.callIdToGlsSessionMap.ContainsKey(callerId))
				{
					this.glsSession = RegionTagRetriever.callIdToGlsSessionMap[callerId];
				}
				else
				{
					this.glsSession = this.CreateGlsSession(callerId);
					RegionTagRetriever.callIdToGlsSessionMap[callerId] = this.glsSession;
				}
			}
		}

		public static void AddRegionTag(Guid tenantId, string regionTag)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("regionTag", regionTag);
			RegionTagRetriever.regionTagCache.AddGoodTenant(tenantId, regionTag);
		}

		public string GetRegionTag(Guid tenantId)
		{
			bool flag;
			return this.GetRegionTag(tenantId, out flag, false, false);
		}

		public string GetRegionTag(Guid tenantId, out bool cacheHit, bool throwOnError = false)
		{
			return this.GetRegionTag(tenantId, out cacheHit, throwOnError, false);
		}

		public string GetRegionTag(Guid tenantId, out bool cacheHit, bool throwOnError, bool useMachineRegionIfGLSTenantWithoutRegion)
		{
			cacheHit = true;
			string text = RegionTagRetriever.regionTagCache.Get(tenantId);
			if (text == null)
			{
				cacheHit = false;
				bool flag = false;
				text = this.QueryGls(tenantId, throwOnError, out flag);
				if (string.IsNullOrWhiteSpace(text) && useMachineRegionIfGLSTenantWithoutRegion)
				{
					text = DalHelper.RegionTag;
					RegionTagRetriever.regionTagCache.AddGoodTenant(tenantId, text);
				}
			}
			return text;
		}

		protected virtual string GetRegionTagFromGLS(Guid tenantId)
		{
			return this.glsSession.GetFfoTenantRegionByOrgGuid(tenantId);
		}

		protected virtual GlsDirectorySession CreateGlsSession(GlsCallerId callerId)
		{
			return new GlsDirectorySession(callerId);
		}

		private static void PublishRecoverEvent(string msg)
		{
			if (Interlocked.CompareExchange(ref RegionTagRetriever.isGlsOk, 1, 0) == 0)
			{
				EventNotificationItem.Publish(ExchangeComponent.MessageTracing.Name, "RegionTagNonUrgent", null, msg, ResultSeverityLevel.Informational, false);
			}
		}

		private string QueryGls(Guid tenantId, bool throwOnError, out bool tenantInGLS)
		{
			string text = string.Empty;
			string result = null;
			int i = 3;
			Exception ex = null;
			tenantInGLS = true;
			while (i > 0)
			{
				try
				{
					ex = null;
					result = null;
					string regionTagFromGLS = this.GetRegionTagFromGLS(tenantId);
					if (string.IsNullOrWhiteSpace(regionTagFromGLS))
					{
						EventLogger.Logger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_FailedToRetrieveRegionTagWhenQueryGLS, tenantId.ToString(), new object[]
						{
							tenantId,
							this.glsCallerId.CallerIdString,
							"Region property is not valid for the tenant."
						});
						result = string.Empty;
						RegionTagRetriever.regionTagCache.AddBadTenant(tenantId);
					}
					else
					{
						RegionTagRetriever.PublishRecoverEvent("Retrieving region property from GLS succeeded.");
						RegionTagRetriever.regionTagCache.AddGoodTenant(tenantId, regionTagFromGLS);
						result = regionTagFromGLS.Trim();
					}
				}
				catch (Exception ex2)
				{
					ex = ex2;
					text = string.Format("Exception from GLS when fetching region tag. Tenant:{1} Detail is:\n{0}", ex2, tenantId);
					ExTraceGlobals.GLSQueryTracer.TraceError((long)this.GetHashCode(), text);
					if (ex is GlsTenantNotFoundException)
					{
						EventLogger.Logger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_FailedToRetrieveRegionTagWhenQueryGLS, tenantId.ToString(), new object[]
						{
							tenantId,
							this.glsCallerId.CallerIdString,
							"Unable to find Tenant in GLS"
						});
						result = string.Empty;
						RegionTagRetriever.regionTagCache.AddBadTenant(tenantId);
						tenantInGLS = false;
						ex = null;
					}
					else if (ex is GlsTransientException)
					{
						EventLogger.Logger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_TransientExceptionWhenQueryGLS, ex.Message, new object[]
						{
							ex,
							tenantId
						});
						if (throwOnError && i == 0)
						{
							throw;
						}
					}
					else
					{
						if (!(ex is GlsPermanentException))
						{
							EventLogger.Logger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_UnknownExceptionWhenQueryGLS, ex.Message, new object[]
							{
								ex,
								tenantId
							});
							i = 0;
							throw;
						}
						EventLogger.Logger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_PermanentExceptionWhenQueryGLS, ex.Message, new object[]
						{
							ex,
							tenantId
						});
						i = 0;
						if (throwOnError)
						{
							throw;
						}
					}
				}
				finally
				{
					if (ex != null)
					{
						if (i == 0)
						{
							this.PublishErrorEvent(text);
						}
						i--;
					}
					else
					{
						i = 0;
					}
				}
			}
			return result;
		}

		private void PublishErrorEvent(string error)
		{
			if (Interlocked.CompareExchange(ref RegionTagRetriever.isGlsOk, 0, 1) == 1)
			{
				EventNotificationItem.Publish(ExchangeComponent.MessageTracing.Name, "RegionTagNonUrgent", null, string.Format("{0}\nThe caller id is {1}", error, this.glsCallerId.CallerIdString), ResultSeverityLevel.Error, false);
			}
		}

		private const string NonUrgentComponentName = "RegionTagNonUrgent";

		private static RegionTagCache regionTagCache;

		private static int isGlsOk = 1;

		private static Dictionary<GlsCallerId, GlsDirectorySession> callIdToGlsSessionMap = new Dictionary<GlsCallerId, GlsDirectorySession>();

		private GlsCallerId glsCallerId;

		private GlsDirectorySession glsSession;

		private static object glsSessionMapLock = new object();
	}
}
