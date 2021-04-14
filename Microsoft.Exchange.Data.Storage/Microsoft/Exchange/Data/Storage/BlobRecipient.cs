using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BlobRecipient
	{
		internal BlobRecipient(ExTimeZone timeZone)
		{
			this.propertyBag = new MemoryPropertyBag();
			this.propertyBag.ExTimeZone = timeZone;
			this.propertyBag.SetAllPropertiesLoaded();
		}

		internal BlobRecipient(RecipientBase recipient)
		{
			this.propertyBag = new MemoryPropertyBag(recipient.CoreRecipient.GetMemoryPropertyBag());
			this.propertyBag.SetAllPropertiesLoaded();
		}

		internal BlobRecipient(Participant participant)
		{
			this.propertyBag = new MemoryPropertyBag();
			this.propertyBag[InternalSchema.RecipientBaseParticipant] = participant;
			this.propertyBag.SetAllPropertiesLoaded();
		}

		internal Participant Participant
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<Participant>(InternalSchema.RecipientBaseParticipant);
			}
		}

		internal object TryGetProperty(PropertyDefinition property)
		{
			StorePropertyDefinition property2 = InternalSchema.ToStorePropertyDefinition(property);
			return this.TryGetProperty(property2);
		}

		internal object TryGetProperty(StorePropertyDefinition property)
		{
			return this.propertyBag.TryGetProperty(property);
		}

		internal T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition)
		{
			return this.propertyBag.GetValueOrDefault<T>(propertyDefinition, default(T));
		}

		internal T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
		{
			return this.propertyBag.GetValueOrDefault<T>(propertyDefinition, defaultValue);
		}

		internal T? GetValueAsNullable<T>(StorePropertyDefinition propertyDefinition) where T : struct
		{
			return this.propertyBag.GetValueAsNullable<T>(propertyDefinition);
		}

		internal IDictionary<PropertyDefinition, object> PropertyValues
		{
			get
			{
				return this.propertyBag;
			}
		}

		internal PropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this.propertyBag[propertyDefinition];
			}
			set
			{
				this.propertyBag[propertyDefinition] = value;
			}
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			return this.propertyBag.GetProperties<PropertyDefinition>(propertyDefinitionArray);
		}

		private MemoryPropertyBag propertyBag;
	}
}
