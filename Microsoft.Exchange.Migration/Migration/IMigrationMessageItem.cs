using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationMessageItem : IMigrationStoreObject, IDisposable, IPropertyBag, IReadOnlyPropertyBag, IMigrationAttachmentMessage
	{
	}
}
