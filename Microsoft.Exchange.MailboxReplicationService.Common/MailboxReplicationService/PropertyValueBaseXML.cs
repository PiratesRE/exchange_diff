using System;
using System.Security.AccessControl;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public abstract class PropertyValueBaseXML : XMLSerializableBase
	{
		internal abstract object RawValue { get; }

		public override string ToString()
		{
			return string.Format("{0}", this.RawValue);
		}

		internal static PropertyValueBaseXML Create(ProviderPropertyDefinition pdef, object value)
		{
			ADPropertyDefinition adpropertyDefinition = pdef as ADPropertyDefinition;
			IFormatProvider formatProvider = (adpropertyDefinition != null) ? adpropertyDefinition.FormatProvider : null;
			ADObjectId adobjectId = value as ADObjectId;
			if (adobjectId != null)
			{
				return ADObjectIdXML.Serialize(adobjectId);
			}
			OrganizationId organizationId = value as OrganizationId;
			if (organizationId != null)
			{
				return OrganizationIdXML.Serialize(organizationId);
			}
			RawSecurityDescriptor rawSecurityDescriptor = value as RawSecurityDescriptor;
			if (rawSecurityDescriptor != null)
			{
				return new PropertyStringValueXML
				{
					StrValue = CommonUtils.GetSDDLString(rawSecurityDescriptor)
				};
			}
			Exception ex;
			if (pdef.IsBinary)
			{
				byte[] binValue;
				if (ADValueConvertor.TryConvertValueToBinary(value, formatProvider, out binValue, out ex))
				{
					return new PropertyBinaryValueXML
					{
						BinValue = binValue
					};
				}
				MrsTracer.Common.Warning("Failed to convert {0} to binary, will try string: {1}", new object[]
				{
					pdef.Name,
					CommonUtils.FullExceptionMessage(ex)
				});
			}
			PropertyStringValueXML propertyStringValueXML = new PropertyStringValueXML();
			string text;
			if (!ADValueConvertor.TryConvertValueToString(value, formatProvider, out text, out ex))
			{
				text = value.ToString();
				MrsTracer.Common.Warning("Failed to convert {0} to string, defaulting to '{1}': {2}", new object[]
				{
					pdef.Name,
					text,
					CommonUtils.FullExceptionMessage(ex)
				});
			}
			propertyStringValueXML.StrValue = text;
			return propertyStringValueXML;
		}

		internal abstract bool TryGetValue(ProviderPropertyDefinition pdef, out object result);

		internal abstract bool HasValue();
	}
}
