using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAnchorMessageItem : IAnchorStoreObject, IDisposable, IPropertyBag, IReadOnlyPropertyBag, IAnchorAttachmentMessage
	{
	}
}
