using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IStoreObject : IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		StoreObjectId StoreObjectId { get; }

		PersistablePropertyBag PropertyBag { get; }

		string ClassName { get; set; }

		bool IsNew { get; }

		IStoreSession Session { get; }

		StoreObjectId ParentId { get; }

		VersionedId Id { get; }

		byte[] RecordKey { get; }

		LocationIdentifierHelper LocationIdentifierHelperInstance { get; }
	}
}
