using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetNamesFromIDsResult : RopResult
	{
		internal SuccessfulGetNamesFromIDsResult(NamedProperty[] namedProperties) : base(RopId.GetNamesFromIDs, ErrorCode.None, null)
		{
			if (namedProperties == null)
			{
				throw new ArgumentNullException("namedProperties");
			}
			this.namedProperties = namedProperties;
		}

		internal SuccessfulGetNamesFromIDsResult(Reader reader) : base(reader)
		{
			this.namedProperties = reader.ReadSizeAndNamedPropertyArray();
		}

		public NamedProperty[] NamedProperties
		{
			get
			{
				return this.namedProperties;
			}
		}

		internal static SuccessfulGetNamesFromIDsResult Parse(Reader reader)
		{
			return new SuccessfulGetNamesFromIDsResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteCountedNamedProperties(this.namedProperties);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			if (this.namedProperties != null)
			{
				stringBuilder.Append(" Names=[");
				Util.AppendToString<NamedProperty>(stringBuilder, this.namedProperties);
				stringBuilder.Append("]");
			}
		}

		private readonly NamedProperty[] namedProperties;
	}
}
