using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface IPerfEventData
	{
		byte[] ToBytes();

		void FromBytes(byte[] data);

		string[] ToCsvRecord();
	}
}
