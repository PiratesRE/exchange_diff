using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class MserveHelper
	{
		private static int CurrentSiteId
		{
			get
			{
				if (-1 == MserveHelper.currentSiteId)
				{
					lock (MserveHelper.lockObj)
					{
						if (-1 == MserveHelper.currentSiteId)
						{
							try
							{
								ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
								MserveHelper.currentSiteId = adtopologyLookup.GetLocalPartnerId();
							}
							catch (Exception ex)
							{
								CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, null, "MservLookup: Get Current SiteId failed, Exception - '{0}'", new object[]
								{
									ex.Message
								});
								throw;
							}
						}
					}
				}
				return MserveHelper.currentSiteId;
			}
		}

		public static bool TryMapTenantGuidToForest(Guid tenantGuid, string umPodRedirectTemplate, out string forestFqdn)
		{
			return MserveHelper.ExcecuteMservLookupOperation(tenantGuid.ToString(), () => EdgeSyncMservConnector.GetRedirectServer(umPodRedirectTemplate, tenantGuid, MserveHelper.CurrentSiteId, 50000, 59999, false, true), umPodRedirectTemplate, out forestFqdn);
		}

		public static bool TryMapOrganizationToForest(string domainName, string umPodRedirectTemplate, out string forestFqdn)
		{
			string addressToLookup = string.Format("E5CB63F56E8B4b69A1F70C192276D6AD@{0}", domainName);
			return MserveHelper.ExcecuteMservLookupOperation(addressToLookup, () => EdgeSyncMservConnector.GetRedirectServer(umPodRedirectTemplate, addressToLookup, MserveHelper.CurrentSiteId, 50000, 59999, false, true), umPodRedirectTemplate, out forestFqdn);
		}

		private static bool ExcecuteMservLookupOperation(string addressToLookup, Func<string> mservOperation, string umPodRedirectTemplate, out string forestFqdn)
		{
			ValidateArgument.NotNullOrEmpty(umPodRedirectTemplate, "umPodRedirectTemplate");
			ValidateArgument.NotNullOrEmpty(addressToLookup, "addressToLookup");
			ValidateArgument.NotNull(mservOperation, "mservOperation");
			Exception ex = null;
			forestFqdn = string.Empty;
			try
			{
				ExDateTime now = ExDateTime.Now;
				forestFqdn = mservOperation();
				TimeSpan timeSpan = ExDateTime.Now - now;
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MserveLookup, null, new object[]
				{
					addressToLookup,
					forestFqdn,
					timeSpan.TotalMilliseconds,
					CallId.Id
				});
				MServeLatencyContext.UpdateContext(1U, (int)timeSpan.TotalMilliseconds);
				if (string.IsNullOrEmpty(forestFqdn))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Mserve entry not found for '{0}'", new object[]
					{
						addressToLookup
					});
					return false;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Mserve entry found for '{0}' - Forest FQDN = '{1}'", new object[]
				{
					addressToLookup,
					forestFqdn
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MserveLookupTargetForest, null, new object[]
				{
					addressToLookup,
					forestFqdn,
					CallId.Id
				});
				return true;
			}
			catch (TransientException ex2)
			{
				ex = ex2;
			}
			catch (ADOperationException ex3)
			{
				ex = ex3;
			}
			catch (InvalidOperationException ex4)
			{
				ex = ex4;
			}
			catch (MServTenantNotFoundException ex5)
			{
				ex = ex5;
			}
			catch (GlsPermanentException ex6)
			{
				ex = ex6;
			}
			catch (Exception ex7)
			{
				ex = ex7;
				throw;
			}
			finally
			{
				if (ex != null)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MserveLookupError, null, new object[]
					{
						addressToLookup,
						CommonUtil.ToEventLogString(ex.ToString()),
						CallId.Id
					});
					CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, null, "Mserve lookup failed for '{0}', Exception - '{1}'", new object[]
					{
						addressToLookup,
						ex.Message
					});
				}
			}
			return false;
		}

		private const int PodSiteStartRange = 50000;

		private const int PodSiteEndRange = 59999;

		private static object lockObj = new object();

		private static int currentSiteId = -1;
	}
}
