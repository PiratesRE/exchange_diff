using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.EntitySets
{
	internal interface IStorageEntitySetScope<out TStoreSession> where TStoreSession : IStoreSession
	{
		TStoreSession StoreSession { get; }

		IXSOFactory XsoFactory { get; }

		IdConverter IdConverter { get; }

		IRecipientSession RecipientSession { get; }
	}
}
