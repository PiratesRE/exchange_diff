using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetOptionsData : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetOptionsData;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetOptionsData();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, string addressType, bool wantWin32)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.addressType = addressType;
			this.wantWin32 = wantWin32;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteAsciiString(this.addressType, StringFlags.IncludeNull);
			writer.WriteBool(this.wantWin32);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, (Reader readerParameter) => SuccessfulGetOptionsDataResult.Parse(readerParameter, string8Encoding), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetOptionsData.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.addressType = reader.ReadAsciiString(StringFlags.IncludeNull);
			this.wantWin32 = reader.ReadBool();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetOptionsData(serverObject, this.addressType, this.wantWin32, RopGetOptionsData.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" AddressType=").Append(this.addressType);
			stringBuilder.Append(" WantWin32=").Append(this.wantWin32);
		}

		private const RopId RopType = RopId.GetOptionsData;

		private static GetOptionsDataResultFactory resultFactory = new GetOptionsDataResultFactory();

		private string addressType;

		private bool wantWin32;
	}
}
