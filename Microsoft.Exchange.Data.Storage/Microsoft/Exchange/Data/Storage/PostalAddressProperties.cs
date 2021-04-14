using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class PostalAddressProperties
	{
		private static NativeStorePropertyDefinition[] GetAllProperties(PostalAddressProperties[] propertySets, Func<PostalAddressProperties, NativeStorePropertyDefinition[]> getProperties)
		{
			List<NativeStorePropertyDefinition> list = new List<NativeStorePropertyDefinition>(propertySets.Length * 13 + PostalAddressProperties.AdditionalProperties.Length);
			foreach (PostalAddressProperties arg in propertySets)
			{
				list.AddRange(getProperties(arg));
			}
			list.AddRange(PostalAddressProperties.AdditionalProperties);
			return list.ToArray();
		}

		private PostalAddressProperties()
		{
		}

		public NativeStorePropertyDefinition Street { get; private set; }

		public NativeStorePropertyDefinition City { get; private set; }

		public NativeStorePropertyDefinition State { get; private set; }

		public NativeStorePropertyDefinition Country { get; private set; }

		public NativeStorePropertyDefinition PostalCode { get; private set; }

		public NativeStorePropertyDefinition PostOfficeBox { get; private set; }

		public NativeStorePropertyDefinition Latitude { get; private set; }

		public NativeStorePropertyDefinition Longitude { get; private set; }

		public NativeStorePropertyDefinition Accuracy { get; private set; }

		public NativeStorePropertyDefinition Altitude { get; private set; }

		public NativeStorePropertyDefinition AltitudeAccuracy { get; private set; }

		public NativeStorePropertyDefinition LocationSource { get; private set; }

		public NativeStorePropertyDefinition LocationUri { get; private set; }

		public PostalAddressType PostalAddressType { get; private set; }

		public NativeStorePropertyDefinition[] Properties
		{
			get
			{
				return new NativeStorePropertyDefinition[]
				{
					this.Street,
					this.City,
					this.State,
					this.Country,
					this.PostalCode,
					this.PostOfficeBox,
					this.Latitude,
					this.Longitude,
					this.Accuracy,
					this.Altitude,
					this.AltitudeAccuracy,
					this.LocationSource,
					this.LocationUri
				};
			}
		}

		private NativeStorePropertyDefinition[] PropertiesForConversationView
		{
			get
			{
				return new NativeStorePropertyDefinition[]
				{
					this.Street,
					this.City,
					this.State,
					this.Country,
					this.PostalCode,
					this.Latitude,
					this.Longitude,
					this.LocationSource,
					this.LocationUri
				};
			}
		}

		public PostalAddress GetFromAllProperties(IStorePropertyBag propertyBag)
		{
			PostalAddress fromAllPropertiesForConversationViewInternal = this.GetFromAllPropertiesForConversationViewInternal(propertyBag);
			fromAllPropertiesForConversationViewInternal.PostOfficeBox = propertyBag.GetValueOrDefault<string>(this.PostOfficeBox, null);
			fromAllPropertiesForConversationViewInternal.Accuracy = propertyBag.GetValueOrDefault<double?>(this.Accuracy, null);
			fromAllPropertiesForConversationViewInternal.Altitude = propertyBag.GetValueOrDefault<double?>(this.Altitude, null);
			fromAllPropertiesForConversationViewInternal.AltitudeAccuracy = propertyBag.GetValueOrDefault<double?>(this.AltitudeAccuracy, null);
			if (fromAllPropertiesForConversationViewInternal.IsEmpty())
			{
				return null;
			}
			return fromAllPropertiesForConversationViewInternal;
		}

		public PostalAddress GetFromAllPropertiesForConversationView(IStorePropertyBag propertyBag)
		{
			PostalAddress fromAllPropertiesForConversationViewInternal = this.GetFromAllPropertiesForConversationViewInternal(propertyBag);
			if (fromAllPropertiesForConversationViewInternal.IsEmpty())
			{
				return null;
			}
			return fromAllPropertiesForConversationViewInternal;
		}

		public void SetTo(IStorePropertyBag propertyBag, PostalAddress postalAddress)
		{
			PostalAddressProperties.SetOrDeleteValue<string>(propertyBag, this.Street, postalAddress.Street);
			PostalAddressProperties.SetOrDeleteValue<string>(propertyBag, this.City, postalAddress.City);
			PostalAddressProperties.SetOrDeleteValue<string>(propertyBag, this.State, postalAddress.State);
			PostalAddressProperties.SetOrDeleteValue<string>(propertyBag, this.Country, postalAddress.Country);
			PostalAddressProperties.SetOrDeleteValue<string>(propertyBag, this.PostalCode, postalAddress.PostalCode);
			PostalAddressProperties.SetOrDeleteValue<double>(propertyBag, this.Latitude, postalAddress.Latitude);
			PostalAddressProperties.SetOrDeleteValue<double>(propertyBag, this.Longitude, postalAddress.Longitude);
			PostalAddressProperties.SetOrDeleteValue<double>(propertyBag, this.Accuracy, postalAddress.Accuracy);
			PostalAddressProperties.SetOrDeleteValue<double>(propertyBag, this.Altitude, postalAddress.Altitude);
			PostalAddressProperties.SetOrDeleteValue<double>(propertyBag, this.AltitudeAccuracy, postalAddress.AltitudeAccuracy);
			PostalAddressProperties.SetOrDeleteValue(propertyBag, this.LocationSource, postalAddress.LocationSource);
			PostalAddressProperties.SetOrDeleteValue<string>(propertyBag, this.LocationUri, postalAddress.LocationUri);
		}

		private static void SetOrDeleteValue<T>(IStorePropertyBag propertyBag, NativeStorePropertyDefinition property, T value) where T : class
		{
			if (value == null)
			{
				propertyBag.Delete(property);
				return;
			}
			propertyBag[property] = value;
		}

		private static void SetOrDeleteValue<T>(IStorePropertyBag propertyBag, NativeStorePropertyDefinition property, T? value) where T : struct
		{
			if (value == null)
			{
				propertyBag.Delete(property);
				return;
			}
			propertyBag[property] = value.Value;
		}

		private static void SetOrDeleteValue(IStorePropertyBag propertyBag, NativeStorePropertyDefinition property, LocationSource value)
		{
			if (value == Microsoft.Exchange.Data.Storage.LocationSource.None)
			{
				propertyBag.Delete(property);
				return;
			}
			propertyBag[property] = value;
		}

		private PostalAddress GetFromAllPropertiesForConversationViewInternal(IStorePropertyBag propertyBag)
		{
			PostalAddress postalAddress = new PostalAddress
			{
				Street = propertyBag.GetValueOrDefault<string>(this.Street, null),
				City = propertyBag.GetValueOrDefault<string>(this.City, null),
				State = propertyBag.GetValueOrDefault<string>(this.State, null),
				Country = propertyBag.GetValueOrDefault<string>(this.Country, null),
				PostalCode = propertyBag.GetValueOrDefault<string>(this.PostalCode, null),
				Latitude = propertyBag.GetValueOrDefault<double?>(this.Latitude, null),
				Longitude = propertyBag.GetValueOrDefault<double?>(this.Longitude, null),
				LocationSource = propertyBag.GetValueOrDefault<LocationSource>(this.LocationSource, Microsoft.Exchange.Data.Storage.LocationSource.None),
				LocationUri = propertyBag.GetValueOrDefault<string>(this.LocationUri, null),
				Type = this.PostalAddressType
			};
			if (!postalAddress.IsEmpty() && postalAddress.LocationSource == Microsoft.Exchange.Data.Storage.LocationSource.None)
			{
				postalAddress.LocationSource = Microsoft.Exchange.Data.Storage.LocationSource.Contact;
				foreach (NativeStorePropertyDefinition propertyDefinition in PostalAddressProperties.AdditionalProperties)
				{
					string valueOrDefault = propertyBag.GetValueOrDefault<string>(propertyDefinition, null);
					if (valueOrDefault != null)
					{
						postalAddress.LocationUri = valueOrDefault;
						break;
					}
				}
			}
			return postalAddress;
		}

		private static readonly NativeStorePropertyDefinition[] AdditionalProperties = new NativeStorePropertyDefinition[]
		{
			InternalSchema.Email1EmailAddress,
			InternalSchema.Email2EmailAddress,
			InternalSchema.Email3EmailAddress
		};

		public static readonly PostalAddressProperties WorkAddress = new PostalAddressProperties
		{
			Street = InternalSchema.WorkAddressStreet,
			City = InternalSchema.WorkAddressCity,
			State = InternalSchema.WorkAddressState,
			Country = InternalSchema.WorkAddressCountry,
			PostalCode = InternalSchema.WorkAddressPostalCode,
			PostOfficeBox = InternalSchema.WorkPostOfficeBox,
			Latitude = InternalSchema.WorkLatitude,
			Longitude = InternalSchema.WorkLongitude,
			Accuracy = InternalSchema.WorkAccuracy,
			Altitude = InternalSchema.WorkAltitude,
			AltitudeAccuracy = InternalSchema.WorkAltitudeAccuracy,
			LocationSource = InternalSchema.WorkLocationSource,
			LocationUri = InternalSchema.WorkLocationUri,
			PostalAddressType = PostalAddressType.Business
		};

		public static readonly PostalAddressProperties HomeAddress = new PostalAddressProperties
		{
			Street = InternalSchema.HomeStreet,
			City = InternalSchema.HomeCity,
			State = InternalSchema.HomeState,
			Country = InternalSchema.HomeCountry,
			PostalCode = InternalSchema.HomePostalCode,
			PostOfficeBox = InternalSchema.HomePostOfficeBox,
			Latitude = InternalSchema.HomeLatitude,
			Longitude = InternalSchema.HomeLongitude,
			Accuracy = InternalSchema.HomeAccuracy,
			Altitude = InternalSchema.HomeAltitude,
			AltitudeAccuracy = InternalSchema.HomeAltitudeAccuracy,
			LocationSource = InternalSchema.HomeLocationSource,
			LocationUri = InternalSchema.HomeLocationUri,
			PostalAddressType = PostalAddressType.Home
		};

		public static readonly PostalAddressProperties OtherAddress = new PostalAddressProperties
		{
			Street = InternalSchema.OtherStreet,
			City = InternalSchema.OtherCity,
			State = InternalSchema.OtherState,
			Country = InternalSchema.OtherCountry,
			PostalCode = InternalSchema.OtherPostalCode,
			PostOfficeBox = InternalSchema.OtherPostOfficeBox,
			Latitude = InternalSchema.OtherLatitude,
			Longitude = InternalSchema.OtherLongitude,
			Accuracy = InternalSchema.OtherAccuracy,
			Altitude = InternalSchema.OtherAltitude,
			AltitudeAccuracy = InternalSchema.OtherAltitudeAccuracy,
			LocationSource = InternalSchema.OtherLocationSource,
			LocationUri = InternalSchema.OtherLocationUri,
			PostalAddressType = PostalAddressType.Other
		};

		public static readonly PostalAddressProperties[] PropertySets = new PostalAddressProperties[]
		{
			PostalAddressProperties.WorkAddress,
			PostalAddressProperties.HomeAddress,
			PostalAddressProperties.OtherAddress
		};

		public static readonly NativeStorePropertyDefinition[] AllProperties = PostalAddressProperties.GetAllProperties(PostalAddressProperties.PropertySets, (PostalAddressProperties propertySet) => propertySet.Properties);

		public static NativeStorePropertyDefinition[] AllPropertiesForConversationView = PostalAddressProperties.GetAllProperties(PostalAddressProperties.PropertySets, (PostalAddressProperties propertySet) => propertySet.PropertiesForConversationView);
	}
}
