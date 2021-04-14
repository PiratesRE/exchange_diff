using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IADMailboxRecipient : IADRecipient, IADObject, IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag, IADMailStorage, IADSecurityPrincipal
	{
		ModernGroupObjectType ModernGroupType { get; set; }

		MultiValuedProperty<SecurityIdentifier> PublicToGroupSids { get; }

		Uri SharePointUrl { get; }

		DateTime? WhenMailboxCreated { get; }
	}
}
