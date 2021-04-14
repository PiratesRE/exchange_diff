using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ActiveSyncDeviceAutoblockThresholdIdParameter : ADIdParameter
	{
		public ActiveSyncDeviceAutoblockThresholdIdParameter()
		{
		}

		public ActiveSyncDeviceAutoblockThresholdIdParameter(string rawString) : base(rawString)
		{
		}

		public ActiveSyncDeviceAutoblockThresholdIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ActiveSyncDeviceAutoblockThresholdIdParameter(ActiveSyncDeviceAutoblockThreshold settings) : base(settings.Id)
		{
		}

		public ActiveSyncDeviceAutoblockThresholdIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ActiveSyncDeviceAutoblockThresholdIdParameter Parse(string rawString)
		{
			return new ActiveSyncDeviceAutoblockThresholdIdParameter(rawString);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(ActiveSyncDeviceAutoblockThreshold))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
		}
	}
}
