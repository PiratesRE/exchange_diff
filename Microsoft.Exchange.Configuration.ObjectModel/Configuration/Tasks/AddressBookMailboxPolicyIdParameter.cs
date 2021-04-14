using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AddressBookMailboxPolicyIdParameter : ADIdParameter
	{
		public AddressBookMailboxPolicyIdParameter()
		{
		}

		public AddressBookMailboxPolicyIdParameter(string identity) : base(identity)
		{
		}

		public AddressBookMailboxPolicyIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public AddressBookMailboxPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static AddressBookMailboxPolicyIdParameter Parse(string identity)
		{
			return new AddressBookMailboxPolicyIdParameter(identity);
		}
	}
}
