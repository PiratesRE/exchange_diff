using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ActiveSyncDeviceIdParameter : MobileDeviceIdParameter
	{
		public ActiveSyncDeviceIdParameter()
		{
		}

		public ActiveSyncDeviceIdParameter(string identity) : base(identity)
		{
		}

		public ActiveSyncDeviceIdParameter(ADObjectId objectId) : base(objectId)
		{
		}

		public ActiveSyncDeviceIdParameter(ActiveSyncDevice device) : base(device)
		{
		}

		public ActiveSyncDeviceIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public new static ActiveSyncDeviceIdParameter Parse(string identity)
		{
			return new ActiveSyncDeviceIdParameter(identity);
		}
	}
}
