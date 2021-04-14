using System;
using System.Collections;
using System.ComponentModel;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[TypeConverter(typeof(SimpleGenericsTypeConverter))]
	[Serializable]
	public class ADMultiValuedProperty<T> : MultiValuedProperty<T>
	{
		internal ADMultiValuedProperty(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage) : base(readOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage)
		{
		}

		internal ADMultiValuedProperty(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values) : base(readOnly, propertyDefinition, values)
		{
		}

		public ADMultiValuedProperty(ICollection values) : base(values)
		{
		}

		public ADMultiValuedProperty(object value) : base(value)
		{
		}

		public ADMultiValuedProperty()
		{
		}

		protected override object SerializeValue(object value)
		{
			return ADValueConvertor.SerializeData((ADPropertyDefinition)this.PropertyDefinition, value);
		}

		protected override object DeserializeValue(object value)
		{
			return ADValueConvertor.DeserializeData((ADPropertyDefinition)this.PropertyDefinition, value);
		}
	}
}
