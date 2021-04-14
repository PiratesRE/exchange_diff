using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class AutoResponseFlagProperty : SmartPropertyDefinition
	{
		internal AutoResponseFlagProperty(string displayName, MessageFlags flag, AutoResponseSuppress suppressFlag) : base(displayName, typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.Flags, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.AutoResponseSuppressInternal, PropertyDependencyType.NeedForRead)
		})
		{
			this.suppressMask = suppressFlag;
			this.nativeFlag = flag;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			bool flagValue = AutoResponseFlagProperty.GetFlagValue(propertyBag, InternalSchema.Flags, (int)this.nativeFlag);
			bool flagValue2 = AutoResponseFlagProperty.GetFlagValue(propertyBag, InternalSchema.AutoResponseSuppressInternal, (int)this.suppressMask);
			return flagValue && !flagValue2;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			throw new InvalidOperationException();
		}

		private static bool GetFlagValue(PropertyBag.BasicPropertyStore propertyBag, NativeStorePropertyDefinition prop, int flag)
		{
			int num;
			return Util.TryConvertTo<int>(propertyBag.GetValue(prop), out num) && (num & flag) == flag;
		}

		private readonly AutoResponseSuppress suppressMask;

		private readonly MessageFlags nativeFlag;
	}
}
