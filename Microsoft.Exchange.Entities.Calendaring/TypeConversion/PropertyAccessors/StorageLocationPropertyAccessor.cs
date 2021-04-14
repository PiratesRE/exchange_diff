using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors
{
	internal sealed class StorageLocationPropertyAccessor : StoragePropertyAccessor<ICalendarItemBase, Location>, IPropertyValueCollectionAccessor<ICalendarItemBase, Microsoft.Exchange.Data.PropertyDefinition, Location>, IPropertyAccessor<ICalendarItemBase, Location>
	{
		public StorageLocationPropertyAccessor() : base(false, PropertyChangeMetadata.PropertyGroup.Location, null)
		{
		}

		public bool TryGetValue(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, out Location location)
		{
			int index;
			object obj;
			if ((propertyIndices.TryGetValue(CalendarItemBaseSchema.Location, out index) && (obj = values[index]) is string) || (propertyIndices.TryGetValue(CalendarItemBaseSchema.LocationDisplayName, out index) && (obj = values[index]) is string))
			{
				location = new Location
				{
					DisplayName = (string)obj
				};
				location.Annotation = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.LocationAnnotation, out index) && (obj = values[index]) is string) ? ((string)obj) : string.Empty);
				location.Address = new PostalAddress();
				location.Address.Street = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.LocationStreet, out index) && (obj = values[index]) is string) ? ((string)obj) : string.Empty);
				location.Address.City = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.LocationCity, out index) && (obj = values[index]) is string) ? ((string)obj) : string.Empty);
				location.Address.State = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.LocationState, out index) && (obj = values[index]) is string) ? ((string)obj) : string.Empty);
				location.Address.PostalCode = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.LocationPostalCode, out index) && (obj = values[index]) is string) ? ((string)obj) : string.Empty);
				location.Address.Country = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.LocationCountry, out index) && (obj = values[index]) is string) ? ((string)obj) : string.Empty);
				location.Address.Latitude = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.Latitude, out index) && (obj = values[index]) is double?) ? ((double?)obj) : null);
				location.Address.Longitude = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.Longitude, out index) && (obj = values[index]) is double?) ? ((double?)obj) : null);
				location.Address.Accuracy = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.Accuracy, out index) && (obj = values[index]) is double?) ? ((double?)obj) : null);
				location.Address.Altitude = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.Altitude, out index) && (obj = values[index]) is double?) ? ((double?)obj) : null);
				location.Address.AltitudeAccuracy = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.AltitudeAccuracy, out index) && (obj = values[index]) is double?) ? ((double?)obj) : null);
				location.Address.LocationSource = ((propertyIndices.TryGetValue(CalendarItemBaseSchema.LocationSource, out index) && (obj = values[index]) is LocationSource) ? ((LocationSource)obj) : LocationSource.None);
				return true;
			}
			location = null;
			return false;
		}

		protected override bool PerformTryGetValue(ICalendarItemBase storeItem, out Location location)
		{
			location = this.GetEntityLocation(storeItem);
			return true;
		}

		protected override void PerformSet(ICalendarItemBase storeItem, Location location)
		{
			this.SetOnStorageItem(location, storeItem);
		}

		private Location GetEntityLocation(ICalendarItemBase input)
		{
			Location location = new Location
			{
				DisplayName = (string.IsNullOrEmpty(input.LocationDisplayName) ? input.Location : input.LocationDisplayName),
				Annotation = input.LocationAnnotation,
				Address = new PostalAddress
				{
					Street = input.LocationStreet,
					City = input.LocationCity,
					State = input.LocationState,
					PostalCode = input.LocationPostalCode,
					Country = input.LocationCountry,
					Longitude = input.Longitude,
					Latitude = input.Latitude,
					Accuracy = input.Accuracy,
					Altitude = input.Altitude,
					AltitudeAccuracy = input.AltitudeAccuracy,
					LocationSource = input.LocationSource
				}
			};
			if (location.Address.IsEmpty())
			{
				location.Address = null;
			}
			return location;
		}

		private void SetOnStorageItem(Location entityLocation, ICalendarItemBase storageItem)
		{
			storageItem.LocationDisplayName = entityLocation.DisplayName;
			storageItem.LocationAnnotation = entityLocation.Annotation;
			if (entityLocation.Address != null && !entityLocation.Address.IsEmpty())
			{
				storageItem.LocationStreet = entityLocation.Address.Street;
				storageItem.LocationCity = entityLocation.Address.City;
				storageItem.LocationPostalCode = entityLocation.Address.PostalCode;
				storageItem.LocationCountry = entityLocation.Address.Country;
				storageItem.LocationSource = entityLocation.Address.LocationSource;
				storageItem.Latitude = entityLocation.Address.Latitude;
				storageItem.Longitude = entityLocation.Address.Longitude;
				storageItem.Accuracy = entityLocation.Address.Accuracy;
				storageItem.Altitude = entityLocation.Address.Altitude;
				storageItem.AltitudeAccuracy = entityLocation.Address.AltitudeAccuracy;
			}
		}
	}
}
