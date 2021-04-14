using System;
using System.Collections;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class DagNetMultiValuedProperty<T> : MultiValuedProperty<T>
	{
		internal DagNetMultiValuedProperty(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage) : base(readOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage)
		{
		}

		internal DagNetMultiValuedProperty(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values) : base(readOnly, propertyDefinition, values)
		{
		}

		public DagNetMultiValuedProperty(ICollection values) : base(values)
		{
		}

		public DagNetMultiValuedProperty(object value) : base(value)
		{
		}

		public DagNetMultiValuedProperty()
		{
		}

		public new static implicit operator DagNetMultiValuedProperty<T>(object[] array)
		{
			if (array == null)
			{
				return null;
			}
			return new DagNetMultiValuedProperty<T>(true, null, array);
		}

		protected override object SerializeValue(object value)
		{
			return value;
		}

		protected override object DeserializeValue(object value)
		{
			return value;
		}
	}
}
