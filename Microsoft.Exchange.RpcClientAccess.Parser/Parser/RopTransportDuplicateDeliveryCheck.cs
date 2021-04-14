using System;
using System.Text;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopTransportDuplicateDeliveryCheck : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.TransportDuplicateDeliveryCheck;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopTransportDuplicateDeliveryCheck();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte flags, ExDateTime submitTime, string internetMessageId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.flags = flags;
			this.submitTime = submitTime;
			this.internetMessageId = internetMessageId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte(this.flags);
			writer.WriteInt64(PropertyValue.ExDateTimeToFileTimeUtc(this.submitTime));
			writer.WriteAsciiString(this.internetMessageId, StringFlags.IncludeNull | StringFlags.Sized16);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopTransportDuplicateDeliveryCheck.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.flags = reader.ReadByte();
			this.submitTime = PropertyValue.ExDateTimeFromFileTimeUtc(reader.ReadInt64());
			this.internetMessageId = reader.ReadAsciiString(StringFlags.IncludeNull | StringFlags.Sized16);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.TransportDuplicateDeliveryCheck(serverObject, this.flags, this.submitTime, this.internetMessageId, RopTransportDuplicateDeliveryCheck.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" SubmitTime=").Append(this.submitTime);
			stringBuilder.Append(" InternetMessageId=").Append(this.internetMessageId);
		}

		private const RopId RopType = RopId.TransportDuplicateDeliveryCheck;

		private static TransportDuplicateDeliveryCheckResultFactory resultFactory = new TransportDuplicateDeliveryCheckResultFactory();

		private byte flags;

		private ExDateTime submitTime;

		private string internetMessageId;
	}
}
