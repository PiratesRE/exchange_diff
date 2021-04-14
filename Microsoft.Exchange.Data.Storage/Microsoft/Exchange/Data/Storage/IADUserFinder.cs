using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IADUserFinder
	{
		IGenericADUser FindBySid(IRecipientSession recipientSession, SecurityIdentifier sid);

		IGenericADUser FindByProxyAddress(IRecipientSession recipientSession, ProxyAddress proxyAddress);

		IGenericADUser FindByExchangeGuid(IRecipientSession recipientSession, Guid mailboxGuid, bool includeSystemMailbox);

		IGenericADUser FindByObjectId(IRecipientSession recipientSession, ADObjectId directoryEntry);

		IGenericADUser FindByLegacyExchangeDn(IRecipientSession recipientSession, string legacyExchangeDn);

		IGenericADUser FindMiniRecipientByProxyAddress(IRecipientSession recipientSession, ProxyAddress proxyAddress, PropertyDefinition[] miniRecipientProperties, out StorageMiniRecipient miniRecipient);
	}
}
