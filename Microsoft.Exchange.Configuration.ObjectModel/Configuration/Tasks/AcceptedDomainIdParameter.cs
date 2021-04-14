using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AcceptedDomainIdParameter : ADIdParameter
	{
		public AcceptedDomainIdParameter()
		{
		}

		public AcceptedDomainIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		protected AcceptedDomainIdParameter(string identity) : base(identity)
		{
		}

		public AcceptedDomainIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static AcceptedDomainIdParameter Parse(string identity)
		{
			return new AcceptedDomainIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(AcceptedDomain))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
		}
	}
}
