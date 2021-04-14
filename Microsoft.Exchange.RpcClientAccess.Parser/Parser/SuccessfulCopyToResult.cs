using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulCopyToResult : RopResult
	{
		internal PropertyProblem[] PropertyProblems
		{
			get
			{
				return this.propertyProblems;
			}
		}

		internal SuccessfulCopyToResult(PropertyProblem[] propertyProblems) : base(RopId.CopyTo, ErrorCode.None, null)
		{
			this.propertyProblems = propertyProblems;
		}

		internal SuccessfulCopyToResult(Reader reader) : base(reader)
		{
			this.propertyProblems = reader.ReadSizeAndPropertyProblemArray();
		}

		internal static SuccessfulCopyToResult Parse(Reader reader)
		{
			return new SuccessfulCopyToResult(reader);
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
