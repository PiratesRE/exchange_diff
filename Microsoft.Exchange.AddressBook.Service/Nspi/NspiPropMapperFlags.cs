using System;

namespace Microsoft.Exchange.AddressBook.Nspi
{
	[Flags]
	internal enum NspiPropMapperFlags
	{
		None = 0,
		UseEphemeralId = 1,
		SkipMissingProperties = 2,
		SkipObjects = 4,
		IncludeDisplayName = 8,
		IncludeHiddenFromAddressListsEnabled = 16,
		GetTemplateProps = 32
	}
}
