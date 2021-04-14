using System;
using System.IO;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal interface ISimpleADValue<T>
	{
		string Name { get; }

		void Read(BinaryReader reader);

		void Write(BinaryWriter writer);

		bool Equals(T right);
	}
}
