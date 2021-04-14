using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TransportSendResultFactory : StandardResultFactory
	{
		internal TransportSendResultFactory(int maxSize, Encoding string8Encoding) : base(RopId.TransportSend)
		{
			this.maxSize = maxSize;
			this.string8Encoding = string8Encoding;
		}

		public RopResult CreateSuccessfulResult(PropertyValue[] propertyValues)
		{
			using (CountWriter countWriter = new CountWriter())
			{
				countWriter.WriteBool(false);
				countWriter.WriteCountAndPropertyValueList(propertyValues, this.string8Encoding, WireFormatStyle.Rop);
				if (countWriter.Position > (long)this.maxSize)
				{
					propertyValues = null;
				}
			}
			return new SuccessfulTransportSendResult(propertyValues);
		}

		private readonly int maxSize;

		private readonly Encoding string8Encoding;
	}
}
