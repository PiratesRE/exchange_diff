using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSetColumns : InputRop
	{
		internal PropertyTag[] PropertyTags
		{
			get
			{
				return this.propertyTags;
			}
		}

		internal override RopId RopId
		{
			get
			{
				return RopId.SetColumns;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSetColumns();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, SetColumnsFlags flags, PropertyTag[] propertyTags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.flags = flags;
			this.propertyTags = propertyTags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.flags);
			writer.WriteUInt16((ushort)this.propertyTags.Length);
			for (int i = 0; i < this.propertyTags.Length; i++)
			{
				writer.WritePropertyTag(this.propertyTags[i]);
			}
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSetColumnsResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSetColumns.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.flags = (SetColumnsFlags)reader.ReadByte();
			ushort num = reader.ReadUInt16();
			reader.CheckBoundary((uint)num, 4U);
			this.propertyTags = new PropertyTag[(int)num];
			for (int i = 0; i < (int)num; i++)
			{
				this.propertyTags[i] = reader.ReadPropertyTag();
			}
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SetColumns(serverObject, this.flags, this.propertyTags, RopSetColumns.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" Tags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.propertyTags);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.SetColumns;

		private static SetColumnsResultFactory resultFactory = new SetColumnsResultFactory();

		private SetColumnsFlags flags;

		private PropertyTag[] propertyTags;
	}
}
