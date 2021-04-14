using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetPropertiesAll : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetPropertiesAll;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetPropertiesAll();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ushort streamLimit, GetPropertiesFlags flags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.streamLimit = streamLimit;
			this.flags = flags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt16(this.streamLimit);
			writer.WriteUInt16((this.flags == GetPropertiesFlags.None) ? 0 : 1);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, (Reader readerParameter) => SuccessfulGetPropertiesAllResult.Parse(readerParameter, string8Encoding), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new GetPropertiesAllResultFactory(outputBuffer.Count, connection.String8Encoding);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.streamLimit = reader.ReadUInt16();
			this.flags = ((reader.ReadUInt16() == 0) ? GetPropertiesFlags.None : ((GetPropertiesFlags)int.MinValue));
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			GetPropertiesAllResultFactory resultFactory = new GetPropertiesAllResultFactory(outputBuffer.Count, serverObject.String8Encoding);
			this.result = ropHandler.GetPropertiesAll(serverObject, this.streamLimit, this.flags, resultFactory);
		}

		private const RopId RopType = RopId.GetPropertiesAll;

		private ushort streamLimit;

		private GetPropertiesFlags flags;
	}
}
