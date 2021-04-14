using System;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[Serializable]
	internal abstract class ProvisioningPropertyBase : ProvisioningRule
	{
		public ProviderPropertyDefinition ObjectProperty
		{
			get
			{
				return this.objectProperty;
			}
		}

		public ValueConverterDelegate ValueConverter
		{
			get
			{
				return this.valueConverter;
			}
		}

		public ADPropertyDefinition PolicyProperty
		{
			get
			{
				return this.policyProperty;
			}
		}

		public ProvisioningPropertyBase(ADPropertyDefinition policyProperty, ProviderPropertyDefinition objectProperty, ValueConverterDelegate valueConverter, Type[] targetObjectTypes) : base(targetObjectTypes)
		{
			if (policyProperty == null)
			{
				throw new ArgumentNullException("policyProperty");
			}
			if (objectProperty == null)
			{
				throw new ArgumentNullException("objectProperty");
			}
			this.policyProperty = policyProperty;
			this.valueConverter = valueConverter;
			this.objectProperty = objectProperty;
		}

		private ADPropertyDefinition policyProperty;

		private ProviderPropertyDefinition objectProperty;

		private ValueConverterDelegate valueConverter;
	}
}
