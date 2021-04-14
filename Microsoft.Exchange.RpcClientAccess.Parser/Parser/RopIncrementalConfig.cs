using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopIncrementalConfig : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.IncrementalConfig;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopIncrementalConfig();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, IncrementalConfigOption configOptions, FastTransferSendOption sendOptions, SyncFlag syncFlags, Restriction restriction, SyncExtraFlag extraFlags, PropertyTag[] propertyTags, StoreId[] messageIds)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.configOptions = configOptions;
			this.sendOptions = sendOptions;
			this.syncFlags = syncFlags;
			this.restriction = restriction;
			this.extraFlags = extraFlags;
			this.propertyTags = propertyTags;
			this.messageIds = messageIds;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.configOptions);
			writer.WriteByte((byte)this.sendOptions);
			writer.WriteUInt16((ushort)this.syncFlags);
			writer.WriteSizedRestriction(this.restriction, string8Encoding, WireFormatStyle.Rop);
			writer.WriteUInt32((uint)this.extraFlags);
			writer.WriteCountAndPropertyTagArray(this.propertyTags, FieldLength.WordSize);
			if ((ushort)(this.syncFlags & SyncFlag.MessageSelective) != 0)
			{
				writer.WriteCountedStoreIds(this.messageIds);
			}
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulIncrementalConfigResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopIncrementalConfig.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.configOptions = (IncrementalConfigOption)reader.ReadByte();
			this.sendOptions = (FastTransferSendOption)reader.ReadByte();
			this.syncFlags = (SyncFlag)reader.ReadUInt16();
			this.restriction = reader.ReadSizeAndRestriction(WireFormatStyle.Rop);
			this.extraFlags = (SyncExtraFlag)reader.ReadUInt32();
			this.propertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
			if ((ushort)(this.syncFlags & SyncFlag.MessageSelective) != 0)
			{
				this.messageIds = reader.ReadSizeAndStoreIdArray();
			}
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			if (this.restriction != null)
			{
				this.restriction.ResolveString8Values(string8Encoding);
			}
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.IncrementalConfig(serverObject, this.configOptions, this.sendOptions, this.syncFlags, this.restriction, this.extraFlags, this.propertyTags, this.messageIds, RopIncrementalConfig.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" icsOptions=").Append(this.configOptions.ToString());
			stringBuilder.Append(" fxOptions=").Append(this.sendOptions.ToString());
			stringBuilder.Append(" syncFlags=").Append(this.syncFlags.ToString());
			stringBuilder.Append(" extraSyncFlags=").Append(this.extraFlags.ToString());
			stringBuilder.Append(" restriction=[");
			if (this.restriction != null)
			{
				this.restriction.AppendToString(stringBuilder);
			}
			stringBuilder.Append("]");
			stringBuilder.Append(" propertyTags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.propertyTags);
			stringBuilder.Append("]");
			stringBuilder.Append(" messageIds=[");
			Util.AppendToString<StoreId>(stringBuilder, this.messageIds);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.IncrementalConfig;

		private static IncrementalConfigResultFactory resultFactory = new IncrementalConfigResultFactory();

		private IncrementalConfigOption configOptions;

		private FastTransferSendOption sendOptions;

		private SyncFlag syncFlags;

		private Restriction restriction;

		private SyncExtraFlag extraFlags;

		private PropertyTag[] propertyTags;

		private StoreId[] messageIds;
	}
}
