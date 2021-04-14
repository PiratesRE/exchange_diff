using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ClientAccessArray : ADConfigurationObject
	{
		public bool IsPriorTo15ExchangeObjectVersion
		{
			get
			{
				return base.ExchangeVersion.IsOlderThan(ClientAccessArray.MinimumSupportedExchangeObjectVersion);
			}
		}

		internal static ExchangeObjectVersion MinimumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal static QueryFilter PriorTo15ExchangeObjectVersionFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.LessThan, ADObjectSchema.ExchangeVersion, ClientAccessArray.MinimumSupportedExchangeObjectVersion);
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012.NextMajorVersion;
			}
		}

		internal override QueryFilter VersioningFilter
		{
			get
			{
				ExchangeObjectVersion exchange = ExchangeObjectVersion.Exchange2007;
				ExchangeObjectVersion nextMajorVersion = this.MaximumSupportedExchangeObjectVersion.NextMajorVersion;
				return new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.ExchangeVersion, exchange),
					new ComparisonFilter(ComparisonOperator.LessThan, ADObjectSchema.ExchangeVersion, nextMajorVersion)
				});
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ClientAccessArray.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchClientAccessArray";
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return QueryFilter.OrTogether(new QueryFilter[]
				{
					base.ImplicitFilter,
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "msExchClientAccessArrayLegacy")
				});
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this.propertyBag[ClientAccessArraySchema.ExchangeLegacyDN];
			}
		}

		public string Fqdn
		{
			get
			{
				return (string)this.propertyBag[ClientAccessArraySchema.Fqdn];
			}
		}

		public string ArrayDefinition
		{
			get
			{
				return (string)this.propertyBag[ClientAccessArraySchema.ArrayDefinition];
			}
			set
			{
				this.propertyBag[ClientAccessArraySchema.ArrayDefinition] = value;
			}
		}

		public ADObjectId Site
		{
			get
			{
				return (ADObjectId)this.propertyBag[ClientAccessArraySchema.Site];
			}
			set
			{
				this.propertyBag[ClientAccessArraySchema.Site] = value;
			}
		}

		public string SiteName
		{
			get
			{
				if (this.Site != null)
				{
					return this.Site.Name;
				}
				return null;
			}
		}

		public MultiValuedProperty<ADObjectId> Servers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ClientAccessArraySchema.Servers];
			}
		}

		public int ServerCount
		{
			get
			{
				return (int)(this[ClientAccessArraySchema.ServerCount] ?? 0);
			}
			set
			{
				this[ClientAccessArraySchema.ServerCount] = value;
			}
		}

		public ADObjectId[] Members { get; private set; }

		internal static void FqdnSetter(object value, IPropertyBag propertyBag)
		{
			NetworkAddressCollection networkAddressCollection = ((NetworkAddressCollection)propertyBag[ServerSchema.NetworkAddress]) ?? new NetworkAddressCollection();
			NetworkAddress networkAddress = networkAddressCollection[NetworkProtocol.TcpIP];
			if (networkAddress != null)
			{
				networkAddressCollection.Remove(networkAddress);
			}
			networkAddressCollection.Add(NetworkProtocol.TcpIP.GetNetworkAddress((string)value));
			propertyBag[ServerSchema.NetworkAddress] = networkAddressCollection;
		}

		internal static ADObjectId GetParentContainer(ITopologyConfigurationSession adSession)
		{
			return adSession.GetAdministrativeGroupId().GetChildId("Arrays");
		}

		internal static IEnumerable<KeyValuePair<Server, ExchangeRpcClientAccess>> GetMembers(IEnumerable<Server> cachedServers, IEnumerable<ExchangeRpcClientAccess> cachedRpcClientAccess, ADObjectId siteId)
		{
			if (cachedServers == null)
			{
				throw new ArgumentNullException("cachedServers");
			}
			if (cachedRpcClientAccess == null)
			{
				throw new ArgumentNullException("cachedRpcClientAccess");
			}
			return from pair in ExchangeRpcClientAccess.GetServersWithRpcClientAccessEnabled(from server in cachedServers
			where siteId != null && siteId.Equals(server.ServerSite)
			select server, cachedRpcClientAccess)
			where !pair.Key.IsE15OrLater && (pair.Value.Responsibility & RpcClientAccessResponsibility.Mailboxes) == RpcClientAccessResponsibility.Mailboxes
			select pair;
		}

		internal static object IsClientAccessArrayGetter(IPropertyBag propertyBag)
		{
			foreach (string a in ((MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass]))
			{
				if (a == "msExchClientAccessArray" || a == "msExchClientAccessArrayLegacy")
				{
					return true;
				}
			}
			return false;
		}

		internal void CompleteAllCalculatedProperties(IEnumerable<Server> cachedServers, IEnumerable<ExchangeRpcClientAccess> cachedRpcClientAccess)
		{
			this.Members = (from serverToRca in ClientAccessArray.GetMembers(cachedServers, cachedRpcClientAccess, this.Site)
			select serverToRca.Key.Id).ToArray<ADObjectId>();
		}

		private const string MostDerivedClassInternal = "msExchClientAccessArray";

		private const string MostDerivedClassLegacyInternal = "msExchClientAccessArrayLegacy";

		private static readonly ClientAccessArraySchema schema = ObjectSchema.GetInstance<ClientAccessArraySchema>();
	}
}
