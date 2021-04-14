using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopHardEmptyFolder : RopEmptyFolderBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.HardEmptyFolder;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopHardEmptyFolder();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = HardEmptyFolderResultFactory.Parse(reader);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			HardEmptyFolderResultFactory resultFactory = new HardEmptyFolderResultFactory(base.LogonIndex);
			this.result = ropHandler.HardEmptyFolder(serverObject, base.ReportProgress, base.EmptyFolderFlags, resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new HardEmptyFolderResultFactory(base.LogonIndex);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.HardEmptyFolder;
	}
}
