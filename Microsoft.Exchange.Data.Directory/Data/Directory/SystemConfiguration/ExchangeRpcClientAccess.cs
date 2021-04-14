using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class ExchangeRpcClientAccess : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ExchangeRpcClientAccess.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchProtocolCfgExchangeRpcService";
			}
		}

		public ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[ExchangeRpcClientAccessSchema.Server];
			}
		}

		[Parameter(Mandatory = false)]
		public int MaximumConnections
		{
			get
			{
				return (int)this[ExchangeRpcClientAccessSchema.MaximumConnections];
			}
			set
			{
				this[ExchangeRpcClientAccessSchema.MaximumConnections] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EncryptionRequired
		{
			get
			{
				return (bool)this[ExchangeRpcClientAccessSchema.IsEncryptionRequired];
			}
			set
			{
				this[ExchangeRpcClientAccessSchema.IsEncryptionRequired] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BlockedClientVersions
		{
			get
			{
				return (string)this[ExchangeRpcClientAccessSchema.BlockedClientVersions];
			}
			set
			{
				this[ExchangeRpcClientAccessSchema.BlockedClientVersions] = value;
			}
		}

		public RpcClientAccessResponsibility Responsibility { get; private set; }

		public override string ToString()
		{
			if (this.Server == null)
			{
				return base.ToString();
			}
			return this.Server.ToString();
		}

		internal static bool CanCreateUnder(Server server)
		{
			return server.IsClientAccessServer || server.IsMailboxServer;
		}

		internal static LegacyDN CreateSelfRedirectLegacyDN(LegacyDN legacyDN, Guid mailboxGuid)
		{
			string rdnPrefix;
			string str;
			LegacyDN parentLegacyDN = legacyDN.GetParentLegacyDN(out rdnPrefix, out str);
			return parentLegacyDN.GetChildLegacyDN("cn", ExchangeRpcClientAccess.selfRedirectLegacyDNSectionPrefix + mailboxGuid.ToString()).GetChildLegacyDN(rdnPrefix, ExchangeRpcClientAccess.selfRedirectLegacyDNServerPrefix + str);
		}

		internal static string CreatePersonalizedServer(Guid mailboxGuid, string domain)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}@{1}", new object[]
			{
				mailboxGuid.ToString("D"),
				domain
			}).ToLowerInvariant();
		}

		internal static LegacyDN CreatePersonalizedServerRedirectLegacyDN(LegacyDN legacyDN, Guid mailboxGuid, string domain)
		{
			LegacyDN parentLegacyDN = legacyDN.GetParentLegacyDN();
			return parentLegacyDN.GetChildLegacyDN("cn", ExchangeRpcClientAccess.CreatePersonalizedServer(mailboxGuid, domain));
		}

		internal static string FixFakeRedirectLegacyDNIfNeeded(string legacyDN)
		{
			LegacyDN legacyDN2;
			if (!LegacyDN.TryParse(legacyDN, out legacyDN2))
			{
				return legacyDN;
			}
			return ExchangeRpcClientAccess.FixFakeRedirectLegacyDNIfNeeded(legacyDN2).ToString();
		}

		internal static LegacyDN FixFakeRedirectLegacyDNIfNeeded(LegacyDN legacyDN)
		{
			try
			{
				string rdnPrefix;
				string text;
				LegacyDN parentLegacyDN = legacyDN.GetParentLegacyDN(out rdnPrefix, out text);
				string text2;
				string text3;
				LegacyDN parentLegacyDN2 = parentLegacyDN.GetParentLegacyDN(out text2, out text3);
				if (text3 != null && text3.StartsWith(ExchangeRpcClientAccess.selfRedirectLegacyDNSectionPrefix, StringComparison.OrdinalIgnoreCase) && text != null && text.StartsWith(ExchangeRpcClientAccess.selfRedirectLegacyDNServerPrefix, StringComparison.OrdinalIgnoreCase))
				{
					return parentLegacyDN2.GetChildLegacyDN(rdnPrefix, text.Substring(ExchangeRpcClientAccess.selfRedirectLegacyDNServerPrefix.Length));
				}
			}
			catch (FormatException)
			{
			}
			return legacyDN;
		}

		internal static ADObjectId FromServerId(ADObjectId serverId)
		{
			return serverId.GetChildId("Protocols").GetChildId("RpcClientAccess");
		}

		internal static ExchangeRpcClientAccess[] GetAll(ITopologyConfigurationSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			IEnumerable<ExchangeRpcClientAccess> source = session.FindPaged<ExchangeRpcClientAccess>(null, QueryScope.SubTree, null, null, 0);
			return source.ToArray<ExchangeRpcClientAccess>();
		}

		internal static IEnumerable<Server> GetAllPossibleServers(ITopologyConfigurationSession session, ADObjectId siteId)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return from server in session.FindPaged<Server>(null, QueryScope.SubTree, QueryFilter.AndTogether(new QueryFilter[]
			{
				new BitMaskOrFilter(ServerSchema.CurrentServerRole, 6UL),
				(siteId != null) ? new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, siteId) : null
			}), null, ADGenericPagedReader<Microsoft.Exchange.Data.Directory.SystemConfiguration.Server>.DefaultPageSize)
			where server.IsE14OrLater
			select server;
		}

		internal static IEnumerable<KeyValuePair<Server, ExchangeRpcClientAccess>> GetServersWithRpcClientAccessEnabled(IEnumerable<Server> cachedServers, IEnumerable<ExchangeRpcClientAccess> cachedRpcClientAccess)
		{
			if (cachedServers == null)
			{
				throw new ArgumentNullException("cachedServers");
			}
			if (cachedRpcClientAccess == null)
			{
				throw new ArgumentNullException("cachedRpcClientAccess");
			}
			Dictionary<string, ExchangeRpcClientAccess> serverIdToRca = cachedRpcClientAccess.ToDictionary((ExchangeRpcClientAccess rca) => rca.Server.DistinguishedName);
			foreach (Server server in cachedServers)
			{
				ExchangeRpcClientAccess rpcClientAccess;
				if (serverIdToRca.TryGetValue(server.Id.DistinguishedName, out rpcClientAccess))
				{
					rpcClientAccess.CompleteAllCalculatedProperties(server);
					yield return new KeyValuePair<Server, ExchangeRpcClientAccess>(server, rpcClientAccess);
				}
			}
			yield break;
		}

		internal static object ServerGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			if (adobjectId == null)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Server", string.Empty), ADObjectSchema.Id, adobjectId));
			}
			object result;
			try
			{
				result = adobjectId.DescendantDN(8);
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Server", ex.Message), ADObjectSchema.Id, adobjectId), ex);
			}
			return result;
		}

		internal void CompleteAllCalculatedProperties(Server cachedServer)
		{
			this.Responsibility = this.GetResponsibility(cachedServer);
		}

		internal RpcClientAccessResponsibility GetResponsibility(Server cachedServer)
		{
			if (cachedServer == null)
			{
				throw new ArgumentNullException("cachedServer");
			}
			if (!cachedServer.Id.Equals(this.Server))
			{
				throw new ArgumentException("Must be the server that this protocol node corresponds to", "server");
			}
			return (cachedServer.IsClientAccessServer ? RpcClientAccessResponsibility.Mailboxes : RpcClientAccessResponsibility.None) | (cachedServer.IsMailboxServer ? RpcClientAccessResponsibility.PublicFolders : RpcClientAccessResponsibility.None);
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		public const string CommonName = "RpcClientAccess";

		private const string MostDerivedClassInternal = "msExchProtocolCfgExchangeRpcService";

		private static readonly ExchangeRpcClientAccessSchema schema = ObjectSchema.GetInstance<ExchangeRpcClientAccessSchema>();

		private static readonly string selfRedirectLegacyDNSectionPrefix = "Instance-";

		private static readonly string selfRedirectLegacyDNServerPrefix = "X";
	}
}
