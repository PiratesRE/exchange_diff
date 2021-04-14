using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSetReadFlag : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SetReadFlag;
			}
		}

		internal bool IsPublicLogon
		{
			get
			{
				return this.isPublicLogon;
			}
		}

		internal override byte InputHandleTableIndex
		{
			get
			{
				return this.realHandleTableIndex;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSetReadFlag();
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			SetReadFlagResultFactory resultFactory = new SetReadFlagResultFactory(base.LogonIndex, this.longTermId, this.IsPublicLogon);
			this.result = ropHandler.SetReadFlag(serverObject, this.flags, resultFactory);
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte realHandleTableIndex, SetReadFlagFlags flags, StoreLongTermId longTermId, bool isPublicLogon)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.realHandleTableIndex = realHandleTableIndex;
			this.flags = flags;
			this.longTermId = longTermId;
			this.isPublicLogon = isPublicLogon;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte(this.realHandleTableIndex);
			writer.WriteByte((byte)this.flags);
			if (this.isPublicLogon)
			{
				this.longTermId.Serialize(writer);
			}
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSetReadFlagResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new SetReadFlagResultFactory(base.LogonIndex, this.longTermId, this.IsPublicLogon);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable, IParseLogonTracker logonTracker)
		{
			base.InternalParseInput(reader, serverObjectHandleTable, logonTracker);
			this.isPublicLogon = logonTracker.ParseIsPublicLogon(base.LogonIndex);
			this.realHandleTableIndex = reader.ReadByte();
			this.flags = (SetReadFlagFlags)reader.ReadByte();
			if (this.isPublicLogon)
			{
				this.longTermId = StoreLongTermId.Parse(reader);
				return;
			}
			this.longTermId = StoreLongTermId.Null;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" LTID=[").Append(this.longTermId).Append("]");
		}

		private const RopId RopType = RopId.SetReadFlag;

		private byte realHandleTableIndex;

		private SetReadFlagFlags flags;

		private StoreLongTermId longTermId;

		private bool isPublicLogon;
	}
}
