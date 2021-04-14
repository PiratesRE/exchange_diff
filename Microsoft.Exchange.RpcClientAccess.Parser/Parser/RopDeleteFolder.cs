using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopDeleteFolder : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.DeleteFolder;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopDeleteFolder();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, DeleteFolderFlags deleteFolderFlags, StoreId folderId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.deleteFolderFlags = deleteFolderFlags;
			this.folderId = folderId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.deleteFolderFlags);
			this.folderId.Serialize(writer);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = DeleteFolderResult.Parse(reader);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopDeleteFolder.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.deleteFolderFlags = (DeleteFolderFlags)reader.ReadByte();
			this.folderId = StoreId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.DeleteFolder(serverObject, this.deleteFolderFlags, this.folderId, RopDeleteFolder.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.deleteFolderFlags);
			stringBuilder.Append(" FID=").Append(this.folderId.ToString());
		}

		private const RopId RopType = RopId.DeleteFolder;

		private static DeleteFolderResultFactory resultFactory = new DeleteFolderResultFactory();

		private DeleteFolderFlags deleteFolderFlags;

		private StoreId folderId;
	}
}
