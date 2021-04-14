using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class InferenceUserCapabilityFlagsProperty : SmartPropertyDefinition
	{
		public InferenceUserCapabilityFlagsProperty(InferenceUserCapabilityFlags flag) : base(flag.ToString(), typeof(bool), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.InferenceUserCapabilityFlags, PropertyDependencyType.AllRead)
		})
		{
			this.propertyFlag = (int)flag;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			int valueOrDefault = propertyBag.GetValueOrDefault<int>(InternalSchema.InferenceUserCapabilityFlags, 0);
			return (valueOrDefault & this.propertyFlag) == this.propertyFlag;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object newValue)
		{
			int num = propertyBag.GetValueOrDefault<int>(InternalSchema.InferenceUserCapabilityFlags, 0);
			if ((bool)newValue)
			{
				num |= this.propertyFlag;
			}
			else
			{
				num &= ~this.propertyFlag;
			}
			propertyBag.SetOrDeleteProperty(InternalSchema.InferenceUserCapabilityFlags, (num == 0) ? null : num);
		}

		private readonly int propertyFlag;
	}
}
