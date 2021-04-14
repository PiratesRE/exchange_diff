using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ActiveSyncOrganizationSettingsIdParameter : ADIdParameter
	{
		public ActiveSyncOrganizationSettingsIdParameter()
		{
		}

		public ActiveSyncOrganizationSettingsIdParameter(string rawString) : base(rawString)
		{
		}

		public ActiveSyncOrganizationSettingsIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ActiveSyncOrganizationSettingsIdParameter(ActiveSyncOrganizationSettings settings) : base(settings.Id)
		{
		}

		public ActiveSyncOrganizationSettingsIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ActiveSyncOrganizationSettingsIdParameter Parse(string rawString)
		{
			return new ActiveSyncOrganizationSettingsIdParameter(rawString);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(ActiveSyncOrganizationSettings))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
		}
	}
}
