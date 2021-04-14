using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class CopyToStreamResult : RopResult
	{
		internal CopyToStreamResult(ErrorCode errorCode, ulong bytesRead, ulong bytesWritten, uint destinationObjectHandleIndex) : base(RopId.CopyToStream, errorCode, null)
		{
			this.bytesRead = bytesRead;
			this.bytesWritten = bytesWritten;
			if (errorCode == ErrorCode.DestinationNullObject)
			{
				this.destinationObjectHandleIndex = destinationObjectHandleIndex;
			}
		}

		internal CopyToStreamResult(Reader reader) : base(reader)
		{
			if (base.ErrorCode == ErrorCode.DestinationNullObject)
			{
				this.destinationObjectHandleIndex = reader.ReadUInt32();
			}
			this.bytesRead = reader.ReadUInt64();
			this.bytesWritten = reader.ReadUInt64();
		}

		public ulong BytesRead
		{
			get
			{
				return this.bytesRead;
			}
		}

		public ulong BytesWritten
		{
			get
			{
				return this.bytesWritten;
			}
		}

		internal static RopResult Parse(Reader reader)
		{
			return new CopyToStreamResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			if (base.ErrorCode == ErrorCode.DestinationNullObject)
			{
				writer.WriteUInt32(this.destinationObjectHandleIndex);
			}
			writer.WriteUInt64(this.bytesRead);
			writer.WriteUInt64(this.bytesWritten);
		}

		private ulong bytesRead;

		private ulong bytesWritten;

		private uint destinationObjectHandleIndex;
	}
}
