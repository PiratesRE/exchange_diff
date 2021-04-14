using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICustomSerializableBuilder : ICustomSerializable
	{
		ushort TypeId { get; set; }

		ICustomSerializable BuildObject();
	}
}
