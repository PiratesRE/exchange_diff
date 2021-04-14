using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ServiceInstanceIdParameter : ADIdParameter
	{
		public ServiceInstanceIdParameter()
		{
		}

		public ServiceInstanceIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		protected ServiceInstanceIdParameter(string identity) : base(identity)
		{
		}

		public ServiceInstanceIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ServiceInstanceIdParameter(ServiceInstanceId serviceInstanceId) : base(serviceInstanceId.InstanceId)
		{
		}

		public ServiceInstanceIdParameter(SyncServiceInstance syncServiceInstance) : base(syncServiceInstance.Name)
		{
		}

		public static ServiceInstanceIdParameter Parse(string identity)
		{
			return new ServiceInstanceIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(SyncServiceInstance))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
		}
	}
}
