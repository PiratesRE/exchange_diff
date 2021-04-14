using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class StorageGroupIdParameter : ServerBasedIdParameter
	{
		public StorageGroupIdParameter()
		{
		}

		public StorageGroupIdParameter(StorageGroup group) : base(group.Id)
		{
		}

		public StorageGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public StorageGroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected StorageGroupIdParameter(string identity) : base(identity)
		{
		}

		protected override ServerRole RoleRestriction
		{
			get
			{
				return ServerRole.Mailbox;
			}
		}

		public static StorageGroupIdParameter Parse(string identity)
		{
			return new StorageGroupIdParameter(identity);
		}

		protected override void Initialize(string identity)
		{
			base.Initialize(identity);
			if (!string.IsNullOrEmpty(base.ServerName) && base.ServerId == null)
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "Identity");
			}
		}
	}
}
