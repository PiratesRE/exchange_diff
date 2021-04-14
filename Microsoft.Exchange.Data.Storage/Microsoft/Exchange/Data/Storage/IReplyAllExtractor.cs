using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IReplyAllExtractor
	{
		bool TryRetrieveReplyAllDisplayNames(IStorePropertyBag propertyBag, out HashSet<string> displayNames);

		HashSet<string> RetrieveReplyAllDisplayNames(ICorePropertyBag propertyBag);

		ParticipantSet RetrieveReplyAllParticipants(ICorePropertyBag propertyBag);

		ParticipantSet RetrieveReplyAllParticipants(IStorePropertyBag propertyBag);
	}
}
