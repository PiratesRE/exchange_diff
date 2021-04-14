using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulDeletePropertiesNoReplicateResult : RopResult
	{
		internal SuccessfulDeletePropertiesNoReplicateResult(PropertyProblem[] propertyProblems) : base(RopId.DeletePropertiesNoReplicate, ErrorCode.None, null)
		{
			this.propertyProblems = propertyProblems;
		}

		internal SuccessfulDeletePropertiesNoReplicateResult(Reader reader) : base(reader)
		{
			this.propertyProblems = reader.ReadSizeAndPropertyProblemArray();
		}

		internal static SuccessfulDeletePropertiesNoReplicateResult Parse(Reader reader)
		{
			return new SuccessfulDeletePropertiesNoReplicateResult(reader);
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
