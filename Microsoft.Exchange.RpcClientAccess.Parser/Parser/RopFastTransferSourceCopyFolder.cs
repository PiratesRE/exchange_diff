using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFastTransferSourceCopyFolder : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FastTransferSourceCopyFolder;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFastTransferSourceCopyFolder();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, FastTransferCopyFolderFlag flags, FastTransferSendOption sendOptions)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.flags = flags;
			this.sendOptions = sendOptions;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.flags);
			writer.WriteByte((byte)this.sendOptions);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulFastTransferSourceCopyFolderResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopFastTransferSourceCopyFolder.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.flags = (FastTransferCopyFolderFlag)reader.ReadByte();
			this.sendOptions = (FastTransferSendOption)reader.ReadByte();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.FastTransferSourceCopyFolder(serverObject, this.flags, this.sendOptions, RopFastTransferSourceCopyFolder.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" flags=").Append(this.flags.ToString());
			stringBuilder.Append(" sendOptions=").Append(this.sendOptions.ToString());
		}

		private const RopId RopType = RopId.FastTransferSourceCopyFolder;

		private static FastTransferSourceCopyFolderResultFactory resultFactory = new FastTransferSourceCopyFolderResultFactory();

		private FastTransferCopyFolderFlag flags;

		private FastTransferSendOption sendOptions;
	}
}
