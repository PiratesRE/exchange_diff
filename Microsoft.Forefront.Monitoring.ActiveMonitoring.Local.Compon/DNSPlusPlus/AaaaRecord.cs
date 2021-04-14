using System;
using System.Net;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal class AaaaRecord
	{
		public IPAddress IPAddressValue { get; private set; }

		public int ProcessResponse(byte[] message, int position)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(message, position, array, 0, 16);
			int result = position + 16;
			this.IPAddressValue = new IPAddress(array);
			return result;
		}

		public override string ToString()
		{
			return string.Format("AAAA={0}", this.IPAddressValue);
		}

		public const ushort RecordLength = 16;
	}
}
