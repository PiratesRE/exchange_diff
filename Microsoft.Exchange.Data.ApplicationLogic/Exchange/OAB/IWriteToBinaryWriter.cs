using System;
using System.IO;

namespace Microsoft.Exchange.OAB
{
	internal interface IWriteToBinaryWriter
	{
		void WriteTo(BinaryWriter writer);
	}
}
