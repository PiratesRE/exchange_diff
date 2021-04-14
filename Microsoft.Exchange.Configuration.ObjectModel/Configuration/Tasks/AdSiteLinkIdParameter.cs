using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AdSiteLinkIdParameter : ADIdParameter
	{
		public AdSiteLinkIdParameter()
		{
		}

		public AdSiteLinkIdParameter(string identity) : base(identity)
		{
		}

		public AdSiteLinkIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public AdSiteLinkIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static AdSiteLinkIdParameter Parse(string identity)
		{
			return new AdSiteLinkIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(ADSiteLink))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return base.GetObjects<T>(session.GetConfigurationNamingContext().GetChildId("Sites"), session, subTreeSession, optionalData, out notFoundReason);
		}
	}
}
