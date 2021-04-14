using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RemoteDiscoveryEndPoint
	{
		public static bool TryGetDiscoveryEndPoint(OrganizationId orgId, string crossPremiseDomain, Func<OrganizationId, OrganizationIdCacheValue> getOrgIdCacheValue, Func<OrganizationIdCacheValue, string, IntraOrganizationConnector> getIntraOrganizationConnector, Func<OrganizationIdCacheValue, string, OrganizationRelationship> getOrganizationRelationShip, out Uri discoveryEndPoint, out EndPointDiscoveryInfo info)
		{
			discoveryEndPoint = null;
			info = new EndPointDiscoveryInfo();
			if (orgId == null)
			{
				info.AddInfo(EndPointDiscoveryInfo.DiscoveryStatus.Error, "orgId is null");
				return false;
			}
			if (string.IsNullOrEmpty(crossPremiseDomain))
			{
				info.AddInfo(EndPointDiscoveryInfo.DiscoveryStatus.Error, "crossPremiseDomain is invalid");
				return false;
			}
			try
			{
				OrganizationIdCacheValue organizationIdCacheValue = null;
				if (getOrgIdCacheValue == null)
				{
					organizationIdCacheValue = OrganizationIdCache.Singleton.Get(orgId);
				}
				else
				{
					organizationIdCacheValue = getOrgIdCacheValue(orgId);
				}
				IntraOrganizationConnector intraOrganizationConnector = null;
				try
				{
					if (getIntraOrganizationConnector == null)
					{
						if (organizationIdCacheValue == null)
						{
							info.AddInfo(EndPointDiscoveryInfo.DiscoveryStatus.Error, string.Format("OrganizationIdCacheValue == null. OrgID=[{0}], domain=[{1}]. getOrgIdCacheValue is{2} null.", orgId.ToExternalDirectoryOrganizationId(), crossPremiseDomain, (getOrgIdCacheValue == null) ? string.Empty : " not"));
							return false;
						}
						intraOrganizationConnector = organizationIdCacheValue.GetIntraOrganizationConnector(crossPremiseDomain);
					}
					else
					{
						intraOrganizationConnector = getIntraOrganizationConnector(organizationIdCacheValue, crossPremiseDomain);
					}
				}
				catch (Exception ex)
				{
					info.AddInfo(EndPointDiscoveryInfo.DiscoveryStatus.IocException, ex.ToString());
				}
				if (intraOrganizationConnector == null)
				{
					string message = string.Format("IntraOrganizationConnector lookup for org [{0}], domain [{1}] found nothing. getIntraOrganizationConnector is{2} null.", orgId.ToExternalDirectoryOrganizationId(), crossPremiseDomain, (getIntraOrganizationConnector == null) ? string.Empty : " not");
					info.AddInfo((info.Status == EndPointDiscoveryInfo.DiscoveryStatus.Success) ? EndPointDiscoveryInfo.DiscoveryStatus.IocNotFound : info.Status, message);
				}
				else
				{
					if (!(intraOrganizationConnector.DiscoveryEndpoint == null))
					{
						ExTraceGlobals.ServiceDiscoveryTracer.TraceDebug(0L, "IntraOrganizationConnector lookup for org [{0}], domain [{1}] found [{2}]. End point=[{3}].", new object[]
						{
							orgId.ToExternalDirectoryOrganizationId(),
							crossPremiseDomain,
							intraOrganizationConnector.Id,
							intraOrganizationConnector.DiscoveryEndpoint
						});
						discoveryEndPoint = intraOrganizationConnector.DiscoveryEndpoint;
						return true;
					}
					info.AddInfo(EndPointDiscoveryInfo.DiscoveryStatus.IocNoUri, string.Format("IntraOrganizationConnector lookup for org [{0}], domain [{1}] found [{2}], but it doesn't have DiscoveryEndpoint set. getIntraOrganizationConnector is{3} null.", new object[]
					{
						orgId.ToExternalDirectoryOrganizationId(),
						crossPremiseDomain,
						intraOrganizationConnector.Id,
						(getIntraOrganizationConnector == null) ? string.Empty : " not"
					}));
				}
				OrganizationRelationship organizationRelationship;
				if (getOrganizationRelationShip == null)
				{
					if (organizationIdCacheValue == null)
					{
						info.AddInfo(EndPointDiscoveryInfo.DiscoveryStatus.Error, string.Format("OrganizationIdCacheValue is null. OrgID=[{0}], domain=[{1}]. getOrgIdCacheValue is{2} null.", orgId.ToExternalDirectoryOrganizationId(), crossPremiseDomain, (getOrgIdCacheValue == null) ? string.Empty : " not"));
						return false;
					}
					organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(crossPremiseDomain);
				}
				else
				{
					organizationRelationship = getOrganizationRelationShip(organizationIdCacheValue, crossPremiseDomain);
				}
				if (organizationRelationship == null)
				{
					info.AddInfo(EndPointDiscoveryInfo.DiscoveryStatus.OrNotFound, string.Format("Unable to find the org relationship for OrgID=[{0}], domain=[{1}]. getOrganizationRelationShip is{2} null.", orgId.ToExternalDirectoryOrganizationId(), crossPremiseDomain, (getOrganizationRelationShip == null) ? string.Empty : " not"));
					return false;
				}
				if (organizationRelationship.TargetAutodiscoverEpr == null)
				{
					info.AddInfo(EndPointDiscoveryInfo.DiscoveryStatus.OrNoUri, string.Format("The TargetAutodiscoverEpr in org relationship is null for OrgID=[{0}], domain=[{1}]. getOrganizationRelationShip is{2} null.", orgId.ToExternalDirectoryOrganizationId(), crossPremiseDomain, (getOrganizationRelationShip == null) ? string.Empty : " not"));
					return false;
				}
				ExTraceGlobals.ServiceDiscoveryTracer.TraceDebug<string, string, Uri>(0L, "OrganizationRelationship lookup for org [{0}], domain [{1}] found end point: [{2}]", orgId.ToExternalDirectoryOrganizationId(), crossPremiseDomain, organizationRelationship.TargetAutodiscoverEpr);
				discoveryEndPoint = organizationRelationship.TargetAutodiscoverEpr;
			}
			catch (Exception ex2)
			{
				info.AddInfo(EndPointDiscoveryInfo.DiscoveryStatus.Error, ex2.ToString());
				return false;
			}
			return true;
		}
	}
}
