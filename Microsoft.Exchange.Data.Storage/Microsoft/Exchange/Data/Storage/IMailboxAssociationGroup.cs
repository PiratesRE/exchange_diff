using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxAssociationGroup : IMailboxAssociationBaseItem, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		ExDateTime PinDate { get; set; }
	}
}
