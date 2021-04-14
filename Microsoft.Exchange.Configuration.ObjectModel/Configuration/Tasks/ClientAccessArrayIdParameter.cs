using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ClientAccessArrayIdParameter : ADIdParameter
	{
		public ClientAccessArrayIdParameter(Fqdn fqdn) : this(fqdn.ToString())
		{
		}

		public ClientAccessArrayIdParameter(ClientAccessArray clientAccessArray) : this(clientAccessArray.Id)
		{
		}

		public ClientAccessArrayIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ClientAccessArrayIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ClientAccessArrayIdParameter()
		{
		}

		protected ClientAccessArrayIdParameter(string identity) : base(identity)
		{
			if (base.InternalADObjectId != null)
			{
				return;
			}
			this.fqdn = ServerIdParameter.Parse(identity).Fqdn;
		}

		public static ClientAccessArrayIdParameter Parse(string identity)
		{
			return new ClientAccessArrayIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(ClientAccessArray))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
			if (!wrapper.HasElements() && this.fqdn != null)
			{
				QueryFilter filter = new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ClientAccessArraySchema.Fqdn, this.fqdn),
					new ComparisonFilter(ComparisonOperator.Equal, ClientAccessArraySchema.ExchangeLegacyDN, this.fqdn)
				});
				wrapper = EnumerableWrapper<T>.GetWrapper(base.PerformPrimarySearch<T>(filter, rootId, session, true, optionalData));
			}
			return wrapper;
		}

		private string fqdn;
	}
}
