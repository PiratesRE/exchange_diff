using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulCopyPropertiesResult : RopResult
	{
		internal PropertyProblem[] PropertyProblems
		{
			get
			{
				return this.propertyProblems;
			}
		}

		internal SuccessfulCopyPropertiesResult(PropertyProblem[] propertyProblems) : base(RopId.CopyProperties, ErrorCode.None, null)
		{
			this.propertyProblems = propertyProblems;
		}

		internal SuccessfulCopyPropertiesResult(Reader reader) : base(reader)
		{
			this.propertyProblems = reader.ReadSizeAndPropertyProblemArray();
		}

		internal static SuccessfulCopyPropertiesResult Parse(Reader reader)
		{
			return new SuccessfulCopyPropertiesResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteCountedPropertyProblems(this.propertyProblems);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			if (this.propertyProblems != null && this.propertyProblems.Length > 0)
			{
				stringBuilder.Append(" Problems=[");
				Util.AppendToString<PropertyProblem>(stringBuilder, this.propertyProblems);
				stringBuilder.Append("]");
			}
		}

		private readonly PropertyProblem[] propertyProblems;
	}
}
