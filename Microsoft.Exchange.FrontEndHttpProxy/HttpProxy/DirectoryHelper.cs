using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class DirectoryHelper
	{
		internal static IRecipientSession GetRecipientSessionFromPartition(LatencyTracker latencyTracker, string partitionId)
		{
			if (latencyTracker == null)
			{
				throw new ArgumentNullException("latencyTracker");
			}
			if (string.IsNullOrEmpty(partitionId))
			{
				throw new ArgumentNullException("partitionId");
			}
			ADSessionSettings adsessionSettings = null;
			PartitionId partitionIdObject = null;
			if ((Utilities.IsPartnerHostedOnly || VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled) && PartitionId.TryParse(partitionId, out partitionIdObject))
			{
				try
				{
					adsessionSettings = DirectoryHelper.ExecuteFunctionAndUpdateMovingAveragePerformanceCounter<ADSessionSettings>(PerfCounters.HttpProxyCountersInstance.MovingAverageTenantLookupLatency, () => DirectoryHelper.InvokeGls<ADSessionSettings>(latencyTracker, () => ADSessionSettings.FromAllTenantsPartitionId(partitionIdObject)));
				}
				catch (CannotResolvePartitionException arg)
				{
					ExTraceGlobals.VerboseTracer.TraceWarning<string, CannotResolvePartitionException>(0L, "[DirectoryHelper::GetRecipientSessionFromPartition] Caught CannotResolvePartitionException when resolving partition by partition ID {0}. Exception details: {1}.", partitionId, arg);
				}
			}
			if (adsessionSettings == null)
			{
				adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			}
			return DirectoryHelper.CreateSession(adsessionSettings);
		}

		internal static IRecipientSession GetRecipientSessionFromSmtpOrLiveId(LatencyTracker latencyTracker, string smtpOrLiveId, bool ignoreCannotResolveTenantNameException = false)
		{
			if (latencyTracker == null)
			{
				throw new ArgumentNullException("latencyTracker");
			}
			if (string.IsNullOrEmpty(smtpOrLiveId))
			{
				throw new ArgumentNullException("smtpOrLiveId");
			}
			if ((Utilities.IsPartnerHostedOnly || VariantConfiguration.InvariantNoFlightingSnapshot.Global.WindowsLiveID.Enabled) && SmtpAddress.IsValidSmtpAddress(smtpOrLiveId))
			{
				string domain = SmtpAddress.Parse(smtpOrLiveId).Domain;
				if (!string.IsNullOrEmpty(domain))
				{
					try
					{
						return DirectoryHelper.GetRecipientSessionFromDomain(latencyTracker, domain, ignoreCannotResolveTenantNameException);
					}
					catch (CannotResolveTenantNameException ex)
					{
						if (!ignoreCannotResolveTenantNameException)
						{
							throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.DomainNotFound, ex.Message, ex);
						}
						ExTraceGlobals.VerboseTracer.TraceWarning<string, CannotResolveTenantNameException>(0L, "[DirectoryHelper::GetRecipientSessionFromSmtpOrLiveId] Caught CannotResolveTenantNameException when trying to get domain scopederecipient session from smtp or liveId for {0}. Exception details: {1}.", smtpOrLiveId, ex);
						return null;
					}
				}
			}
			return DirectoryHelper.GetRootOrgRecipientSession();
		}

		internal static ITenantRecipientSession GetTenantRecipientSessionFromSmtpOrLiveId(LatencyTracker latencyTracker, string smtpOrLiveId, bool ignoreCannotResolveTenantNameException = false)
		{
			if (!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Global.WindowsLiveID.Enabled)
			{
				throw new InvalidOperationException("Cannot create ITenantRecipientSession if WindowsLiveId feature is disabled.");
			}
			if (latencyTracker == null)
			{
				throw new ArgumentNullException("latencyTracker");
			}
			if (string.IsNullOrEmpty(smtpOrLiveId))
			{
				throw new ArgumentNullException("smtpOrLiveId");
			}
			if (!SmtpAddress.IsValidSmtpAddress(smtpOrLiveId))
			{
				throw new ArgumentException(string.Format("{0} is not a valid SmtpAddress.", smtpOrLiveId));
			}
			string domain = SmtpAddress.Parse(smtpOrLiveId).Domain;
			ADSessionSettings sessionSettings = null;
			try
			{
				sessionSettings = DirectoryHelper.CreateADSessionSettingsFromDomain(latencyTracker, domain);
			}
			catch (CannotResolveTenantNameException ex)
			{
				if (!ignoreCannotResolveTenantNameException)
				{
					throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.DomainNotFound, ex.Message, ex);
				}
				ExTraceGlobals.VerboseTracer.TraceWarning<string, CannotResolveTenantNameException>(0L, "[DirectoryHelper::GetTenantRecipientSessionFromSmtpOrLiveId] Caught CannotResolveTenantNameException when trying to get tenant recipient session from smtp or liveId for {0}. Exception details: {1}.", smtpOrLiveId, ex);
				return null;
			}
			return DirectorySessionFactory.Default.CreateTenantRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 211, "GetTenantRecipientSessionFromSmtpOrLiveId", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\Misc\\DirectoryHelper.cs");
		}

		internal static IRecipientSession GetRecipientSessionFromDomain(LatencyTracker latencyTracker, string domain, bool ignoreCannotResolveTenantNameException = false)
		{
			if (latencyTracker == null)
			{
				throw new ArgumentNullException("latencyTracker");
			}
			if (string.IsNullOrEmpty(domain))
			{
				throw new ArgumentNullException("domain");
			}
			ADSessionSettings sessionSettings = null;
			if (!Utilities.IsPartnerHostedOnly)
			{
				if (!VariantConfiguration.InvariantNoFlightingSnapshot.Global.WindowsLiveID.Enabled)
				{
					goto IL_7F;
				}
			}
			try
			{
				sessionSettings = DirectoryHelper.CreateADSessionSettingsFromDomain(latencyTracker, domain);
				goto IL_85;
			}
			catch (CannotResolveTenantNameException ex)
			{
				ExTraceGlobals.VerboseTracer.TraceWarning<string, CannotResolveTenantNameException>(0L, "[DirectoryHelper::GetRecipientSessionFromDomain] Caught CannotResolveTenantNameException when resolving tenant by domain {0}. Exception details: {1}.", domain, ex);
				if (!ignoreCannotResolveTenantNameException)
				{
					throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.DomainNotFound, ex.Message, ex);
				}
				return null;
			}
			IL_7F:
			sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			IL_85:
			return DirectoryHelper.CreateSession(sessionSettings);
		}

		internal static IRecipientSession GetRecipientSessionFromExternalDirectoryOrganizationId(LatencyTracker latencyTracker, Guid externalOrgId)
		{
			if (latencyTracker == null)
			{
				throw new ArgumentNullException("latencyTracker");
			}
			if (externalOrgId == Guid.Empty)
			{
				throw new ArgumentNullException("latencyTracker");
			}
			ADSessionSettings sessionSettings = null;
			if (!Utilities.IsPartnerHostedOnly)
			{
				if (!VariantConfiguration.InvariantNoFlightingSnapshot.Global.WindowsLiveID.Enabled)
				{
					goto IL_A4;
				}
			}
			try
			{
				sessionSettings = DirectoryHelper.ExecuteFunctionAndUpdateMovingAveragePerformanceCounter<ADSessionSettings>(PerfCounters.HttpProxyCountersInstance.MovingAverageTenantLookupLatency, () => DirectoryHelper.InvokeGls<ADSessionSettings>(latencyTracker, () => ADSessionSettings.FromExternalDirectoryOrganizationId(externalOrgId)));
				goto IL_AA;
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException ex)
			{
				throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.DomainNotFound, ex.Message, ex);
			}
			IL_A4:
			sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			IL_AA:
			return DirectoryHelper.CreateSession(sessionSettings);
		}

		internal static IRecipientSession GetTenantRecipientSessionByMSAUserNetID(LatencyTracker latencyTracker, string msaUserNetID, bool ignoreCannotResolveMSAUserNetIDException, bool requestForestWideDomainControllerAffinityByUserId)
		{
			if (latencyTracker == null)
			{
				throw new ArgumentNullException("latencyTracker");
			}
			NetID netID;
			if (!NetID.TryParse(msaUserNetID, out netID))
			{
				throw new ArgumentException("msaUserNetID");
			}
			ADSessionSettings sessionSettings = null;
			if (!Utilities.IsPartnerHostedOnly)
			{
				if (!VariantConfiguration.InvariantNoFlightingSnapshot.Global.WindowsLiveID.Enabled)
				{
					goto IL_AE;
				}
			}
			try
			{
				sessionSettings = DirectoryHelper.CreateADTenantSessionSettingsByMSAUserNetID(latencyTracker, msaUserNetID);
				goto IL_B4;
			}
			catch (CannotResolveMSAUserNetIDException ex)
			{
				if (!ignoreCannotResolveMSAUserNetIDException)
				{
					ExTraceGlobals.VerboseTracer.TraceWarning<string, CannotResolveMSAUserNetIDException>(0L, "[DirectoryHelper::GetTenantRecipientSessionByMSAUserNetID] Caught CannotResolveMSAUserNetIDException when trying to get tenant recipient session by MSA user NetID for {0}. Exception details: {1}.", msaUserNetID, ex);
					throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.UserNotFound, ex.Message, ex);
				}
				ExTraceGlobals.VerboseTracer.TraceWarning<string, CannotResolveMSAUserNetIDException>(0L, "[DirectoryHelper::GetTenantRecipientSessionByMSAUserNetID] Caught CannotResolveMSAUserNetIDException when trying to get tenant recipient session by MSA user NetID for {0}. Exception details: {1}.", msaUserNetID, ex);
				return null;
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException ex2)
			{
				throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.DomainNotFound, ex2.Message, ex2);
			}
			IL_AE:
			sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			IL_B4:
			return DirectoryHelper.CreateSession(sessionSettings, msaUserNetID, requestForestWideDomainControllerAffinityByUserId);
		}

		internal static IRecipientSession GetRecipientSessionFromOrganizationId(LatencyTracker latencyTracker, OrganizationId organizationId)
		{
			if (latencyTracker == null)
			{
				throw new ArgumentNullException("latencyTracker");
			}
			if (organizationId == null)
			{
				organizationId = OrganizationId.ForestWideOrgId;
			}
			ADSessionSettings sessionSettings = DirectoryHelper.ExecuteFunctionAndUpdateMovingAveragePerformanceCounter<ADSessionSettings>(PerfCounters.HttpProxyCountersInstance.MovingAverageTenantLookupLatency, () => DirectoryHelper.InvokeGls<ADSessionSettings>(latencyTracker, () => ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId)));
			return DirectoryHelper.CreateSession(sessionSettings);
		}

		internal static IRecipientSession GetRootOrgRecipientSession()
		{
			return DirectoryHelper.CreateSession(ADSessionSettings.FromRootOrgScopeSet());
		}

		internal static ITopologyConfigurationSession GetConfigurationSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 465, "GetConfigurationSession", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\Misc\\DirectoryHelper.cs");
		}

		internal static IConfigurationSession GetRootOrgOrAllTenantsConfigurationSession(ADObjectId objectId)
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(objectId), 477, "GetRootOrgOrAllTenantsConfigurationSession", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\Misc\\DirectoryHelper.cs");
		}

		internal static IConfigurationSession GetConfigurationSessionFromDomain(string domain)
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, string.IsNullOrEmpty(domain) ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromTenantAcceptedDomain(domain), 489, "GetConfigurationSessionFromDomain", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\Misc\\DirectoryHelper.cs");
		}

		internal static T InvokeGls<T>(LatencyTracker latencyTracker, Func<T> glsCall)
		{
			if (latencyTracker == null)
			{
				throw new ArgumentNullException("latencyTracker");
			}
			if (glsCall == null)
			{
				throw new ArgumentNullException("glsCall");
			}
			long latency = 0L;
			T latency2 = LatencyTracker.GetLatency<T>(() => glsCall(), out latency);
			latencyTracker.HandleGlsLatency(latency);
			return latency2;
		}

		internal static ADRawEntry InvokeAccountForest(LatencyTracker latencyTracker, Func<ADRawEntry> activeDirectoryFunction)
		{
			if (latencyTracker == null)
			{
				throw new ArgumentNullException("latencyTracker");
			}
			if (activeDirectoryFunction == null)
			{
				throw new ArgumentNullException("activeDirectoryFunction");
			}
			long latency = 0L;
			ADRawEntry latency2 = LatencyTracker.GetLatency<ADRawEntry>(() => activeDirectoryFunction(), out latency);
			latencyTracker.HandleAccountLatency(latency);
			if (latency2 != null)
			{
				string originatingServer = latency2.OriginatingServer;
			}
			return latency2;
		}

		internal static MailboxDatabase[] InvokeResourceForest(LatencyTracker latencyTracker, Func<MailboxDatabase[]> activeDirectoryFunction)
		{
			if (latencyTracker == null)
			{
				throw new ArgumentNullException("latencyTracker");
			}
			if (activeDirectoryFunction == null)
			{
				throw new ArgumentNullException("activeDirectoryFunction");
			}
			long latency = 0L;
			MailboxDatabase[] latency2 = LatencyTracker.GetLatency<MailboxDatabase[]>(() => activeDirectoryFunction(), out latency);
			latencyTracker.HandleResourceLatency(latency);
			if (latency2 != null && latency2.Length > 0)
			{
				string originatingServer = latency2[0].OriginatingServer;
			}
			return latency2;
		}

		private static IRecipientSession CreateSession(ADSessionSettings sessionSettings)
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 619, "CreateSession", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\Misc\\DirectoryHelper.cs");
		}

		private static IRecipientSession CreateSession(ADSessionSettings sessionSettings, string netId, bool requestForestWideDomainControllerAffinityByUserId)
		{
			string domainController = null;
			if (requestForestWideDomainControllerAffinityByUserId)
			{
				domainController = DirectoryHelper.GetDomainControllerWithForestWideAffinityByUserId(netId, sessionSettings.CurrentOrganizationId);
			}
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, true, ConsistencyMode.IgnoreInvalid, sessionSettings, 649, "CreateSession", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\Misc\\DirectoryHelper.cs");
		}

		private static ADSessionSettings CreateADSessionSettingsFromDomain(LatencyTracker latencyTracker, string domain)
		{
			return DirectoryHelper.ExecuteFunctionAndUpdateMovingAveragePerformanceCounter<ADSessionSettings>(PerfCounters.HttpProxyCountersInstance.MovingAverageTenantLookupLatency, () => DirectoryHelper.InvokeGls<ADSessionSettings>(latencyTracker, () => ADSessionSettings.FromTenantAcceptedDomain(domain)));
		}

		private static ADSessionSettings CreateADTenantSessionSettingsByMSAUserNetID(LatencyTracker latencyTracker, string msaUserNetID)
		{
			return DirectoryHelper.ExecuteFunctionAndUpdateMovingAveragePerformanceCounter<ADSessionSettings>(PerfCounters.HttpProxyCountersInstance.MovingAverageTenantLookupLatency, () => DirectoryHelper.InvokeGls<ADSessionSettings>(latencyTracker, () => ADSessionSettings.FromTenantMSAUser(msaUserNetID)));
		}

		private static T ExecuteFunctionAndUpdateMovingAveragePerformanceCounter<T>(ExPerformanceCounter performanceCounter, Func<T> operationToTrack)
		{
			long newValue = 0L;
			T latency;
			try
			{
				latency = LatencyTracker.GetLatency<T>(operationToTrack, out newValue);
			}
			finally
			{
				PerfCounters.UpdateMovingAveragePerformanceCounter(performanceCounter, newValue);
			}
			return latency;
		}
	}
}
