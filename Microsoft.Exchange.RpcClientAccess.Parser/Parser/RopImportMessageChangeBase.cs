using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopImportMessageChangeBase : InputOutputRop
	{
		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.importMessageChangeFlags = importMessageChangeFlags;
			this.propertyValues = propertyValues;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.importMessageChangeFlags);
			writer.WriteCountAndPropertyValueList(this.propertyValues, string8Encoding, WireFormatStyle.Rop);
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			foreach (PropertyValue propertyValue in this.propertyValues)
			{
				propertyValue.ResolveString8Values(string8Encoding);
			}
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.importMessageChangeFlags = (ImportMessageChangeFlags)reader.ReadByte();
			this.propertyValues = reader.ReadCountAndPropertyValueList(WireFormatStyle.Rop);
		}

		protected ImportMessageChangeFlags importMessageChangeFlags;

		protected PropertyValue[] propertyValues;
	}
}
