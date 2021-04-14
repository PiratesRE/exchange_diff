using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ServerIdParameter : ADIdParameter
	{
		public ServerIdParameter(Fqdn fqdn) : this(fqdn.ToString())
		{
		}

		public ServerIdParameter(MailboxServer server) : base(server.Id)
		{
		}

		public ServerIdParameter(ExchangeServer exServer) : base(exServer.Id)
		{
		}

		public ServerIdParameter(ClientAccessServer caServer) : base(caServer.Id)
		{
		}

		public ServerIdParameter(UMServer umServer) : base(umServer.Id)
		{
		}

		public ServerIdParameter(TransportServer trServer) : base(trServer.Id)
		{
		}

		public ServerIdParameter(FrontendTransportServerPresentationObject frontendTransportServer) : base(frontendTransportServer.Id.Parent.Parent.Name)
		{
		}

		public ServerIdParameter(MailboxTransportServerPresentationObject mailboxTransportServer) : base(mailboxTransportServer.Id.Parent.Parent.Name)
		{
		}

		public ServerIdParameter(MalwareFilteringServer mfServer) : base(mfServer.Id)
		{
		}

		public ServerIdParameter(ExchangeRpcClientAccess rpcClientAccess) : base(rpcClientAccess.Server)
		{
		}

		public ServerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ServerIdParameter() : this(ServerIdParameter.LocalServerFQDN)
		{
		}

		public ServerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		protected ServerIdParameter(string identity) : base(identity)
		{
			if (base.InternalADObjectId != null)
			{
				return;
			}
			LegacyDN legacyDN;
			if (!ADObjectNameHelper.ReservedADNameStringRegex.IsMatch(identity) && !ServerIdParameter.IsValidName(identity) && !ServerIdParameter.IsValidFqdn(identity) && !LegacyDN.TryParse(identity, out legacyDN))
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "identity");
			}
			this.fqdn = identity;
			if (identity.EndsWith(".", StringComparison.Ordinal))
			{
				this.fqdn = identity.Substring(0, identity.Length - 1);
			}
		}

		internal string Fqdn
		{
			get
			{
				return this.fqdn;
			}
		}

		private static string LocalServerFQDN
		{
			get
			{
				return NativeHelpers.GetLocalComputerFqdn(true);
			}
		}

		public static ServerIdParameter Parse(string identity)
		{
			return new ServerIdParameter(identity);
		}

		internal static void ClearServerRoleCache()
		{
			lock (ServerIdParameter.serverRoleCache)
			{
				ServerIdParameter.serverRoleCache.Clear();
			}
		}

		internal static bool HasRole(ADObjectId identity, ServerRole role, IConfigDataProvider session)
		{
			ServerIdParameter serverIdParameter = new ServerIdParameter(identity.DescendantDN(8));
			ServerInfo[] serverInfo = serverIdParameter.GetServerInfo(session);
			return serverInfo != null && serverInfo.Length == 1 && (serverInfo[0].Role & role) != ServerRole.None;
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(Server) && typeof(T) != typeof(MiniServer) && typeof(T) != typeof(MiniClientAccessServerOrArray))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
			if (!wrapper.HasElements() && this.Fqdn != null)
			{
				QueryFilter filter = new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.Fqdn, this.Fqdn),
					new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ExchangeLegacyDN, this.Fqdn)
				});
				wrapper = EnumerableWrapper<T>.GetWrapper(base.PerformPrimarySearch<T>(filter, rootId, session, true, optionalData));
			}
			if (typeof(T) == typeof(Server))
			{
				List<T> list = new List<T>();
				foreach (T t in wrapper)
				{
					list.Add(t);
					string key = ((ADObjectId)t.Identity).ToDNString().ToLowerInvariant();
					Server server = (Server)((object)t);
					ServerInfo serverInfo = new ServerInfo();
					serverInfo.Identity = server.Id;
					serverInfo.Role = server.CurrentServerRole;
					lock (ServerIdParameter.serverRoleCache)
					{
						ServerIdParameter.serverRoleCache[key] = serverInfo;
						ServerIdParameter.serverRoleCache[server.Name.ToLowerInvariant()] = serverInfo;
						ServerIdParameter.serverRoleCache[server.Fqdn.ToLowerInvariant()] = serverInfo;
					}
				}
				return list;
			}
			return wrapper;
		}

		internal ADObjectId[] GetMatchingIdentities(IConfigDataProvider session)
		{
			ServerInfo[] serverInfo = this.GetServerInfo(session);
			ADObjectId[] array = new ADObjectId[serverInfo.Length];
			for (int i = 0; i < serverInfo.Length; i++)
			{
				array[i] = serverInfo[i].Identity;
			}
			return array;
		}

		private static bool IsValidName(string nameString)
		{
			return Regex.IsMatch(nameString, "^[^`~!@#&\\^\\(\\)\\+\\[\\]\\{\\}\\<\\>\\?=,:|./\\\\; ]+$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
		}

		private static bool IsValidFqdn(string fqdnString)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(fqdnString))
			{
				if (fqdnString.EndsWith("."))
				{
					fqdnString = fqdnString.Substring(0, fqdnString.Length - 1);
				}
				string[] array = fqdnString.Split(new char[]
				{
					'.'
				});
				result = (array.Length > 1);
				foreach (string nameString in array)
				{
					if (!ServerIdParameter.IsValidName(nameString))
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		private ServerInfo[] GetServerInfo(IConfigDataProvider session)
		{
			string text;
			if (base.InternalADObjectId != null && !string.IsNullOrEmpty(base.InternalADObjectId.DistinguishedName))
			{
				text = base.InternalADObjectId.ToDNString();
			}
			else
			{
				text = base.RawIdentity;
			}
			text = text.ToLowerInvariant();
			lock (ServerIdParameter.serverRoleCache)
			{
				if (ServerIdParameter.serverRoleCache.ContainsKey(text))
				{
					return new ServerInfo[]
					{
						ServerIdParameter.serverRoleCache[text]
					};
				}
			}
			IEnumerable<Server> objects = base.GetObjects<Server>(null, session);
			List<ServerInfo> list = new List<ServerInfo>();
			foreach (Server server in objects)
			{
				text = ((ADObjectId)server.Identity).ToDNString().ToLowerInvariant();
				lock (ServerIdParameter.serverRoleCache)
				{
					list.Add(ServerIdParameter.serverRoleCache[text]);
				}
			}
			return list.ToArray();
		}

		private const int ServerDepth = 8;

		private static Dictionary<string, ServerInfo> serverRoleCache = new Dictionary<string, ServerInfo>();

		private string fqdn;
	}
}
