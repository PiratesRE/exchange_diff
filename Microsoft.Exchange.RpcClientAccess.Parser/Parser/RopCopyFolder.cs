using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopCopyFolder : RopCopyMoveFolderBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.CopyFolder;
			}
		}

		protected override bool IsCopyFolder
		{
			get
			{
				return true;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopCopyFolder();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = CopyFolderResultFactory.Parse(reader);
		}

		protected override void InternalExecute(IServerObject sourceServerObject, IServerObject destinationServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			CopyFolderResultFactory resultFactory = new CopyFolderResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
			this.result = ropHandler.CopyFolder(sourceServerObject, destinationServerObject, base.ReportProgress, base.Recurse, base.FolderId, base.FolderName.StringValue, resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new CopyFolderResultFactory(base.LogonIndex, (uint)base.DestinationHandleTableIndex);
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
			stringBuilder.Append(" Recurse=").Append(base.Recurse);
			stringBuilder.Append(" Progress=").Append(base.ReportProgress);
		}

		private const RopId RopType = RopId.CopyFolder;
	}
}
