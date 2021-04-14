using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopEmptyFolder : RopEmptyFolderBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.EmptyFolder;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopEmptyFolder();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = EmptyFolderResultFactory.Parse(reader);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			EmptyFolderResultFactory resultFactory = new EmptyFolderResultFactory(base.LogonIndex);
			this.result = ropHandler.EmptyFolder(serverObject, base.ReportProgress, base.EmptyFolderFlags, resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new EmptyFolderResultFactory(base.LogonIndex);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.EmptyFolder;
	}
}
