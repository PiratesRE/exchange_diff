using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IDirectoryPersonSearcher
	{
		bool TryFind(ContactInfoForLinking contactInfo, out ContactInfoForLinkingFromDirectory matchingContactInfo);
	}
}
