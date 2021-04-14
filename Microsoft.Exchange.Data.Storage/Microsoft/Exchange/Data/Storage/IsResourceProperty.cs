using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class IsResourceProperty : SmartPropertyDefinition
	{
		public IsResourceProperty() : base("IsResource", typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.DisplayTypeExInternal, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return this.GetIsResourceFromDisplayTypeEx(propertyBag) ?? new PropertyError(this, PropertyErrorCode.NotFound);
		}

		private bool? GetIsResourceFromDisplayTypeEx(PropertyBag.BasicPropertyStore propertyBag)
		{
			RecipientDisplayType? recipientDisplayType = new RecipientDisplayType?(propertyBag.GetValueOrDefault<RecipientDisplayType>(InternalSchema.DisplayTypeExInternal));
			if (recipientDisplayType == null)
			{
				return null;
			}
			return new bool?(DisplayTypeExProperty.IsResource(recipientDisplayType.Value));
		}
	}
}
