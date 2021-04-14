using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopOpenFolder : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.OpenFolder;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopOpenFolder();
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopOpenFolder.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.folderId = StoreId.Parse(reader);
			this.openMode = (OpenMode)reader.ReadByte();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.OpenFolder(serverObject, this.folderId, this.openMode, RopOpenFolder.resultFactory);
		}

		internal void SetInput(byte logonIndex, byte inputObjectHandleTableIndex, byte outputFolderHandleIndex, StoreId folderId, OpenMode openMode)
		{
			base.SetCommonInput(logonIndex, inputObjectHandleTableIndex, outputFolderHandleIndex);
			this.folderId = folderId;
			this.openMode = openMode;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.folderId.Serialize(writer);
			writer.WriteByte((byte)this.openMode);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulOpenFolderResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" OpenMode=").Append(this.openMode);
			stringBuilder.Append(" FID=").Append(this.folderId.ToString());
		}

		private const RopId RopType = RopId.OpenFolder;

		private static OpenFolderResultFactory resultFactory = new OpenFolderResultFactory();

		private StoreId folderId;

		private OpenMode openMode;
	}
}
