using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncItemId : ICustomSerializableBuilder, ICustomSerializable
	{
		object NativeId { get; }
	}
}
