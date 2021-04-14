using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetIdsFromNamesResult : RopResult
	{
		internal SuccessfulGetIdsFromNamesResult(PropertyId[] propertyIds) : base(RopId.GetIdsFromNames, ErrorCode.None, null)
		{
			if (propertyIds == null)
			{
				throw new ArgumentNullException("propertyIds");
			}
			this.propertyIds = propertyIds;
		}

		internal SuccessfulGetIdsFromNamesResult(Reader reader) : base(reader)
		{
			this.propertyIds = reader.ReadSizeAndPropertyIdArray();
		}

		public PropertyId[] PropertyIds
		{
			get
			{
				return this.propertyIds;
			}
		}

		internal static SuccessfulGetIdsFromNamesResult Parse(Reader reader)
		{
			return new SuccessfulGetIdsFromNamesResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteCountedPropertyIds(this.propertyIds);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" IDs=[");
			Util.AppendToString<PropertyId>(stringBuilder, this.propertyIds);
			stringBuilder.Append("]");
		}

		private readonly PropertyId[] propertyIds;
	}
}
