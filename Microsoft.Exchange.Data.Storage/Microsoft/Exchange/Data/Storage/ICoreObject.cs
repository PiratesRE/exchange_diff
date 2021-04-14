using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICoreObject : ICoreState, IValidatable, IDisposeTrackable, IDisposable
	{
		ICorePropertyBag PropertyBag { get; }

		Schema GetCorrectSchemaForStoreObject();

		StoreSession Session { get; }

		StoreObjectId StoreObjectId { get; }

		StoreObjectId InternalStoreObjectId { get; }

		VersionedId Id { get; }

		bool IsDirty { get; }

		void ResetId();

		void SetEnableFullValidation(bool enableFullValidation);
	}
}
