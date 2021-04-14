using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulCopyToExtendedResult : RopResult
	{
		internal SuccessfulCopyToExtendedResult(PropertyProblem[] propertyProblems) : base(RopId.CopyToExtended, ErrorCode.None, null)
		{
			this.propertyProblems = propertyProblems;
		}

		internal SuccessfulCopyToExtendedResult(Reader reader) : base(reader)
		{
			this.propertyProblems = reader.ReadSizeAndPropertyProblemArray();
		}

		internal static SuccessfulCopyToExtendedResult Parse(Reader reader)
		{
			return new SuccessfulCopyToExtendedResult(reader);
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
