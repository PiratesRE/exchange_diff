using System;
using System.Net;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal class ARecord
	{
		public IPAddress IPAddressValue { get; private set; }

		public int ProcessResponse(byte[] message, int position)
		{
			byte[] array = new byte[4];
			Buffer.BlockCopy(message, position, array, 0, 4);
			int result = position + 4;
			this.IPAddressValue = new IPAddress(array);
			return result;
		}

		public override string ToString()
		{
			return string.Format("A={0}", this.IPAddressValue);
		}

		public const ushort RecordLength = 4;
	}
}
