using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSetReadFlags : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SetReadFlags;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSetReadFlags();
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			SetReadFlagsResultFactory resultFactory = new SetReadFlagsResultFactory(base.LogonIndex);
			this.result = ropHandler.SetReadFlags(serverObject, this.reportProgress, this.flags, this.messageIds, resultFactory);
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, bool reportProgress, SetReadFlagFlags flags, StoreId[] messageIds)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.reportProgress = reportProgress;
			this.flags = flags;
			this.messageIds = messageIds;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteBool(this.reportProgress);
			writer.WriteByte((byte)this.flags);
			writer.WriteCountedStoreIds(this.messageIds);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = SetReadFlagsResultFactory.Parse(reader);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new SetReadFlagsResultFactory(base.LogonIndex);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable, IParseLogonTracker logonTracker)
		{
			base.InternalParseInput(reader, serverObjectHandleTable, logonTracker);
			this.reportProgress = reader.ReadBool();
			this.flags = (SetReadFlagFlags)reader.ReadByte();
			this.messageIds = reader.ReadSizeAndStoreIdArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Progress=").Append(this.reportProgress);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" MIDs=[");
			Util.AppendToString<StoreId>(stringBuilder, this.messageIds);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.SetReadFlags;

		private bool reportProgress;

		private SetReadFlagFlags flags;

		private StoreId[] messageIds;
	}
}
