using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopUploadStateStreamBegin : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.UploadStateStreamBegin;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopUploadStateStreamBegin();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, PropertyTag propertyTag, uint size)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.propertyTag = propertyTag;
			this.size = size;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WritePropertyTag(this.propertyTag);
			writer.WriteUInt32(this.size);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopUploadStateStreamBegin.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.propertyTag = reader.ReadPropertyTag();
			this.size = reader.ReadUInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.UploadStateStreamBegin(serverObject, this.propertyTag, this.size, RopUploadStateStreamBegin.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" propertyTag=").Append(this.propertyTag.ToString());
			stringBuilder.Append(" size=").Append(this.size);
		}

		private const RopId RopType = RopId.UploadStateStreamBegin;

		private static UploadStateStreamBeginResultFactory resultFactory = new UploadStateStreamBeginResultFactory();

		private PropertyTag propertyTag;

		private uint size;
	}
}
