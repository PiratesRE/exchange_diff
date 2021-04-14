using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SimpleMiniClientAccessServerOrArrayLookup : IFindMiniClientAccessServerOrArray
	{
		public SimpleMiniClientAccessServerOrArrayLookup(ITopologyConfigurationSession adSession)
		{
			this.AdSession = adSession;
		}

		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ActiveManagerClientTracer;
			}
		}

		private ITopologyConfigurationSession AdSession
		{
			get
			{
				return this.adSession;
			}
			set
			{
				this.adSession = value;
				this.cdsAdSession = ADSessionFactory.CreateWrapper(this.adSession);
			}
		}

		public static IADMiniClientAccessServerOrArray FindMiniCasOrArrayWithClientAccess(IFindMiniClientAccessServerOrArray findMiniCasArray, ITopologyConfigurationSession adSession, ADObjectId siteId, ADObjectId preferredServerId)
		{
			ADObjectId adobjectId = SimpleMiniClientAccessServerOrArrayLookup.FindServerIdWithClientAccess(adSession, siteId, preferredServerId);
			if (adobjectId != null)
			{
				return findMiniCasArray.ReadMiniClientAccessServerOrArrayByObjectId(adobjectId);
			}
			return null;
		}

		public void Clear()
		{
		}

		public IADMiniClientAccessServerOrArray FindMiniClientAccessServerOrArrayByFqdn(string serverFqdn)
		{
			return SimpleMiniClientAccessServerOrArrayLookup.FindMiniCasOrArrayByFqdn(this.cdsAdSession, serverFqdn);
		}

		internal static IADMiniClientAccessServerOrArray FindMiniCasOrArrayByFqdn(IADToplogyConfigurationSession cdsAdSession, string serverFqdn)
		{
			IADMiniClientAccessServerOrArray returnObj = null;
			Exception ex = ADUtils.RunADOperation(delegate()
			{
				returnObj = cdsAdSession.FindMiniClientAccessServerOrArrayByFqdn(serverFqdn);
			}, 2);
			if (ex != null)
			{
				SimpleMiniClientAccessServerOrArrayLookup.Tracer.TraceDebug<Exception>(0L, "SimpleMiniClientAccessServerOrArrayLookup.FindMiniCasOrArrayByFqdn got an exception: {0}", ex);
			}
			return returnObj;
		}

		public IADMiniClientAccessServerOrArray FindMiniClientAccessServerOrArrayByLegdn(string serverLegdn)
		{
			return SimpleMiniClientAccessServerOrArrayLookup.FindMiniCasOrArrayByLegdn(this.cdsAdSession, serverLegdn);
		}

		internal static IADMiniClientAccessServerOrArray FindMiniCasOrArrayByLegdn(IADToplogyConfigurationSession cdsAdSession, string serverLegdn)
		{
			IADMiniClientAccessServerOrArray result = null;
			Exception ex = ADUtils.RunADOperation(delegate()
			{
				bool flag = cdsAdSession.TryFindByExchangeLegacyDN(serverLegdn, out result);
				SimpleMiniClientAccessServerOrArrayLookup.Tracer.TraceDebug<string, bool>(0L, "TryFindByExchangeLegacyDN({0}) returned {1}.", serverLegdn, flag);
				if (!flag)
				{
					SimpleMiniClientAccessServerOrArrayLookup.Tracer.TraceDebug<string>(0L, "FindMiniCasOrArrayByLegdn: Could not find a MiniServer for the legdn extracted server '{0}'.", serverLegdn);
				}
			}, 2);
			if (ex != null)
			{
				SimpleMiniClientAccessServerOrArrayLookup.Tracer.TraceDebug<Exception>(0L, "SimpleMiniClientAccessServerOrArrayLookup.FindMiniCasOrArrayByLegdn got an exception: {0}", ex);
			}
			return result;
		}

		public IADMiniClientAccessServerOrArray ReadMiniClientAccessServerOrArrayByObjectId(ADObjectId serverId)
		{
			IADMiniClientAccessServerOrArray result = null;
			Exception ex = ADUtils.RunADOperation(delegate()
			{
				result = this.cdsAdSession.ReadMiniClientAccessServerOrArray(serverId);
			}, 2);
			if (ex != null)
			{
				SimpleMiniClientAccessServerOrArrayLookup.Tracer.TraceDebug<Exception>((long)this.GetHashCode(), "SimpleMiniClientAccessServerOrArrayLookup.ReadMiniClientAccessServerOrArrayByObjectId got an exception: {0}", ex);
			}
			return result;
		}

		public IADMiniClientAccessServerOrArray FindMiniClientAccessServerOrArrayWithClientAccess(ADObjectId siteId, ADObjectId preferredServerId)
		{
			return SimpleMiniClientAccessServerOrArrayLookup.FindMiniCasOrArrayWithClientAccess(this, this.AdSession, siteId, preferredServerId);
		}

		private static ADObjectId FindServerIdWithClientAccess(ITopologyConfigurationSession adSession, ADObjectId mountedSiteId, ADObjectId preferredServerId)
		{
			ADObjectId adobjectId = null;
			List<ADObjectId> list = new List<ADObjectId>(8);
			foreach (KeyValuePair<Server, ExchangeRpcClientAccess> keyValuePair in ExchangeRpcClientAccess.GetServersWithRpcClientAccessEnabled(ExchangeRpcClientAccess.GetAllPossibleServers(adSession, mountedSiteId), ExchangeRpcClientAccess.GetAll(adSession)))
			{
				if ((keyValuePair.Value.Responsibility & RpcClientAccessResponsibility.Mailboxes) != RpcClientAccessResponsibility.None)
				{
					if (preferredServerId != null && preferredServerId.Equals(keyValuePair.Key.Id))
					{
						adobjectId = keyValuePair.Key.Id;
						break;
					}
					list.Add(keyValuePair.Key.Id);
				}
			}
			if (adobjectId == null && list.Count > 0)
			{
				adobjectId = list[0];
				if (list.Count > 1)
				{
					adobjectId = list[(Environment.TickCount & int.MaxValue) % list.Count];
				}
			}
			return adobjectId;
		}

		private ITopologyConfigurationSession adSession;

		private IADToplogyConfigurationSession cdsAdSession;
	}
}
