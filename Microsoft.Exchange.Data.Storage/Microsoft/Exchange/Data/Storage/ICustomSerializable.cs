using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICustomSerializable
	{
		void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool);

		void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool);
	}
}
