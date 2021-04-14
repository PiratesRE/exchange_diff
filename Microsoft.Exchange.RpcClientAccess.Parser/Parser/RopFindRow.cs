using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFindRow : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FindRow;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFindRow();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, FindRowFlags flags, Restriction restriction, BookmarkOrigin bookmarkOrigin, byte[] bookmark)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.flags = flags;
			this.restriction = restriction;
			this.bookmarkOrigin = bookmarkOrigin;
			this.bookmark = bookmark;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte((byte)this.flags);
			writer.WriteSizedRestriction(this.restriction, string8Encoding, WireFormatStyle.Rop);
			writer.WriteByte((byte)this.bookmarkOrigin);
			if (this.bookmark == null)
			{
				writer.WriteUInt16(0);
				return;
			}
			writer.WriteUInt16((ushort)this.bookmark.Length);
			writer.WriteBytes(this.bookmark);
		}

		internal void SetParseOutputData(PropertyTag[] columns)
		{
			Util.ThrowOnNullArgument(columns, "columns");
			this.columns = columns;
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			if (this.columns == null)
			{
				throw new InvalidOperationException("SetParseOutputData must be called before ParseOutput.");
			}
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, (Reader readerParameter) => new SuccessfulFindRowResult(readerParameter, this.columns, string8Encoding), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return new FindRowResultFactory(outputBuffer.Count, connection.String8Encoding);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.flags = (FindRowFlags)reader.ReadByte();
			this.restriction = reader.ReadSizeAndRestriction(WireFormatStyle.Rop);
			this.bookmarkOrigin = (BookmarkOrigin)reader.ReadByte();
			uint num = (uint)reader.ReadUInt16();
			if (num > 0U)
			{
				this.bookmark = reader.ReadBytes(num);
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
			FindRowResultFactory resultFactory = new FindRowResultFactory(outputBuffer.Count, serverObject.String8Encoding);
			this.result = ropHandler.FindRow(serverObject, this.flags, this.restriction, this.bookmarkOrigin, this.bookmark, resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" Origin=").Append(this.bookmarkOrigin);
			if (this.bookmark != null)
			{
				stringBuilder.Append(" Bookmark=[");
				Util.AppendToString(stringBuilder, this.bookmark);
				stringBuilder.Append("]");
			}
			if (this.restriction != null)
			{
				stringBuilder.Append(" Restriction=[");
				this.restriction.AppendToString(stringBuilder);
				stringBuilder.Append("]");
			}
		}

		private const RopId RopType = RopId.FindRow;

		private FindRowFlags flags;

		private Restriction restriction;

		private BookmarkOrigin bookmarkOrigin;

		private byte[] bookmark;

		private PropertyTag[] columns;
	}
}
