using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ActiveSyncDeviceClassIdParameter : ADIdParameter
	{
		public ActiveSyncDeviceClassIdParameter()
		{
		}

		public ActiveSyncDeviceClassIdParameter(string rawString) : base(rawString)
		{
		}

		public ActiveSyncDeviceClassIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ActiveSyncDeviceClassIdParameter(ActiveSyncDeviceClass settings) : base(settings.Id)
		{
		}

		public ActiveSyncDeviceClassIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ActiveSyncDeviceClassIdParameter Parse(string rawString)
		{
			return new ActiveSyncDeviceClassIdParameter(rawString);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(ActiveSyncDeviceClass))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
		}
	}
}
