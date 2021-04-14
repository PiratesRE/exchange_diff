using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class HostedConnectionFilterPolicyIdParameter : ADIdParameter
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		public HostedConnectionFilterPolicyIdParameter()
		{
		}

		public HostedConnectionFilterPolicyIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		public HostedConnectionFilterPolicyIdParameter(string identity) : base(identity)
		{
		}

		public HostedConnectionFilterPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static HostedConnectionFilterPolicyIdParameter Parse(string identity)
		{
			return new HostedConnectionFilterPolicyIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(HostedConnectionFilterPolicy))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
		}
	}
}
