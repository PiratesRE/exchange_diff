using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetReceiveFolder : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetReceiveFolder;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetReceiveFolder();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, string messageClass)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.messageClass = messageClass;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteAsciiString(this.messageClass, StringFlags.IncludeNull);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetReceiveFolderResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetReceiveFolder.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.messageClass = reader.ReadAsciiString(StringFlags.IncludeNull);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetReceiveFolder(serverObject, this.messageClass, RopGetReceiveFolder.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Message Class=[").Append(this.messageClass).Append("]");
		}

		private const RopId RopType = RopId.GetReceiveFolder;

		private static GetReceiveFolderResultFactory resultFactory = new GetReceiveFolderResultFactory();

		private string messageClass;
	}
}
