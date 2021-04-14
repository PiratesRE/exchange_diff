using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IADGroup : IADMailboxRecipient, IADRecipient, IADObject, IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag, IADMailStorage, IADSecurityPrincipal, IADDistributionList
	{
	}
}
