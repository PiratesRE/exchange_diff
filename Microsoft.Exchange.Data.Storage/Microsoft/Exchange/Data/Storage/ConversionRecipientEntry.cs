using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConversionRecipientEntry
	{
		internal ConversionRecipientEntry()
		{
			this.propertyValues = new MemoryPropertyBag();
			this.propertyValues.SetAllPropertiesLoaded();
		}

		internal ConversionRecipientEntry(Participant participant, RecipientItemType recipientItemType)
		{
			this.propertyValues = new MemoryPropertyBag();
			this.propertyValues.SetAllPropertiesLoaded();
			this.Participant = participant;
			this.recipientItemType = recipientItemType;
		}

		internal void SetProperty(StorePropertyDefinition property, object value, bool resetOldValue)
		{
			if (property.Equals(InternalSchema.RowId))
			{
				return;
			}
			if (property.Equals(InternalSchema.RecipientType))
			{
				this.recipientItemType = MapiUtil.MapiRecipientTypeToRecipientItemType((RecipientType)((int)value));
				return;
			}
			if (resetOldValue || !this.IsPropertySetOrDeleted(property))
			{
				this.propertyValues[property] = value;
			}
			this.cachedParticipant = null;
		}

		private void DeleteProperty(StorePropertyDefinition property, bool resetOldValue)
		{
			if (resetOldValue || !this.IsPropertySetOrDeleted(property))
			{
				this.propertyValues.Delete(property);
			}
		}

		private bool IsPropertySetOrDeleted(StorePropertyDefinition property)
		{
			if (this.propertyValues.DeleteList.Contains(property))
			{
				return true;
			}
			object obj = this.propertyValues.TryGetProperty(property);
			return !(obj is PropertyError);
		}

		internal object TryGetProperty(StorePropertyDefinition property)
		{
			return this.propertyValues.TryGetProperty(property);
		}

		internal RecipientItemType RecipientItemType
		{
			get
			{
				return this.recipientItemType;
			}
		}

		internal ICollection<NativeStorePropertyDefinition> AllNativeProperties
		{
			get
			{
				return this.propertyValues.AllNativeProperties;
			}
		}

		internal Participant Participant
		{
			get
			{
				if (this.cachedParticipant == null)
				{
					this.cachedParticipant = this.propertyValues.GetValueOrDefault<Participant>(InternalSchema.RecipientBaseParticipant);
				}
				return this.cachedParticipant;
			}
			set
			{
				this.cachedParticipant = null;
				this.propertyValues.SetOrDeleteProperty(InternalSchema.RecipientBaseParticipant, value);
			}
		}

		internal object TryGetProperty(NativeStorePropertyDefinition property)
		{
			return this.propertyValues.TryGetProperty(property);
		}

		internal T GetValueOrDefault<T>(NativeStorePropertyDefinition propertyDefinition)
		{
			return this.GetValueOrDefault<T>(propertyDefinition, default(T));
		}

		internal T GetValueOrDefault<T>(NativeStorePropertyDefinition propertyDefinition, T defaultValue)
		{
			return PropertyBag.CheckPropertyValue<T>(propertyDefinition, this.TryGetProperty(propertyDefinition), defaultValue);
		}

		internal void CopyDependentProperties(ConversionRecipientEntry dependentEntry)
		{
			if (dependentEntry == null)
			{
				return;
			}
			foreach (NativeStorePropertyDefinition property in dependentEntry.AllNativeProperties)
			{
				object obj = dependentEntry.TryGetProperty(property);
				if (obj != null && !(obj is PropertyError))
				{
					this.SetProperty(property, obj, false);
				}
			}
			foreach (PropertyDefinition propertyDefinition in dependentEntry.propertyValues.DeleteList)
			{
				StorePropertyDefinition property2 = (StorePropertyDefinition)propertyDefinition;
				this.DeleteProperty(property2, false);
			}
		}

		public override bool Equals(object obj)
		{
			ConversionRecipientEntry conversionRecipientEntry = obj as ConversionRecipientEntry;
			return conversionRecipientEntry != null && (object.ReferenceEquals(this.Participant, conversionRecipientEntry.Participant) || (this.Participant != null && this.RecipientItemType == conversionRecipientEntry.RecipientItemType && this.Participant.AreAddressesEqual(conversionRecipientEntry.Participant)));
		}

		public override int GetHashCode()
		{
			int num = (int)((int)this.RecipientItemType << 16);
			if (this.Participant != null)
			{
				int num2 = num;
				string text;
				if ((text = this.Participant.RoutingType) == null)
				{
					text = (this.Participant.DisplayName ?? string.Empty);
				}
				num = (num2 ^ text.GetHashCode());
				num ^= (this.Participant.EmailAddress ?? string.Empty).ToLowerInvariant().GetHashCode();
			}
			return num;
		}

		private RecipientItemType recipientItemType;

		private MemoryPropertyBag propertyValues;

		private Participant cachedParticipant;
	}
}
