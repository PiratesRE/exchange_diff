using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulDeletePropertiesResult : RopResult
	{
		internal SuccessfulDeletePropertiesResult(PropertyProblem[] propertyProblems) : base(RopId.DeleteProperties, ErrorCode.None, null)
		{
			this.propertyProblems = propertyProblems;
		}

		internal SuccessfulDeletePropertiesResult(Reader reader) : base(reader)
		{
			this.propertyProblems = reader.ReadSizeAndPropertyProblemArray();
		}

		public PropertyProblem[] PropertyProblems
		{
			get
			{
				return this.propertyProblems;
			}
		}

		internal static SuccessfulDeletePropertiesResult Parse(Reader reader)
		{
			return new SuccessfulDeletePropertiesResult(reader);
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
