using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulCreateBookmarkResult : RopResult
	{
		internal SuccessfulCreateBookmarkResult(byte[] bookmark) : base(RopId.CreateBookmark, ErrorCode.None, null)
		{
			if (bookmark == null)
			{
				throw new ArgumentNullException("bookmark");
			}
			this.bookmark = bookmark;
		}

		internal SuccessfulCreateBookmarkResult(Reader reader) : base(reader)
		{
			this.bookmark = reader.ReadSizeAndByteArray();
		}

		internal byte[] Bookmark
		{
			get
			{
				return this.bookmark;
			}
		}

		internal static SuccessfulCreateBookmarkResult Parse(Reader reader)
		{
			return new SuccessfulCreateBookmarkResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteSizedBytes(this.bookmark);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Bookmark=[");
			Util.AppendToString(stringBuilder, this.bookmark);
			stringBuilder.Append("]");
		}

		private readonly byte[] bookmark;
	}
}
