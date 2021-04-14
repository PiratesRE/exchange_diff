using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetSearchCriteriaResult : RopResult
	{
		internal SuccessfulGetSearchCriteriaResult(Restriction restriction, byte logonIndex, StoreId[] folderIds, SearchState searchState) : base(RopId.GetSearchCriteria, ErrorCode.None, null)
		{
			this.restriction = restriction;
			this.logonIndex = logonIndex;
			this.folderIds = folderIds;
			this.searchState = searchState;
		}

		internal SuccessfulGetSearchCriteriaResult(Reader reader, Encoding string8Encoding) : base(reader)
		{
			this.restriction = reader.ReadSizeAndRestriction(WireFormatStyle.Rop);
			if (this.restriction != null)
			{
				this.restriction.ResolveString8Values(string8Encoding);
			}
			this.logonIndex = reader.ReadByte();
			this.folderIds = reader.ReadSizeAndStoreIdArray();
			this.searchState = (SearchState)reader.ReadUInt32();
		}

		internal StoreId[] FolderIds
		{
			get
			{
				return this.folderIds;
			}
		}

		internal Restriction Restriction
		{
			get
			{
				return this.restriction;
			}
		}

		internal SearchState SearchState
		{
			get
			{
				return this.searchState;
			}
		}

		internal static SuccessfulGetSearchCriteriaResult Parse(Reader reader, Encoding string8Encoding)
		{
			return new SuccessfulGetSearchCriteriaResult(reader, string8Encoding);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteSizedRestriction(this.restriction, base.String8Encoding, WireFormatStyle.Rop);
			writer.WriteByte(this.logonIndex);
			writer.WriteCountedStoreIds(this.folderIds);
			writer.WriteUInt32((uint)this.searchState);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			if (this.restriction != null)
			{
				stringBuilder.Append(" Restriction=[");
				this.restriction.AppendToString(stringBuilder);
				stringBuilder.Append("]");
			}
			stringBuilder.Append(" Folders=[");
			Util.AppendToString<StoreId>(stringBuilder, this.folderIds);
			stringBuilder.Append("] SearchState=").Append(this.searchState);
		}

		private readonly Restriction restriction;

		private readonly StoreId[] folderIds;

		private readonly byte logonIndex;

		private readonly SearchState searchState;
	}
}
