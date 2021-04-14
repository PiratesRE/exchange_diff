using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulQueryColumnsAllResult : RopResult
	{
		internal SuccessfulQueryColumnsAllResult(PropertyTag[] propertyTags) : base(RopId.QueryColumnsAll, ErrorCode.None, null)
		{
			if (propertyTags == null)
			{
				throw new ArgumentNullException("propertyTags");
			}
			this.propertyTags = propertyTags;
		}

		internal SuccessfulQueryColumnsAllResult(Reader reader) : base(reader)
		{
			this.propertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
		}

		internal PropertyTag[] Columns
		{
			get
			{
				return this.propertyTags;
			}
		}

		internal static SuccessfulQueryColumnsAllResult Parse(Reader reader)
		{
			return new SuccessfulQueryColumnsAllResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteCountAndPropertyTagArray(this.propertyTags, FieldLength.WordSize);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Tags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.propertyTags);
			stringBuilder.Append("]");
		}

		private readonly PropertyTag[] propertyTags;
	}
}
