using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	internal class EntityEnhancedLocationProperty : EntityNestedProperty
	{
		public EntityEnhancedLocationProperty(int protocolVersion, EntityPropertyDefinition propertyDefinition, PropertyType type = PropertyType.ReadWrite) : base(propertyDefinition, type)
		{
			base.NestedData = new EnhancedLocationData(protocolVersion);
		}

		public override INestedData NestedData
		{
			get
			{
				if (base.State == PropertyState.Uninitialized)
				{
					return base.NestedData;
				}
				if (base.EntityPropertyDefinition.Getter == null)
				{
					throw new ConversionException("Unable to get data of type " + base.EntityPropertyDefinition.Type.FullName);
				}
				Location location = (Location)base.EntityPropertyDefinition.Getter(base.Item);
				if (location == null)
				{
					base.State = PropertyState.SetToDefault;
					return base.NestedData;
				}
				EnhancedLocationData enhancedLocationData = base.NestedData as EnhancedLocationData;
				if (enhancedLocationData == null)
				{
					throw new UnexpectedTypeException("EnhancedLocationData", base.NestedData);
				}
				EntityEnhancedLocationProperty.PopulateLocation(enhancedLocationData, location);
				return enhancedLocationData;
			}
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			if (base.EntityPropertyDefinition.Setter == null)
			{
				throw new ConversionException("Unable to set data of type " + base.EntityPropertyDefinition.Type.FullName);
			}
			INestedProperty nestedProperty = srcProperty as INestedProperty;
			if (nestedProperty == null)
			{
				throw new UnexpectedTypeException("INestedProperty", srcProperty);
			}
			EnhancedLocationData enhancedLocationData = nestedProperty.NestedData as EnhancedLocationData;
			if (enhancedLocationData == null)
			{
				throw new UnexpectedTypeException("EnhancedLocationData", nestedProperty.NestedData);
			}
			Location arg = EntityEnhancedLocationProperty.CreateLocation(enhancedLocationData);
			base.EntityPropertyDefinition.Setter(base.Item, arg);
		}

		private static Location CreateLocation(EnhancedLocationData enhancedLocationData)
		{
			Location location = new Location();
			location.DisplayName = enhancedLocationData.DisplayName;
			location.Annotation = enhancedLocationData.Annotation;
			PostalAddress postalAddress = new PostalAddress();
			postalAddress.Street = enhancedLocationData.Street;
			postalAddress.City = enhancedLocationData.City;
			postalAddress.State = enhancedLocationData.State;
			postalAddress.Country = enhancedLocationData.Country;
			postalAddress.PostalCode = enhancedLocationData.PostalCode;
			postalAddress.Latitude = EntityEnhancedLocationProperty.StringToNullableDouble(enhancedLocationData.Latitude);
			postalAddress.Longitude = EntityEnhancedLocationProperty.StringToNullableDouble(enhancedLocationData.Longitude);
			postalAddress.Accuracy = EntityEnhancedLocationProperty.StringToNullableDouble(enhancedLocationData.Accuracy);
			postalAddress.Altitude = EntityEnhancedLocationProperty.StringToNullableDouble(enhancedLocationData.Altitude);
			postalAddress.AltitudeAccuracy = EntityEnhancedLocationProperty.StringToNullableDouble(enhancedLocationData.AltitudeAccuracy);
			postalAddress.LocationUri = enhancedLocationData.LocationUri;
			if (!postalAddress.IsEmpty())
			{
				location.Address = postalAddress;
			}
			return location;
		}

		private static void PopulateLocation(EnhancedLocationData enhancedLocationData, Location location)
		{
			enhancedLocationData.DisplayName = location.DisplayName;
			enhancedLocationData.Annotation = location.Annotation;
			PostalAddress address = location.Address;
			if (address != null && !address.IsEmpty())
			{
				enhancedLocationData.Street = address.Street;
				enhancedLocationData.City = address.City;
				enhancedLocationData.State = address.State;
				enhancedLocationData.Country = address.Country;
				enhancedLocationData.PostalCode = address.PostalCode;
				enhancedLocationData.Latitude = EntityEnhancedLocationProperty.NullableDoubleToString(address.Latitude);
				enhancedLocationData.Longitude = EntityEnhancedLocationProperty.NullableDoubleToString(address.Longitude);
				enhancedLocationData.Accuracy = EntityEnhancedLocationProperty.NullableDoubleToString(address.Accuracy);
				enhancedLocationData.Altitude = EntityEnhancedLocationProperty.NullableDoubleToString(address.Altitude);
				enhancedLocationData.AltitudeAccuracy = EntityEnhancedLocationProperty.NullableDoubleToString(address.AltitudeAccuracy);
				enhancedLocationData.LocationUri = address.LocationUri;
			}
		}

		private static double? StringToNullableDouble(string value)
		{
			double num;
			if (!double.TryParse(value, out num) || double.IsNaN(num))
			{
				return null;
			}
			return new double?(num);
		}

		private static string NullableDoubleToString(double? value)
		{
			if (value == null || double.IsNaN(value.Value))
			{
				return null;
			}
			return value.Value.ToString();
		}
	}
}
