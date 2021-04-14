using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ClientAccessServerOrArrayIdParameter : ServerIdParameter
	{
		public ClientAccessServerOrArrayIdParameter(Fqdn fqdn) : this(fqdn.ToString())
		{
		}

		public ClientAccessServerOrArrayIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ClientAccessServerOrArrayIdParameter(ExchangeServer exServer) : base(exServer.Id)
		{
		}

		public ClientAccessServerOrArrayIdParameter(ClientAccessServer caServer) : this(caServer.Id)
		{
		}

		public ClientAccessServerOrArrayIdParameter(ClientAccessArray caArray) : this(caArray.Id)
		{
		}

		public ClientAccessServerOrArrayIdParameter(ExchangeRpcClientAccess rpcClientAccess) : this(rpcClientAccess.Server)
		{
		}

		public ClientAccessServerOrArrayIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ClientAccessServerOrArrayIdParameter()
		{
		}

		protected ClientAccessServerOrArrayIdParameter(string identity) : base(identity)
		{
		}

		public new static ClientAccessServerOrArrayIdParameter Parse(string identity)
		{
			return new ClientAccessServerOrArrayIdParameter(identity);
		}

		internal override IEnumerableFilter<T> GetEnumerableFilter<T>()
		{
			if (typeof(T) != typeof(MiniClientAccessServerOrArray))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return ClientAccessServerOrArrayIdParameter.Filter.Instance as IEnumerableFilter<T>;
		}

		private sealed class Filter : IEnumerableFilter<MiniClientAccessServerOrArray>
		{
			private Filter()
			{
			}

			public bool AcceptElement(MiniClientAccessServerOrArray element)
			{
				return element.IsClientAccessArray || element.IsClientAccessServer;
			}

			public static readonly ClientAccessServerOrArrayIdParameter.Filter Instance = new ClientAccessServerOrArrayIdParameter.Filter();
		}
	}
}
