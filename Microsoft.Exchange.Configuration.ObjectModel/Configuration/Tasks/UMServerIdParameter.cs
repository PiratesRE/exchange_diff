using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class UMServerIdParameter : RoleServerIdParameter
	{
		public UMServerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public UMServerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public UMServerIdParameter()
		{
		}

		protected UMServerIdParameter(string identity) : base(identity)
		{
		}

		protected override ServerRole RoleRestriction
		{
			get
			{
				return ServerRole.UnifiedMessaging;
			}
		}

		public new static UMServerIdParameter Parse(string identity)
		{
			return new UMServerIdParameter(identity);
		}
	}
}
