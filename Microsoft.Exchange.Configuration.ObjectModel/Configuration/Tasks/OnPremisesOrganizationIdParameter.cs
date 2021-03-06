using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class OnPremisesOrganizationIdParameter : ADIdParameter
	{
		public OnPremisesOrganizationIdParameter()
		{
		}

		public OnPremisesOrganizationIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		protected OnPremisesOrganizationIdParameter(string identity) : base(identity)
		{
		}

		public OnPremisesOrganizationIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static OnPremisesOrganizationIdParameter Parse(string identity)
		{
			return new OnPremisesOrganizationIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(OnPremisesOrganization))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
		}
	}
}
