using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopMoveCopyMessagesExtendedBase : RopMoveCopyMessagesBase
	{
		internal void SetInput(byte logonIndex, byte sourceHandleTableIndex, byte destinationHandleTableIndex, StoreId[] messageIds, bool reportProgress, bool isCopy, PropertyValue[] propertyValues)
		{
			base.SetInput(logonIndex, sourceHandleTableIndex, destinationHandleTableIndex, messageIds, reportProgress, isCopy);
			this.propertyValues = propertyValues;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteCountAndPropertyValueList(this.propertyValues, string8Encoding, WireFormatStyle.Rop);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.propertyValues = reader.ReadCountAndPropertyValueList(WireFormatStyle.Rop);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			foreach (PropertyValue propertyValue in this.propertyValues)
			{
				propertyValue.ResolveString8Values(string8Encoding);
			}
		}

		protected PropertyValue[] propertyValues;
	}
}
