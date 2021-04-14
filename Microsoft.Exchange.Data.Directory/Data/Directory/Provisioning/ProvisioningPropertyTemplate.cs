using System;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[Serializable]
	internal class ProvisioningPropertyTemplate : ProvisioningPropertyBase, IProvisioningTemplate, IProvisioningRule
	{
		public ProvisioningPropertyTemplate(ADPropertyDefinition policyProperty, ProviderPropertyDefinition objectProperty, ValueConverterDelegate valueConverter, Type[] targetObjectTypes) : base(policyProperty, objectProperty, valueConverter, targetObjectTypes)
		{
			if (objectProperty.IsReadOnly)
			{
				throw new ArgumentException(string.Format("objectProperty '{0}' is read only.", objectProperty.Name));
			}
		}

		public ProvisioningPropertyTemplate(ADPropertyDefinition policyProperty, ProviderPropertyDefinition objectProperty, ValueConverterDelegate valueConverter, Type targetObjectType) : this(policyProperty, objectProperty, valueConverter, new Type[]
		{
			targetObjectType
		})
		{
		}

		public virtual void Provision(ADProvisioningPolicy templatePolicy, IConfigurable writablePresentationObject)
		{
			if (templatePolicy == null)
			{
				throw new ArgumentNullException("templatePolicy");
			}
			if (writablePresentationObject == null)
			{
				throw new ArgumentNullException("writablePresentationObject");
			}
			if (!base.TargetObjectTypes.Contains(writablePresentationObject.GetType()))
			{
				throw new ArgumentOutOfRangeException("writablePresentationObject");
			}
			if (!(writablePresentationObject is ADObject))
			{
				throw new ArgumentOutOfRangeException("writablePresentationObject");
			}
			object obj = templatePolicy[base.PolicyProperty];
			if (!this.IsNullOrUnlimited(obj) && !this.IsNullOrEmptyMvp(obj))
			{
				if (base.ValueConverter != null)
				{
					obj = base.ValueConverter(obj);
				}
				((ADObject)writablePresentationObject)[base.ObjectProperty] = obj;
				return;
			}
		}

		protected bool IsNullOrUnlimited(object templateValue)
		{
			return templateValue == null || string.Empty.Equals(templateValue) || (base.PolicyProperty.Type.IsGenericType && base.PolicyProperty.Type.GetGenericTypeDefinition() == typeof(Unlimited<>) && templateValue.Equals(base.PolicyProperty.DefaultValue));
		}

		protected bool IsNullOrEmptyMvp(object templateValue)
		{
			return base.PolicyProperty.IsMultivalued && MultiValuedPropertyBase.IsNullOrEmpty((MultiValuedPropertyBase)templateValue);
		}
	}
}
