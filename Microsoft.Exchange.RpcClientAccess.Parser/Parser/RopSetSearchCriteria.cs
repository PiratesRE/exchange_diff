using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSetSearchCriteria : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SetSearchCriteria;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSetSearchCriteria();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, Restriction restriction, StoreId[] folderIds, SetSearchCriteriaFlags setSearchCriteriaFlags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.restriction = restriction;
			this.folderIds = folderIds;
			this.setSearchCriteriaFlags = setSearchCriteriaFlags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedRestriction(this.restriction, string8Encoding, WireFormatStyle.Rop);
			writer.WriteCountedStoreIds(this.folderIds);
			writer.WriteUInt32((uint)this.setSearchCriteriaFlags);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSetSearchCriteria.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.restriction = reader.ReadSizeAndRestriction(WireFormatStyle.Rop);
			this.folderIds = reader.ReadSizeAndStoreIdArray();
			this.setSearchCriteriaFlags = (SetSearchCriteriaFlags)reader.ReadUInt32();
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
			this.result = ropHandler.SetSearchCriteria(serverObject, this.restriction, this.folderIds, this.setSearchCriteriaFlags, RopSetSearchCriteria.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.setSearchCriteriaFlags);
			if (this.folderIds != null)
			{
				stringBuilder.Append(" FIDs=[");
				Util.AppendToString<StoreId>(stringBuilder, this.folderIds);
				stringBuilder.Append("]");
			}
			if (this.restriction != null)
			{
				stringBuilder.Append(" Restriction=[");
				this.restriction.AppendToString(stringBuilder);
				stringBuilder.Append("]");
			}
		}

		private const RopId RopType = RopId.SetSearchCriteria;

		private static SetSearchCriteriaResultFactory resultFactory = new SetSearchCriteriaResultFactory();

		private Restriction restriction;

		private StoreId[] folderIds;

		private SetSearchCriteriaFlags setSearchCriteriaFlags;
	}
}
