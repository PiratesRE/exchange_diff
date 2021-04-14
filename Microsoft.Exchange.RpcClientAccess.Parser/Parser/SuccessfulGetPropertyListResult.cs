using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetPropertyListResult : RopResult
	{
		internal SuccessfulGetPropertyListResult(PropertyTag[] propertyTags) : base(RopId.GetPropertyList, ErrorCode.None, null)
		{
			if (propertyTags == null)
			{
				throw new ArgumentNullException("propertyTags");
			}
			this.propertyTags = propertyTags;
		}

		internal SuccessfulGetPropertyListResult(Reader reader) : base(reader)
		{
			this.propertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
		}

		internal static SuccessfulGetPropertyListResult Parse(Reader reader)
		{
			return new SuccessfulGetPropertyListResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteCountAndPropertyTagArray(this.propertyTags, FieldLength.WordSize);
		}

		public PropertyTag[] PropertyTags
		{
			get
			{
				return this.propertyTags;
			}
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
