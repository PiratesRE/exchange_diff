using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class AttachmentIsInlineProperty : SmartPropertyDefinition
	{
		internal AttachmentIsInlineProperty() : base("AttachmentIsInline", typeof(bool), PropertyFlags.None, Array<PropertyDefinitionConstraint>.Empty, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.AttachMhtmlFlags, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.AttachMethod, PropertyDependencyType.AllRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			int? num = propertyBag.GetValue(InternalSchema.AttachMethod) as int?;
			if (num != null && num.Value == 6)
			{
				return true;
			}
			object value = propertyBag.GetValue(InternalSchema.AttachMhtmlFlags);
			if (value is int)
			{
				return ((int)value & 4) == 4;
			}
			return value;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (!(value is bool))
			{
				string message = ServerStrings.ExInvalidValueTypeForCalculatedProperty(value, base.Type);
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), message);
				throw new ArgumentException(message);
			}
			int? num = propertyBag.GetValue(InternalSchema.AttachMethod) as int?;
			if (num != null && num.Value == 6 && !(bool)value)
			{
				return;
			}
			int? num2 = propertyBag.GetValue(InternalSchema.AttachMhtmlFlags) as int?;
			int num3;
			if ((bool)value)
			{
				if (num2 != null)
				{
					num3 = (num2.Value | 4);
				}
				else
				{
					num3 = 4;
				}
			}
			else if (num2 != null)
			{
				num3 = (num2.Value & -5);
			}
			else
			{
				num3 = 0;
			}
			propertyBag.SetValueWithFixup(InternalSchema.AttachMhtmlFlags, num3);
		}

		private const int AttachFlagIsInlineBit = 4;
	}
}
