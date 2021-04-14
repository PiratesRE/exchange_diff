using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSetPropertiesResult : RopResult
	{
		internal SuccessfulSetPropertiesResult(RopId ropId, PropertyProblem[] propertyProblems) : base(ropId, ErrorCode.None, null)
		{
			if (ropId != RopId.SetProperties && ropId != RopId.SetPropertiesNoReplicate)
			{
				throw new ArgumentException("SetProperties and SetPropertiesNoReplicate are the only valid rop types.");
			}
			Util.ThrowOnNullArgument(propertyProblems, "propertyProblems");
			this.propertyProblems = propertyProblems;
		}

		internal SuccessfulSetPropertiesResult(Reader reader) : base(reader)
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

		internal static RopResult Parse(Reader reader)
		{
			return new SuccessfulSetPropertiesResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteCountedPropertyProblems(this.propertyProblems);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Problems=[");
			Util.AppendToString<PropertyProblem>(stringBuilder, this.propertyProblems);
			stringBuilder.Append("]");
		}

		private readonly PropertyProblem[] propertyProblems;
	}
}
