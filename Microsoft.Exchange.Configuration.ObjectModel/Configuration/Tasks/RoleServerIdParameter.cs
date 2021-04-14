using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public abstract class RoleServerIdParameter : ServerIdParameter
	{
		public RoleServerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RoleServerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public RoleServerIdParameter()
		{
		}

		protected RoleServerIdParameter(string identity) : base(identity)
		{
		}

		protected abstract ServerRole RoleRestriction { get; }

		internal override IEnumerableFilter<T> GetEnumerableFilter<T>()
		{
			return ServerRoleFilter<T>.GetServerRoleFilter(this.RoleRestriction);
		}
	}
}
