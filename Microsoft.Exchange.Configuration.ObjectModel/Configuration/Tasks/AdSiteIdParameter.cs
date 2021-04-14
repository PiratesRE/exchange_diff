using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AdSiteIdParameter : ADIdParameter
	{
		public AdSiteIdParameter()
		{
		}

		public AdSiteIdParameter(string identity) : base(identity)
		{
		}

		public AdSiteIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public AdSiteIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public AdSiteIdParameter(ADSite site) : base(site.Id)
		{
		}

		public static AdSiteIdParameter Parse(string identity)
		{
			return new AdSiteIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(ADSite))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return base.GetObjects<T>(session.GetConfigurationNamingContext().GetChildId("Sites"), session, subTreeSession, optionalData, out notFoundReason);
		}
	}
}
