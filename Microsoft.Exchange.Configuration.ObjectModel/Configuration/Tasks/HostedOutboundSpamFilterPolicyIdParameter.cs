using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class HostedOutboundSpamFilterPolicyIdParameter : ADIdParameter
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		public HostedOutboundSpamFilterPolicyIdParameter()
		{
		}

		public HostedOutboundSpamFilterPolicyIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		public HostedOutboundSpamFilterPolicyIdParameter(string identity) : base(identity)
		{
		}

		public HostedOutboundSpamFilterPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static HostedOutboundSpamFilterPolicyIdParameter Parse(string identity)
		{
			return new HostedOutboundSpamFilterPolicyIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(HostedOutboundSpamFilterPolicy))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			return base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
		}
	}
}
