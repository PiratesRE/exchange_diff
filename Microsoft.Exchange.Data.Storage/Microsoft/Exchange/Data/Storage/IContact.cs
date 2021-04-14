using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IContact : IContactBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		IDictionary<EmailAddressIndex, Participant> EmailAddresses { get; }

		string ImAddress { get; set; }

		bool IsFavorite { get; set; }

		Stream GetPhotoStream();
	}
}
