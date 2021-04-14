using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal struct PropertyProblem
	{
		public ushort Index
		{
			get
			{
				return this.index;
			}
		}

		public PropertyTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public ErrorCode ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public PropertyProblem(ushort index, PropertyTag propertyTag, ErrorCode errorCode)
		{
			this.index = index;
			this.propertyTag = propertyTag;
			this.errorCode = errorCode;
		}

		public override string ToString()
		{
			return string.Format("{{Index: {0}, PropertyTag: {1}, ErrorCode: {2}}}", this.index, this.propertyTag, this.errorCode);
		}

		internal static PropertyProblem Parse(Reader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			return new PropertyProblem(reader.ReadUInt16(), new PropertyTag(reader.ReadUInt32()), (ErrorCode)reader.ReadUInt32());
		}

		internal void Serialize(Writer writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteUInt16(this.index);
			writer.WritePropertyTag(this.propertyTag);
			writer.WriteUInt32((uint)this.errorCode);
		}

		internal static readonly uint MinimumSize = 10U;

		private readonly ushort index;

		private readonly PropertyTag propertyTag;

		private readonly ErrorCode errorCode;
	}
}
