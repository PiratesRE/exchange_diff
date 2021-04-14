using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class LargeValue
	{
		public LargeValue(long actualLength, byte[] truncatedValue)
		{
			this.ActualLength = actualLength;
			this.TruncatedValue = truncatedValue;
		}

		public long ActualLength { get; private set; }

		public byte[] TruncatedValue { get; private set; }
	}
}
