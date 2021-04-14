using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulTransportSendResult : RopResult
	{
		internal SuccessfulTransportSendResult(PropertyValue[] propertyValues) : base(RopId.TransportSend, ErrorCode.None, null)
		{
			this.propertyValues = propertyValues;
		}

		internal SuccessfulTransportSendResult(Reader reader, Encoding string8Encoding) : base(reader)
		{
			if (!reader.ReadBool())
			{
				this.propertyValues = reader.ReadCountAndPropertyValueList(WireFormatStyle.Rop);
				foreach (PropertyValue propertyValue in this.propertyValues)
				{
					propertyValue.ResolveString8Values(string8Encoding);
				}
			}
		}

		internal static SuccessfulTransportSendResult Parse(Reader reader, Encoding string8Encoding)
		{
			return new SuccessfulTransportSendResult(reader, string8Encoding);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			long position = writer.Position;
			bool flag = this.propertyValues == null || this.propertyValues.Length == 0;
			writer.WriteBool(flag);
			if (!flag)
			{
				writer.WriteCountAndPropertyValueList(this.propertyValues, base.String8Encoding, WireFormatStyle.Rop);
			}
		}

		private readonly PropertyValue[] propertyValues;
	}
}
