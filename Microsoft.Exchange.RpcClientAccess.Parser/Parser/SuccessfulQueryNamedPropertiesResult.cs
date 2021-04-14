using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulQueryNamedPropertiesResult : RopResult
	{
		internal SuccessfulQueryNamedPropertiesResult(PropertyId[] propertyIds, NamedProperty[] namedProperties) : base(RopId.QueryNamedProperties, ErrorCode.None, null)
		{
			Util.ThrowOnNullArgument(propertyIds, "propertyIds");
			Util.ThrowOnNullArgument(namedProperties, "namedProperties");
			if (propertyIds.Length != namedProperties.Length)
			{
				throw new ArgumentException("PropertyId[] and NamedProperty[] must have the same length.");
			}
			this.propertyIds = propertyIds;
			this.namedProperties = namedProperties;
		}

		internal SuccessfulQueryNamedPropertiesResult(Reader reader) : base(reader)
		{
			this.propertyIds = reader.ReadSizeAndPropertyIdArray();
			this.namedProperties = reader.ReadNamedPropertyArray((ushort)this.propertyIds.Length);
		}

		internal PropertyId[] PropertyIds
		{
			get
			{
				return this.propertyIds;
			}
		}

		internal NamedProperty[] NamedProperties
		{
			get
			{
				return this.namedProperties;
			}
		}

		internal static SuccessfulQueryNamedPropertiesResult Parse(Reader reader)
		{
			return new SuccessfulQueryNamedPropertiesResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteCountedPropertyIds(this.propertyIds);
			for (int i = 0; i < this.namedProperties.Length; i++)
			{
				this.namedProperties[i].Serialize(writer);
			}
		}

		private readonly PropertyId[] propertyIds;

		private readonly NamedProperty[] namedProperties;
	}
}
