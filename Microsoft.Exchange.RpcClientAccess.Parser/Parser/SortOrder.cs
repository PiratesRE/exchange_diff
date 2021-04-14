using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class SortOrder
	{
		internal PropertyTag Tag
		{
			get
			{
				return this.propertyTag;
			}
		}

		internal SortOrderFlags Flags
		{
			get
			{
				return this.sortOrderFlags;
			}
		}

		internal SortOrder(PropertyTag propertyTag, SortOrderFlags sortOrderFlags)
		{
			this.propertyTag = propertyTag;
			this.sortOrderFlags = sortOrderFlags;
		}

		internal static SortOrder Parse(Reader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			PropertyTag propertyTag = reader.ReadPropertyTag();
			SortOrderFlags sortOrderFlags = (SortOrderFlags)reader.ReadByte();
			return new SortOrder(propertyTag, sortOrderFlags);
		}

		internal void Serialize(Writer writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WritePropertyTag(this.propertyTag);
			writer.WriteByte((byte)this.sortOrderFlags);
		}

		public override string ToString()
		{
			return string.Format("SortOrder: [PropertyTag: {0}, SortOrderFlags: {1}]", this.propertyTag, this.sortOrderFlags);
		}

		private readonly PropertyTag propertyTag;

		private readonly SortOrderFlags sortOrderFlags;
	}
}
