using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal enum TopologyMode
	{
		ADTopologyService,
		[Obsolete("Removed and replaced by LdapTopology Provider")]
		DirectoryServices,
		Adam,
		Ldap
	}
}
