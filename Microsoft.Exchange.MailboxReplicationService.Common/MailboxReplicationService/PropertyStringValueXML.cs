using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class PropertyStringValueXML : PropertyValueBaseXML
	{
		[XmlText]
		public string StrValue { get; set; }

		internal override object RawValue
		{
			get
			{
				return this.StrValue;
			}
		}

		internal override bool TryGetValue(ProviderPropertyDefinition pdef, out object result)
		{
			if (this.StrValue == null)
			{
				result = null;
				return true;
			}
			ADPropertyDefinition adpropertyDefinition = pdef as ADPropertyDefinition;
			IFormatProvider formatProvider = (adpropertyDefinition != null) ? adpropertyDefinition.FormatProvider : null;
			result = null;
			Exception ex = null;
			if (!ADValueConvertor.TryConvertValueFromString(this.StrValue, pdef.Type, formatProvider, out result, out ex))
			{
				MrsTracer.Common.Warning("Failed to convert {0} from string '{1}': {2}", new object[]
				{
					pdef.Name,
					this.StrValue,
					CommonUtils.FullExceptionMessage(ex)
				});
				return false;
			}
			return true;
		}

		internal override bool HasValue()
		{
			return !string.IsNullOrEmpty(this.StrValue);
		}
	}
}
