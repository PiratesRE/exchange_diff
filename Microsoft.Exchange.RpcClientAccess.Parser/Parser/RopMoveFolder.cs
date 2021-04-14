using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopMoveFolder : RopCopyMoveFolderBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.MoveFolder;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopMoveFolder();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = MoveFolderResultFactory.Parse(reader);
		}

		protected override void InternalExecute(IServerObject sourceServerObject, IServerObject destinationServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			MoveFolderResultFactory resultFactory = new MoveFolderResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
			this.result = ropHandler.MoveFolder(sourceServerObject, destinationServerObject, base.ReportProgress, base.FolderId, base.FolderName.StringValue, resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new MoveFolderResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
		}

		internal void SetInput(byte logonIndex, byte sourceHandleTableIndex, byte destinationHandleTableIndex, bool reportProgress, bool useUnicode, StoreId folderId, string folderName)
		{
			base.SetInput(logonIndex, sourceHandleTableIndex, destinationHandleTableIndex, reportProgress, false, useUnicode, folderId, folderName);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" FID=").Append(base.FolderId.ToString());
			stringBuilder.Append(" Name=[").Append(base.FolderName).Append("]");
			stringBuilder.Append(" Progress=").Append(base.ReportProgress);
		}

		private const RopId RopType = RopId.MoveFolder;
	}
}
