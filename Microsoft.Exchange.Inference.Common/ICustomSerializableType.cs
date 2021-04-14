using System;
using System.IO;

namespace Microsoft.Exchange.Inference.Common
{
	public interface ICustomSerializableType
	{
		void Serialize(BinaryWriter writer);

		void Deserialize(BinaryReader reader);
	}
}
