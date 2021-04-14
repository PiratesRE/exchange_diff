using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopSetPropertiesBase : InputRop
	{
		internal PropertyValue[] Properties
		{
			get
			{
				return this.propertyValues;
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Properties=[");
			Util.AppendToString<PropertyValue>(stringBuilder, this.propertyValues);
			stringBuilder.Append("]");
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, PropertyValue[] propertyValues)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.propertyValues = propertyValues;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			long position = writer.Position;
			writer.WriteUInt16(0);
			long position2 = writer.Position;
			writer.WriteCountAndPropertyValueList(this.propertyValues, string8Encoding, WireFormatStyle.Rop);
			long position3 = writer.Position;
			writer.Position = position;
			writer.WriteUInt16((ushort)(position3 - position2));
			writer.Position = position3;
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSetPropertiesResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			uint num = (uint)reader.ReadUInt16();
			reader.CheckBoundary(num, 1U);
			long position = reader.Position;
			int num2 = (int)reader.ReadUInt16();
			reader.CheckBoundary((uint)num2, 4U);
			this.propertyValues = new PropertyValue[num2];
			for (int i = 0; i < num2; i++)
			{
				this.propertyValues[i] = reader.ReadPropertyValue(WireFormatStyle.Rop);
			}
			long position2 = reader.Position;
			ulong num3 = (ulong)(position2 - position);
			if (num3 != (ulong)num)
			{
				throw new BufferParseException(string.Format("Size of PropertyValue[] reported incorrectly by client.  Size = {0}, Actual = {1}.", num, num3));
			}
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

		private PropertyValue[] propertyValues;
	}
}
