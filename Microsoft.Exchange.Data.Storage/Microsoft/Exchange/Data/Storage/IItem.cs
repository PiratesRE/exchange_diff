using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IItem : IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		ConflictResolutionResult Save(SaveMode saveMode);

		AttachmentCollection AttachmentCollection { get; }

		Body Body { get; }

		IBody IBody { get; }

		ItemCategoryList Categories { get; }

		ICoreItem CoreItem { get; }

		ItemCharsetDetector CharsetDetector { get; }

		IAttachmentCollection IAttachmentCollection { get; }

		MapiMessage MapiMessage { get; }

		PropertyBagSaveFlags SaveFlags { get; set; }

		void OpenAsReadWrite();
	}
}
