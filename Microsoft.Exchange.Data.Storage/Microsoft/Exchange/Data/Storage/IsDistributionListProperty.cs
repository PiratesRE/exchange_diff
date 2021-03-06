using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class IsDistributionListProperty : SmartPropertyDefinition
	{
		public IsDistributionListProperty() : base("IsDistributionList", typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.DisplayTypeExInternal, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.DisplayType, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object result;
			if ((result = this.GetIsDLFromDisplayTypeEx(propertyBag)) == null)
			{
				result = (this.GetIsDLFromDisplayType(propertyBag) ?? new PropertyError(this, PropertyErrorCode.NotFound));
			}
			return result;
		}

		private bool? GetIsDLFromDisplayTypeEx(PropertyBag.BasicPropertyStore propertyBag)
		{
			RecipientDisplayType? valueAsNullable = propertyBag.GetValueAsNullable<RecipientDisplayType>(InternalSchema.DisplayTypeExInternal);
			if (valueAsNullable == null)
			{
				return null;
			}
			return new bool?(DisplayTypeExProperty.IsDL(valueAsNullable.Value));
		}

		private bool? GetIsDLFromDisplayType(PropertyBag.BasicPropertyStore propertyBag)
		{
			LegacyRecipientDisplayType? valueAsNullable = propertyBag.GetValueAsNullable<LegacyRecipientDisplayType>(InternalSchema.DisplayType);
			if (valueAsNullable == null)
			{
				return null;
			}
			return new bool?(DisplayTypeExProperty.IsDL(valueAsNullable.Value));
		}
	}
}
