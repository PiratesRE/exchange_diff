using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class ProxyAddressTemplateCollection : ProxyAddressBaseCollection<ProxyAddressTemplate>
	{
		public new static ProxyAddressTemplateCollection Empty
		{
			get
			{
				return ProxyAddressTemplateCollection.empty;
			}
		}

		public new bool AutoPromotionDisabled
		{
			get
			{
				return base.AutoPromotionDisabled;
			}
			set
			{
				base.AutoPromotionDisabled = value;
			}
		}

		public ProxyAddressTemplateCollection()
		{
			this.AutoPromotionDisabled = true;
		}

		public ProxyAddressTemplateCollection(object value)
		{
			this.AutoPromotionDisabled = true;
			this.Add(value);
		}

		public ProxyAddressTemplateCollection(ICollection values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			this.AutoPromotionDisabled = true;
			foreach (object item in values)
			{
				this.Add(item);
			}
		}

		public ProxyAddressTemplateCollection(Dictionary<string, object> table) : base(table)
		{
		}

		internal ProxyAddressTemplateCollection(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values) : base(readOnly, propertyDefinition, values)
		{
			this.AutoPromotionDisabled = true;
		}

		internal ProxyAddressTemplateCollection(bool readOnly, ProviderPropertyDefinition propertyDefinition, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage) : base(readOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage)
		{
			this.AutoPromotionDisabled = true;
		}

		public new static implicit operator ProxyAddressTemplateCollection(object[] array)
		{
			return new ProxyAddressTemplateCollection(false, null, array);
		}

		protected override ProxyAddressTemplate ConvertInput(object item)
		{
			if (item is string)
			{
				return ProxyAddressTemplate.Parse((string)item);
			}
			return base.ConvertInput(item);
		}

		public ProxyAddressTemplateCollection(Hashtable table) : base(table)
		{
		}

		private static ProxyAddressTemplateCollection empty = new ProxyAddressTemplateCollection(true, null, new ProxyAddressTemplate[0]);
	}
}
