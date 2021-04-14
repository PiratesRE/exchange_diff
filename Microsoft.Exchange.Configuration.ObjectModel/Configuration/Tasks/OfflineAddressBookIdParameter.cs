using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class OfflineAddressBookIdParameter : ADIdParameter
	{
		public OfflineAddressBookIdParameter()
		{
		}

		public OfflineAddressBookIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public OfflineAddressBookIdParameter(OfflineAddressBook offlineAddresss) : base(offlineAddresss.Id)
		{
		}

		public OfflineAddressBookIdParameter(OfflineAddressBookPresentationObject offlineAddresss) : base(offlineAddresss.Id)
		{
		}

		public OfflineAddressBookIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected OfflineAddressBookIdParameter(string identity) : base(identity)
		{
		}

		public static OfflineAddressBookIdParameter Parse(string identity)
		{
			return new OfflineAddressBookIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
			if (!wrapper.HasElements() && base.RawIdentity.StartsWith("\\") && base.RawIdentity.Length > 1)
			{
				string identityString = base.RawIdentity.Substring(1);
				wrapper = EnumerableWrapper<T>.GetWrapper(this.GetObjectsInOrganization<T>(identityString, rootId, session, optionalData));
			}
			return wrapper;
		}
	}
}
