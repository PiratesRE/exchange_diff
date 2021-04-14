using System;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[Serializable]
	internal abstract class ProvisioningResourceCountQuota : ProvisioningRule, IProvisioningEnforcement, IProvisioningRule
	{
		public ADPropertyDefinition CountQuotaProperty
		{
			get
			{
				return this.countQuotaProperty;
			}
		}

		public string SystemAddressListName
		{
			get
			{
				return this.systemAddressListName;
			}
		}

		public ProvisioningResourceCountQuota(ADPropertyDefinition countQuotaProperty, string systemAddressListName, Type[] targetObjectTypes) : base(targetObjectTypes)
		{
			if (countQuotaProperty == null)
			{
				throw new ArgumentNullException("countQuotaProperty");
			}
			if (systemAddressListName == null)
			{
				throw new ArgumentNullException("systemAddressListName");
			}
			if (typeof(Unlimited<int>) != countQuotaProperty.Type)
			{
				throw new ArgumentException("The type of countQuotaProperty is not Unlimited(of int32): dev code bug.");
			}
			this.countQuotaProperty = countQuotaProperty;
			this.systemAddressListName = systemAddressListName;
		}

		public virtual bool IsApplicable(IConfigurable readOnlyPresentationObject)
		{
			if (readOnlyPresentationObject == null)
			{
				throw new ArgumentNullException("readOnlyPresentationObject");
			}
			if (!base.TargetObjectTypes.Contains(readOnlyPresentationObject.GetType()))
			{
				throw new ArgumentOutOfRangeException("readOnlyPresentationObject");
			}
			return ObjectState.New == readOnlyPresentationObject.ObjectState;
		}

		public virtual ProvisioningValidationError[] Validate(ADProvisioningPolicy enforcementPolicy, IConfigurable readOnlyPresentationObject)
		{
			if (enforcementPolicy == null)
			{
				throw new ArgumentNullException("enforcementPolicy");
			}
			if (readOnlyPresentationObject == null)
			{
				throw new ArgumentNullException("readOnlyPresentationObject");
			}
			if (!base.TargetObjectTypes.Contains(readOnlyPresentationObject.GetType()))
			{
				throw new ArgumentOutOfRangeException("readOnlyPresentationObject");
			}
			return null;
		}

		private ADPropertyDefinition countQuotaProperty;

		private string systemAddressListName;
	}
}
