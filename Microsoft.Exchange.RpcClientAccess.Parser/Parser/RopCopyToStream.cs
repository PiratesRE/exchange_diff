using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopCopyToStream : DualInputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.CopyToStream;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopCopyToStream();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = CopyToStreamResult.Parse(reader);
		}

		internal void SetInput(byte logonIndex, byte sourceHandleTableIndex, byte destinationHandleTableIndex, ulong bytesToCopy)
		{
			base.SetCommonInput(logonIndex, sourceHandleTableIndex, destinationHandleTableIndex);
			this.bytesToCopy = bytesToCopy;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt64(this.bytesToCopy);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new CopyToStreamResultFactory((uint)base.DestinationHandleTableIndex);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.bytesToCopy = reader.ReadUInt64();
		}

		protected override void InternalExecute(IServerObject sourceServerObject, IServerObject destinationServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			CopyToStreamResultFactory resultFactory = new CopyToStreamResultFactory((uint)base.DestinationHandleTableIndex);
			this.result = ropHandler.CopyToStream(sourceServerObject, destinationServerObject, this.bytesToCopy, resultFactory);
		}

		private const RopId RopType = RopId.CopyToStream;

		private ulong bytesToCopy;
	}
}
