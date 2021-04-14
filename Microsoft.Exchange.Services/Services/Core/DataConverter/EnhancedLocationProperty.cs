using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class EnhancedLocationProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		private EnhancedLocationProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static EnhancedLocationProperty CreateCommand(CommandContext commandContext)
		{
			return new EnhancedLocationProperty(commandContext);
		}

		private static T GetValueOrDefault<T>(IDictionary<PropertyDefinition, object> propertyBag, PropertyDefinition propertyDefinition, T defaultValue)
		{
			T result;
			if (PropertyCommand.TryGetValueFromPropertyBag<T>(propertyBag, propertyDefinition, out result))
			{
				return result;
			}
			return defaultValue;
		}

		private static EnhancedLocationType Render(string location, string locationDisplayName, string locationAnnotation, double? latitude, double? longitude, double? accuracy, double? altitude, double? altitudeAccuracy, string locationStreet, string locationCity, string locationState, string locationCountry, string locationPostalCode, LocationSourceType locationSource, string locationUri, ExDateTime? startTime)
		{
			if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
			{
				return null;
			}
			latitude = ((latitude != null && !double.IsNaN(latitude.Value)) ? latitude : null);
			longitude = ((longitude != null && !double.IsNaN(longitude.Value)) ? longitude : null);
			accuracy = ((accuracy != null && !double.IsNaN(accuracy.Value)) ? accuracy : null);
			altitude = ((altitude != null && !double.IsNaN(altitude.Value)) ? altitude : null);
			altitudeAccuracy = ((altitudeAccuracy != null && !double.IsNaN(altitudeAccuracy.Value)) ? altitudeAccuracy : null);
			EnhancedLocationType enhancedLocationType = (locationSource == LocationSourceType.None) ? new EnhancedLocationType
			{
				DisplayName = location,
				PostalAddress = new Microsoft.Exchange.Services.Core.Types.PostalAddress()
			} : new EnhancedLocationType
			{
				DisplayName = locationDisplayName,
				Annotation = locationAnnotation,
				PostalAddress = new Microsoft.Exchange.Services.Core.Types.PostalAddress
				{
					Latitude = latitude,
					Longitude = longitude,
					Accuracy = accuracy,
					Altitude = altitude,
					AltitudeAccuracy = altitudeAccuracy,
					Street = locationStreet,
					City = locationCity,
					State = locationState,
					Country = locationCountry,
					PostalCode = locationPostalCode,
					LocationSource = locationSource,
					LocationUri = locationUri
				}
			};
			ExDateTime t = ExDateTime.UtcNow.AddYears(-1);
			if (startTime != null && startTime < t)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<ExDateTime?>(0L, "[EnhancedLocationProperty::Render]: Ignoring geo-coordinates for event dated '{0}'", startTime);
				enhancedLocationType.PostalAddress.Latitude = null;
				enhancedLocationType.PostalAddress.Longitude = null;
				enhancedLocationType.PostalAddress.Accuracy = null;
				enhancedLocationType.PostalAddress.Altitude = null;
				enhancedLocationType.PostalAddress.AltitudeAccuracy = null;
				if (enhancedLocationType.PostalAddress.LocationSource != LocationSourceType.PhonebookServices)
				{
					enhancedLocationType.DisplayName = location;
					enhancedLocationType.PostalAddress.LocationUri = string.Empty;
					enhancedLocationType.PostalAddress.LocationSource = LocationSourceType.None;
				}
			}
			return enhancedLocationType;
		}

		private static void SetProperty(StoreObject storeObject, EnhancedLocationType enhancedLocation)
		{
			CalendarItemBase calendarItemBase = storeObject as CalendarItemBase;
			if (calendarItemBase != null)
			{
				if (calendarItemBase.IsNew)
				{
					EnhancedLocationProperty.LogDataPoints(enhancedLocation);
				}
				if (enhancedLocation.PostalAddress.LocationSource == LocationSourceType.None)
				{
					calendarItemBase.SetOrDeleteProperty(CalendarItemBaseSchema.Location, enhancedLocation.DisplayName);
					return;
				}
				calendarItemBase.LocationDisplayName = (enhancedLocation.DisplayName ?? string.Empty);
				calendarItemBase.LocationAnnotation = (enhancedLocation.Annotation ?? string.Empty);
				calendarItemBase.LocationSource = (LocationSource)enhancedLocation.PostalAddress.LocationSource;
				calendarItemBase.LocationUri = (enhancedLocation.PostalAddress.LocationUri ?? string.Empty);
				CalendarItemBase calendarItemBase2 = calendarItemBase;
				double? latitude = enhancedLocation.PostalAddress.Latitude;
				calendarItemBase2.Latitude = new double?((latitude != null) ? latitude.GetValueOrDefault() : double.NaN);
				CalendarItemBase calendarItemBase3 = calendarItemBase;
				double? longitude = enhancedLocation.PostalAddress.Longitude;
				calendarItemBase3.Longitude = new double?((longitude != null) ? longitude.GetValueOrDefault() : double.NaN);
				CalendarItemBase calendarItemBase4 = calendarItemBase;
				double? accuracy = enhancedLocation.PostalAddress.Accuracy;
				calendarItemBase4.Accuracy = new double?((accuracy != null) ? accuracy.GetValueOrDefault() : double.NaN);
				CalendarItemBase calendarItemBase5 = calendarItemBase;
				double? altitude = enhancedLocation.PostalAddress.Altitude;
				calendarItemBase5.Altitude = new double?((altitude != null) ? altitude.GetValueOrDefault() : double.NaN);
				CalendarItemBase calendarItemBase6 = calendarItemBase;
				double? altitudeAccuracy = enhancedLocation.PostalAddress.AltitudeAccuracy;
				calendarItemBase6.AltitudeAccuracy = new double?((altitudeAccuracy != null) ? altitudeAccuracy.GetValueOrDefault() : double.NaN);
				calendarItemBase.LocationStreet = (enhancedLocation.PostalAddress.Street ?? string.Empty);
				calendarItemBase.LocationCity = (enhancedLocation.PostalAddress.City ?? string.Empty);
				calendarItemBase.LocationState = (enhancedLocation.PostalAddress.State ?? string.Empty);
				calendarItemBase.LocationCountry = (enhancedLocation.PostalAddress.Country ?? string.Empty);
				calendarItemBase.LocationPostalCode = (enhancedLocation.PostalAddress.PostalCode ?? string.Empty);
				if (enhancedLocation.PostalAddress.LocationSource != LocationSourceType.None && enhancedLocation.PostalAddress.LocationSource != LocationSourceType.Resource && !string.IsNullOrEmpty(enhancedLocation.PostalAddress.LocationUri))
				{
					MailboxSession mailboxSession = (MailboxSession)calendarItemBase.Session;
					StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Location);
					if (defaultFolderId != null)
					{
						ComparisonFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ContactSchema.WorkLocationUri, enhancedLocation.PostalAddress.LocationUri);
						using (Folder folder = Folder.Bind(mailboxSession, defaultFolderId))
						{
							using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, queryFilter, null, new PropertyDefinition[]
							{
								ItemSchema.Id
							}))
							{
								object[][] rows = queryResult.GetRows(1);
								if (rows.Length == 0)
								{
									using (Place place = Place.Create(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Location)))
									{
										EnhancedLocationProperty.UpdatePlace(place, enhancedLocation);
										place.Save(SaveMode.NoConflictResolution);
										goto IL_329;
									}
								}
								VersionedId storeId = rows[0][0] as VersionedId;
								using (Place place2 = Place.Bind(mailboxSession, storeId, new PropertyDefinition[0]))
								{
									EnhancedLocationProperty.UpdatePlace(place2, enhancedLocation);
									place2.Save(SaveMode.NoConflictResolution);
								}
								IL_329:;
							}
						}
					}
				}
			}
		}

		private static void UpdatePlace(Place place, EnhancedLocationType enhancedLocation)
		{
			place.SetOrDeleteProperty(ContactSchema.GivenName, enhancedLocation.DisplayName);
			place.RelevanceRank++;
			place.WorkLocationSource = (LocationSource)enhancedLocation.PostalAddress.LocationSource;
			place.WorkLocationUri = enhancedLocation.PostalAddress.LocationUri;
			place.WorkLatitude = enhancedLocation.PostalAddress.Latitude;
			place.WorkLongitude = enhancedLocation.PostalAddress.Longitude;
			place.WorkAccuracy = enhancedLocation.PostalAddress.Accuracy;
			place.WorkAltitude = enhancedLocation.PostalAddress.Altitude;
			place.WorkAltitudeAccuracy = enhancedLocation.PostalAddress.AltitudeAccuracy;
			place.SetOrDeleteProperty(ContactSchema.WorkAddressStreet, enhancedLocation.PostalAddress.Street);
			place.SetOrDeleteProperty(ContactSchema.WorkAddressCity, enhancedLocation.PostalAddress.City);
			place.SetOrDeleteProperty(ContactSchema.WorkAddressState, enhancedLocation.PostalAddress.State);
			place.SetOrDeleteProperty(ContactSchema.WorkAddressCountry, enhancedLocation.PostalAddress.Country);
			place.SetOrDeleteProperty(ContactSchema.WorkAddressPostalCode, enhancedLocation.PostalAddress.PostalCode);
		}

		private static void LogDataPoints(EnhancedLocationType enhancedLocation)
		{
			CallContext callContext = CallContext.Current;
			bool flag = !string.IsNullOrEmpty(enhancedLocation.DisplayName);
			callContext.ProtocolLog.Set(EnhancedLocationMetadata.HasLocation, flag);
			callContext.ProtocolLog.Set(EnhancedLocationMetadata.LocationSource, enhancedLocation.PostalAddress.LocationSource);
			bool flag2 = !string.IsNullOrEmpty(enhancedLocation.Annotation);
			callContext.ProtocolLog.Set(EnhancedLocationMetadata.HasAnnotation, flag2);
			bool flag3 = enhancedLocation.PostalAddress.Latitude != null && enhancedLocation.PostalAddress.Longitude != null;
			callContext.ProtocolLog.Set(EnhancedLocationMetadata.HasCoordinates, flag3);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("EnhancedLocationProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("EnhancedLocationProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			serviceObject[CalendarItemSchema.EnhancedLocation] = EnhancedLocationProperty.Render(EnhancedLocationProperty.GetValueOrDefault<string>(propertyBag, CalendarItemBaseSchema.Location, string.Empty), EnhancedLocationProperty.GetValueOrDefault<string>(propertyBag, CalendarItemBaseSchema.LocationDisplayName, string.Empty), EnhancedLocationProperty.GetValueOrDefault<string>(propertyBag, CalendarItemBaseSchema.LocationAnnotation, string.Empty), EnhancedLocationProperty.GetValueOrDefault<double?>(propertyBag, CalendarItemBaseSchema.Latitude, new double?(double.NaN)), EnhancedLocationProperty.GetValueOrDefault<double?>(propertyBag, CalendarItemBaseSchema.Longitude, new double?(double.NaN)), EnhancedLocationProperty.GetValueOrDefault<double?>(propertyBag, CalendarItemBaseSchema.Accuracy, new double?(double.NaN)), EnhancedLocationProperty.GetValueOrDefault<double?>(propertyBag, CalendarItemBaseSchema.Altitude, new double?(double.NaN)), EnhancedLocationProperty.GetValueOrDefault<double?>(propertyBag, CalendarItemBaseSchema.AltitudeAccuracy, new double?(double.NaN)), EnhancedLocationProperty.GetValueOrDefault<string>(propertyBag, CalendarItemBaseSchema.LocationStreet, string.Empty), EnhancedLocationProperty.GetValueOrDefault<string>(propertyBag, CalendarItemBaseSchema.LocationCity, string.Empty), EnhancedLocationProperty.GetValueOrDefault<string>(propertyBag, CalendarItemBaseSchema.LocationState, string.Empty), EnhancedLocationProperty.GetValueOrDefault<string>(propertyBag, CalendarItemBaseSchema.LocationCountry, string.Empty), EnhancedLocationProperty.GetValueOrDefault<string>(propertyBag, CalendarItemBaseSchema.LocationPostalCode, string.Empty), (LocationSourceType)EnhancedLocationProperty.GetValueOrDefault<int>(propertyBag, CalendarItemBaseSchema.LocationSource, 0), EnhancedLocationProperty.GetValueOrDefault<string>(propertyBag, CalendarItemBaseSchema.LocationUri, string.Empty), EnhancedLocationProperty.GetValueOrDefault<ExDateTime?>(propertyBag, CalendarItemInstanceSchema.StartTime, null));
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			if (storeObject is CalendarItemBase || storeObject is MeetingMessage)
			{
				serviceObject[CalendarItemSchema.EnhancedLocation] = EnhancedLocationProperty.Render(storeObject.GetValueOrDefault<string>(CalendarItemBaseSchema.Location, string.Empty), storeObject.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationDisplayName, string.Empty), storeObject.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationAnnotation, string.Empty), storeObject.GetValueOrDefault<double?>(CalendarItemBaseSchema.Latitude, new double?(double.NaN)), storeObject.GetValueOrDefault<double?>(CalendarItemBaseSchema.Longitude, new double?(double.NaN)), storeObject.GetValueOrDefault<double?>(CalendarItemBaseSchema.Accuracy, new double?(double.NaN)), storeObject.GetValueOrDefault<double?>(CalendarItemBaseSchema.Altitude, new double?(double.NaN)), storeObject.GetValueOrDefault<double?>(CalendarItemBaseSchema.AltitudeAccuracy, new double?(double.NaN)), storeObject.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationStreet, string.Empty), storeObject.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationCity, string.Empty), storeObject.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationState, string.Empty), storeObject.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationCountry, string.Empty), storeObject.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationPostalCode, string.Empty), (LocationSourceType)storeObject.GetValueOrDefault<int>(CalendarItemBaseSchema.LocationSource), storeObject.GetValueOrDefault<string>(CalendarItemBaseSchema.LocationUri, string.Empty), storeObject.GetValueAsNullable<ExDateTime>(CalendarItemInstanceSchema.StartTime));
				return;
			}
			serviceObject[CalendarItemSchema.EnhancedLocation] = null;
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			EnhancedLocationType enhancedLocation = (EnhancedLocationType)commandSettings.ServiceObject.PropertyBag[CalendarItemSchema.EnhancedLocation];
			EnhancedLocationProperty.SetProperty(commandSettings.StoreObject, enhancedLocation);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			EnhancedLocationType valueOrDefault = setPropertyUpdate.ServiceObject.GetValueOrDefault<EnhancedLocationType>(CalendarItemSchema.EnhancedLocation);
			EnhancedLocationProperty.SetProperty(updateCommandSettings.StoreObject, valueOrDefault);
		}

		internal static readonly PropertyDefinition[] LocationProperties = CalendarItemProperties.EnhancedLocationPropertyDefinitions.Concat(new PropertyDefinition[]
		{
			CalendarItemBaseSchema.Location,
			CalendarItemInstanceSchema.StartTime
		}).ToArray<PropertyDefinition>();
	}
}
