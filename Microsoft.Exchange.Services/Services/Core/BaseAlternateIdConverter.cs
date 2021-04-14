using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class BaseAlternateIdConverter
	{
		internal abstract CanonicalConvertedId Parse(AlternateId id);

		internal abstract CanonicalConvertedId Parse(AlternatePublicFolderId id);

		internal abstract CanonicalConvertedId Parse(AlternatePublicFolderItemId id);

		internal abstract string ConvertStoreObjectIdToString(StoreObjectId storeObjectId);

		internal abstract IdFormat IdFormat { get; }

		internal abstract AlternateIdBase Format(CanonicalConvertedId canonicalId);
	}
}
