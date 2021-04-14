using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ActiveSyncDeviceAccessRuleIdParameter : ADIdParameter
	{
		public ActiveSyncDeviceAccessRuleIdParameter()
		{
		}

		public ActiveSyncDeviceAccessRuleIdParameter(string rawString) : base(rawString)
		{
		}

		public ActiveSyncDeviceAccessRuleIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ActiveSyncDeviceAccessRuleIdParameter(ActiveSyncDeviceAccessRule settings) : base(settings.Id)
		{
		}

		public ActiveSyncDeviceAccessRuleIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ActiveSyncDeviceAccessRuleIdParameter Parse(string rawString)
		{
			return new ActiveSyncDeviceAccessRuleIdParameter(rawString);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(ActiveSyncDeviceAccessRule))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
		}
	}
}
