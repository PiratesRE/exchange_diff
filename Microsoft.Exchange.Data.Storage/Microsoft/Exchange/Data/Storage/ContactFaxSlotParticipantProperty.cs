using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ContactFaxSlotParticipantProperty : ContactEmailSlotParticipantProperty
	{
		internal ContactFaxSlotParticipantProperty(EmailAddressIndex emailAddressIndex, NativeStorePropertyDefinition displayNamePropertyDefinition, NativeStorePropertyDefinition emailAddressPropertyDefinition, NativeStorePropertyDefinition routingTypePropertyDefinition, NativeStorePropertyDefinition entryIdPropertyDefinition, NativeStorePropertyDefinition faxPropDef) : base(emailAddressIndex, displayNamePropertyDefinition, emailAddressPropertyDefinition, routingTypePropertyDefinition, entryIdPropertyDefinition, null, new PropertyDependency[]
		{
			new PropertyDependency(faxPropDef, PropertyDependencyType.AllRead)
		})
		{
			this.faxPropDef = faxPropDef;
		}

		protected override ReadOnlyCollection<NativeStorePropertyDefinition> GetEmailSlotProperties()
		{
			ICollection<NativeStorePropertyDefinition> emailSlotProperties = base.GetEmailSlotProperties();
			NativeStorePropertyDefinition[] array = new NativeStorePropertyDefinition[emailSlotProperties.Count + 1];
			emailSlotProperties.CopyTo(array, 0);
			array[emailSlotProperties.Count] = this.faxPropDef;
			return new ReadOnlyCollection<NativeStorePropertyDefinition>(array);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			base.InternalSetValue(propertyBag, value);
			if (value == null)
			{
				propertyBag.Delete(this.faxPropDef);
				return;
			}
			Participant participant = (Participant)value;
			propertyBag.SetValueWithFixup(this.faxPropDef, participant.EmailAddress ?? string.Empty);
		}

		private readonly NativeStorePropertyDefinition faxPropDef;
	}
}
