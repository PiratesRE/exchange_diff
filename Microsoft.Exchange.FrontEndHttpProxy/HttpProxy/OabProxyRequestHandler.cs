using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.OAB;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.HttpProxy
{
	internal sealed class OabProxyRequestHandler : BEServerCookieProxyRequestHandler<OabService>
	{
		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.Internal;
			}
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			AnchorMailbox anchorMailbox = null;
			AnchorMailbox anchorMailbox2 = base.ResolveAnchorMailbox();
			UserBasedAnchorMailbox userBasedAnchorMailbox = anchorMailbox2 as UserBasedAnchorMailbox;
			if (userBasedAnchorMailbox == null)
			{
				return anchorMailbox2;
			}
			userBasedAnchorMailbox.UseServerCookie = true;
			string targetOrgMailbox = base.HttpContext.Request.Headers["TargetOrgMailbox"];
			Guid guid = Guid.Empty;
			if (!string.IsNullOrEmpty(targetOrgMailbox))
			{
				IRecipientSession session = DirectoryHelper.GetRecipientSessionFromSmtpOrLiveId(base.LatencyTracker, targetOrgMailbox, false);
				ADRawEntry adrawEntry = DirectoryHelper.InvokeAccountForest(base.LatencyTracker, () => OrganizationMailbox.GetOrganizationMailboxByUPNAndCapability(session, targetOrgMailbox, OrganizationCapability.OABGen));
				if (adrawEntry != null)
				{
					anchorMailbox = new UserADRawEntryAnchorMailbox(adrawEntry, this);
				}
			}
			else
			{
				AnchoredRoutingTarget anchoredRoutingTarget = this.TryFastTargetCalculationByAnchorMailbox(anchorMailbox2);
				if (anchoredRoutingTarget != null)
				{
					return anchoredRoutingTarget.AnchorMailbox;
				}
				ADRawEntry adrawEntry2 = userBasedAnchorMailbox.GetADRawEntry();
				if (adrawEntry2 == null)
				{
					return anchorMailbox2;
				}
				guid = OABRequestUrl.GetOabGuidFromRequest(base.HttpContext.Request);
				if (guid == Guid.Empty)
				{
					return anchorMailbox2;
				}
				OrganizationId organizationId = (OrganizationId)adrawEntry2[ADObjectSchema.OrganizationId];
				string userAcceptedDomain = null;
				if (organizationId != OrganizationId.ForestWideOrgId)
				{
					userAcceptedDomain = ((SmtpAddress)adrawEntry2[ADRecipientSchema.PrimarySmtpAddress]).Domain;
				}
				OABCache.OABCacheEntry oabfromCacheOrAD = OABCache.Instance.GetOABFromCacheOrAD(guid, userAcceptedDomain);
				if (oabfromCacheOrAD.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2012))
				{
					anchorMailbox = this.GetE14CASServer(oabfromCacheOrAD);
				}
				else
				{
					ADRawEntry adrawEntry3 = null;
					if (OABVariantConfigurationSettings.IsLinkedOABGenMailboxesEnabled && !oabfromCacheOrAD.ShadowMailboxDistributionEnabled && oabfromCacheOrAD.GeneratingMailbox != null)
					{
						IRecipientSession recipientSessionFromOrganizationId = DirectoryHelper.GetRecipientSessionFromOrganizationId(base.LatencyTracker, organizationId);
						adrawEntry3 = recipientSessionFromOrganizationId.Read(oabfromCacheOrAD.GeneratingMailbox);
					}
					if (adrawEntry3 == null)
					{
						if (OABVariantConfigurationSettings.IsSkipServiceTopologyDiscoveryEnabled)
						{
							adrawEntry3 = HttpProxyBackEndHelper.GetOrganizationMailbox(organizationId, OrganizationCapability.OABGen, null);
						}
						else
						{
							adrawEntry3 = HttpProxyBackEndHelper.GetOrganizationMailboxInClosestSite(organizationId, OrganizationCapability.OABGen);
						}
					}
					if (adrawEntry3 != null)
					{
						anchorMailbox = new UserADRawEntryAnchorMailbox(adrawEntry3, this)
						{
							UseServerCookie = true
						};
					}
				}
			}
			if (anchorMailbox == null)
			{
				ExTraceGlobals.VerboseTracer.TraceError(0L, "[OabProxyRequestHandler::ResolveAnchorMailbox] Unable to locate appropriate server for OAB");
				string message;
				if (string.IsNullOrEmpty(targetOrgMailbox))
				{
					message = string.Format("Unable to locate appropriate server for OAB {0}.", guid);
				}
				else
				{
					message = string.Format("Unable to locate organization mailbox {0}", targetOrgMailbox);
				}
				throw new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.OrganizationMailboxNotFound, message);
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailbox>(0L, "[OabProxyRequestHandler::ResolveAnchorMailbox] Proxying OAB request using anchor {0}.", anchorMailbox);
			string value = string.Format("{0}-{1}", base.Logger.Get(HttpProxyMetadata.RoutingHint), "OABOrgMailbox");
			base.Logger.Set(HttpProxyMetadata.RoutingHint, value);
			anchorMailbox.OriginalAnchorMailbox = anchorMailbox2;
			return anchorMailbox;
		}

		protected override BackEndServer GetDownLevelClientAccessServer(AnchorMailbox anchorMailbox, BackEndServer backEndServer)
		{
			return backEndServer;
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			base.AddProtocolSpecificHeadersToServerRequest(headers);
			OAuthIdentity oauthIdentity = base.HttpContext.User.Identity as OAuthIdentity;
			if (oauthIdentity != null)
			{
				if (oauthIdentity.IsAppOnly)
				{
					throw new HttpException(403, "unsupported scenario");
				}
				if (oauthIdentity.OrganizationId.OrganizationalUnit != null)
				{
					headers[WellKnownHeader.WLIDMemberName] = "dummy@" + oauthIdentity.OrganizationId.OrganizationalUnit.Name;
				}
			}
		}

		private AnchorMailbox GetE14CASServer(OABCache.OABCacheEntry oab)
		{
			ServiceTopology serviceTopology = ServiceTopology.GetCurrentLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\RequestHandlers\\OabProxyRequestHandler.cs", "GetE14CASServer", 287);
			Site currentSite = HttpProxyGlobals.LocalSite.Member;
			List<OabService> cheapestCASServers = new List<OabService>();
			int cheapestSiteConnectionCost = int.MaxValue;
			OabProxyRequestHandler.IsEligibleOabService isEligibleOabServiceDelegate = null;
			if (oab.GlobalWebDistributionEnabled)
			{
				isEligibleOabServiceDelegate = new OabProxyRequestHandler.IsEligibleOabService(this.IsEligibleOabServiceBasedOnVersion);
			}
			else
			{
				if (oab.VirtualDirectories == null || oab.VirtualDirectories.Count <= 0)
				{
					ExTraceGlobals.VerboseTracer.TraceError(0L, "[OabProxyRequestHandler::ResolveAnchorMailbox] The OAB is distributed neither globally nor to named vdirs; there is no way to retrieve it");
					throw new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.ServerNotFound, "The OAB is distributed neither globally nor to named vdirs; there is no way to retrieve it");
				}
				isEligibleOabServiceDelegate = new OabProxyRequestHandler.IsEligibleOabService(this.IsEligibleOabServiceBasedOnVersionAndVirtualDirectory);
			}
			serviceTopology.ForEach<OabService>(delegate(OabService oabService)
			{
				if (isEligibleOabServiceDelegate(oabService, oab))
				{
					int maxValue = int.MaxValue;
					if (currentSite != null && oabService.Site != null)
					{
						serviceTopology.TryGetConnectionCost(currentSite, oabService.Site, out maxValue, "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\RequestHandlers\\OabProxyRequestHandler.cs", "GetE14CASServer", 325);
					}
					if (maxValue == cheapestSiteConnectionCost)
					{
						cheapestCASServers.Add(oabService);
						return;
					}
					if (maxValue < cheapestSiteConnectionCost)
					{
						cheapestCASServers.Clear();
						cheapestCASServers.Add(oabService);
						cheapestSiteConnectionCost = maxValue;
					}
				}
			}, "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\RequestHandlers\\OabProxyRequestHandler.cs", "GetE14CASServer", 316);
			if (cheapestCASServers.Count == 0)
			{
				ExTraceGlobals.VerboseTracer.TraceError(0L, "[OabProxyRequestHandler::ResolveAnchorMailbox] Could not find a valid downlevel CAS server for this OAB");
				throw new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.ServerNotFound, "Could not find a valid downlevel CAS server for this OAB");
			}
			OabService oabService2;
			if (cheapestCASServers.Count == 1)
			{
				oabService2 = cheapestCASServers[0];
			}
			else
			{
				oabService2 = cheapestCASServers[OabProxyRequestHandler.RandomNumberGenerator.Next(cheapestCASServers.Count)];
			}
			BackEndServer backendServer = new BackEndServer(oabService2.ServerFullyQualifiedDomainName, oabService2.ServerVersionNumber);
			return new ServerInfoAnchorMailbox(backendServer, this);
		}

		private bool IsEligibleOabServiceBasedOnVersion(OabService oabService, OABCache.OABCacheEntry oabCacheEntry)
		{
			bool result = false;
			if (oabService != null && !oabService.IsOutOfService && oabService.ServerVersionNumber < Server.E15MinVersion)
			{
				result = true;
			}
			return result;
		}

		private bool IsEligibleOabServiceBasedOnVersionAndVirtualDirectory(OabService oabService, OABCache.OABCacheEntry oabCacheEntry)
		{
			bool result = false;
			if (oabService != null && !oabService.IsOutOfService && oabService.ServerVersionNumber < Server.E15MinVersion)
			{
				foreach (ADObjectId x in oabCacheEntry.VirtualDirectories)
				{
					if (ADObjectId.Equals(x, oabService.ADObjectId))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private static readonly Random RandomNumberGenerator = new Random();

		private delegate bool IsEligibleOabService(OabService oabService, OABCache.OABCacheEntry oabCacheEntry);
	}
}
