using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ClientAccessServerIdParameter : RoleServerIdParameter
	{
		public ClientAccessServerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ClientAccessServerIdParameter(ClientAccessServer caServer) : base(caServer.Id)
		{
		}

		public ClientAccessServerIdParameter(ExchangeRpcClientAccess rpcClientAccess) : base(rpcClientAccess.Server)
		{
		}

		public ClientAccessServerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ClientAccessServerIdParameter()
		{
		}

		protected ClientAccessServerIdParameter(string identity) : base(identity)
		{
		}

		protected override ServerRole RoleRestriction
		{
			get
			{
				throw new NotImplementedException("DEV BUG, this method should not be invoked.");
			}
		}

		internal override IEnumerableFilter<T> GetEnumerableFilter<T>()
		{
			return new ClientAccessServerIdParameter.E15CafeOrE14CASFilter<T>();
		}

		public new static ClientAccessServerIdParameter Parse(string identity)
		{
			return new ClientAccessServerIdParameter(identity);
		}

		private class E15CafeOrE14CASFilter<T> : IEnumerableFilter<T>
		{
			public bool AcceptElement(T element)
			{
				if (element == null)
				{
					return false;
				}
				Server server = element as Server;
				return server != null && (server.IsCafeServer || (server.IsClientAccessServer && server.VersionNumber < Server.E15MinVersion));
			}
		}
	}
}
