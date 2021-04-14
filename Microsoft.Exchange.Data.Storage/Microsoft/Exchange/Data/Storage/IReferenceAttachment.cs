using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IReferenceAttachment : IAttachment, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		string AttachLongPathName { get; set; }

		string ProviderEndpointUrl { get; set; }

		string ProviderType { get; set; }
	}
}
