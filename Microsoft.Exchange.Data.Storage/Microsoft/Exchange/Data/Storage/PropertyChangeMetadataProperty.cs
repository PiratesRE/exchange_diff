using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class PropertyChangeMetadataProperty : SmartPropertyDefinition
	{
		internal PropertyChangeMetadataProperty() : base("PropertyChangeMetadataProperty", typeof(PropertyChangeMetadata), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.PropertyChangeMetadataRaw, PropertyDependencyType.AllRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			byte[] valueOrDefault = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.PropertyChangeMetadataRaw);
			if (valueOrDefault == null)
			{
				CalendarItemOccurrence calendarItemOccurrence = propertyBag.Context.StoreObject as CalendarItemOccurrence;
				if (calendarItemOccurrence == null)
				{
					return null;
				}
				return calendarItemOccurrence.OccurrencePropertyBag.ComputeChangeMetadataBasedOnLoadedProperties();
			}
			else
			{
				if (valueOrDefault.Length == 0)
				{
					return null;
				}
				object result;
				try
				{
					result = PropertyChangeMetadata.Parse(valueOrDefault);
				}
				catch (PropertyChangeMetadataFormatException ex)
				{
					result = new PropertyError(this, PropertyErrorCode.CorruptedData, ex.Message);
				}
				return result;
			}
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			PropertyChangeMetadata propertyChangeMetadata = value as PropertyChangeMetadata;
			if (propertyChangeMetadata == null)
			{
				throw new ArgumentException("value");
			}
			propertyBag.SetValue(InternalSchema.PropertyChangeMetadataRaw, propertyChangeMetadata.ToByteArray());
		}
	}
}
