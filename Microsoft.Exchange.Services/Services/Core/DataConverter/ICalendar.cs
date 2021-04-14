using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ICalendar
	{
		private static readonly PropertyDefinition GlobalObjectId = CalendarItemBaseSchema.GlobalObjectId;

		private static readonly PropertyDefinition CleanGlobalObjectId = CalendarItemBaseSchema.CleanGlobalObjectId;

		private static readonly PropertyDefinition StartRecurTime = CalendarItemBaseSchema.StartRecurTime;

		private static readonly PropertyDefinition TimeZoneBlob = CalendarItemBaseSchema.TimeZoneBlob;

		private static readonly PropertyDefinition TimeZoneDefinitionRecurring = CalendarItemBaseSchema.TimeZoneDefinitionRecurring;

		private static readonly PropertyDefinition TimeZoneDefinitionStart = ItemSchema.TimeZoneDefinitionStart;

		private static readonly PropertyDefinition OwnerCriticalChangeTime = CalendarItemBaseSchema.OwnerCriticalChangeTime;

		private static readonly PropertyDefinition AttendeeCriticalChangeTime = CalendarItemBaseSchema.AttendeeCriticalChangeTime;

		private static readonly PropertyDefinition LastModifiedTime = StoreObjectSchema.LastModifiedTime;

		private static readonly PropertyDefinition ItemClass = StoreObjectSchema.ItemClass;

		internal sealed class UidProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
		{
			private UidProperty(CommandContext commandContext) : base(commandContext)
			{
			}

			public static ICalendar.UidProperty CreateCommand(CommandContext commandContext)
			{
				return new ICalendar.UidProperty(commandContext);
			}

			public static bool TryGetUidFromStoreObject(StoreObject storeObject, PropertyPath uidPropertyPath, out string uid)
			{
				if (PropertyCommand.StorePropertyExists(storeObject, ICalendar.CleanGlobalObjectId))
				{
					byte[] globalObjectIdBytes = storeObject.TryGetProperty(ICalendar.CleanGlobalObjectId) as byte[];
					uid = ICalendar.UidProperty.GetUidFromGoidBytes(globalObjectIdBytes, uidPropertyPath);
					return true;
				}
				uid = null;
				return false;
			}

			public static bool TryGetUidFromPropertyBag(IStorePropertyBag propertyBag, out string uid)
			{
				uid = null;
				byte[] array = propertyBag.TryGetProperty(CalendarItemBaseSchema.CleanGlobalObjectId) as byte[];
				if (array == null)
				{
					return false;
				}
				uid = ICalendar.UidProperty.GetUidFromGoidBytes(array, CalendarItemSchema.ICalendarUid.PropertyPath);
				return true;
			}

			public void Set()
			{
				SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
				CalendarItem calendarItem = commandSettings.StoreObject as CalendarItem;
				this.SetProperty(calendarItem, commandSettings.ServiceObject);
			}

			public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
			{
				ServiceObject serviceObject = setPropertyUpdate.ServiceObject;
				CalendarItem calendarItem = updateCommandSettings.StoreObject as CalendarItem;
				if (calendarItem == null)
				{
					throw new InvalidPropertySetException((CoreResources.IDs)4292861306U, updateCommandSettings.PropertyUpdate.PropertyPath);
				}
				this.SetProperty(calendarItem, serviceObject);
			}

			public void ToXml()
			{
				throw new InvalidOperationException("UidProperty.ToXml should not be called.");
			}

			public void ToXmlForPropertyBag()
			{
				throw new InvalidOperationException("UidProperty.ToXmlForPropertyBag should not be called.");
			}

			public void ToServiceObject()
			{
				ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
				StoreObject storeObject = commandSettings.StoreObject;
				ServiceObject serviceObject = commandSettings.ServiceObject;
				string value;
				if (ICalendar.UidProperty.TryGetUidFromStoreObject(storeObject, this.commandContext.PropertyInformation.PropertyPath, out value))
				{
					serviceObject[this.commandContext.PropertyInformation] = value;
				}
			}

			public void ToServiceObjectForPropertyBag()
			{
				ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
				IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
				ServiceObject serviceObject = commandSettings.ServiceObject;
				byte[] globalObjectIdBytes;
				if (PropertyCommand.TryGetValueFromPropertyBag<byte[]>(propertyBag, ICalendar.CleanGlobalObjectId, out globalObjectIdBytes))
				{
					serviceObject[this.commandContext.PropertyInformation] = ICalendar.UidProperty.GetUidFromGoidBytes(globalObjectIdBytes, this.commandContext.PropertyInformation.PropertyPath);
				}
			}

			private void SetProperty(CalendarItem calendarItem, ServiceObject serviceObject)
			{
				string valueOrDefault = serviceObject.GetValueOrDefault<string>(this.commandContext.PropertyInformation);
				try
				{
					GlobalObjectId globalObjectId = new GlobalObjectId(valueOrDefault);
					calendarItem[ICalendar.GlobalObjectId] = globalObjectId.Bytes;
					calendarItem[ICalendar.CleanGlobalObjectId] = globalObjectId.Bytes;
				}
				catch (Exception innerException)
				{
					throw new CalendarExceptionInvalidPropertyValue(this.commandContext.PropertyInformation.PropertyPath, innerException);
				}
			}

			private static string GetUidFromGoidBytes(byte[] globalObjectIdBytes, PropertyPath uidPropertyPath)
			{
				string uid;
				try
				{
					GlobalObjectId globalObjectId = new GlobalObjectId(globalObjectIdBytes);
					uid = globalObjectId.Uid;
				}
				catch (Exception innerException)
				{
					throw new CalendarExceptionInvalidPropertyValue(uidPropertyPath, innerException);
				}
				return uid;
			}

			public static readonly PropertyDefinition PropertyToLoad = ICalendar.CleanGlobalObjectId;
		}

		internal sealed class RecurrenceIdProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
		{
			private RecurrenceIdProperty(CommandContext commandContext) : base(commandContext)
			{
			}

			public static ICalendar.RecurrenceIdProperty CreateCommand(CommandContext commandContext)
			{
				return new ICalendar.RecurrenceIdProperty(commandContext);
			}

			public void ToXml()
			{
				throw new InvalidOperationException("RecurrenceIdProperty.ToXml should not be called.");
			}

			public void ToXmlForPropertyBag()
			{
				throw new InvalidOperationException("RecurrenceIdProperty.ToXmlForPropertyBag should not be called.");
			}

			public void ToServiceObject()
			{
				ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
				StoreObject storeObject = commandSettings.StoreObject;
				ServiceObject serviceObject = commandSettings.ServiceObject;
				ExTimeZone recurringTimeZoneFromPropertyBag = TimeZoneHelper.GetRecurringTimeZoneFromPropertyBag(storeObject.PropertyBag);
				if (recurringTimeZoneFromPropertyBag == null)
				{
					byte[] valueOrDefault = storeObject.PropertyBag.GetValueOrDefault<byte[]>(ICalendar.TimeZoneDefinitionStart);
					if (!O12TimeZoneFormatter.TryParseTimeZoneBlob(valueOrDefault, string.Empty, out recurringTimeZoneFromPropertyBag))
					{
						return;
					}
				}
				if (PropertyCommand.StorePropertyExists(storeObject, ICalendar.GlobalObjectId) && PropertyCommand.StorePropertyExists(storeObject, ICalendar.StartRecurTime))
				{
					byte[] globalObjectIdBytes = storeObject.TryGetProperty(ICalendar.GlobalObjectId) as byte[];
					int occurrenceScdTime = (int)storeObject.TryGetProperty(ICalendar.StartRecurTime);
					this.RenderToServiceObject(recurringTimeZoneFromPropertyBag, serviceObject, globalObjectIdBytes, occurrenceScdTime);
				}
			}

			public void ToServiceObjectForPropertyBag()
			{
				ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
				ServiceObject serviceObject = commandSettings.ServiceObject;
				IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
				ExTimeZone exTimeZone = null;
				byte[] bytes;
				if (PropertyCommand.TryGetValueFromPropertyBag<byte[]>(propertyBag, ICalendar.TimeZoneDefinitionRecurring, out bytes))
				{
					O12TimeZoneFormatter.TryParseTimeZoneBlob(bytes, string.Empty, out exTimeZone);
				}
				else if (PropertyCommand.TryGetValueFromPropertyBag<byte[]>(propertyBag, ICalendar.TimeZoneBlob, out bytes))
				{
					O11TimeZoneFormatter.TryParseTimeZoneBlob(bytes, string.Empty, out exTimeZone);
				}
				else if (PropertyCommand.TryGetValueFromPropertyBag<byte[]>(propertyBag, ICalendar.TimeZoneDefinitionStart, out bytes))
				{
					O12TimeZoneFormatter.TryParseTimeZoneBlob(bytes, string.Empty, out exTimeZone);
				}
				byte[] globalObjectIdBytes;
				int occurrenceScdTime;
				if (exTimeZone != null && PropertyCommand.TryGetValueFromPropertyBag<byte[]>(propertyBag, ICalendar.GlobalObjectId, out globalObjectIdBytes) && PropertyCommand.TryGetValueFromPropertyBag<int>(propertyBag, ICalendar.StartRecurTime, out occurrenceScdTime))
				{
					this.RenderToServiceObject(exTimeZone, serviceObject, globalObjectIdBytes, occurrenceScdTime);
				}
			}

			private static TimeSpan ConvertSCDTimeToTimeSpan(int scdTime)
			{
				return new TimeSpan(scdTime >> 12 & 31, scdTime >> 6 & 63, scdTime & 63);
			}

			private void RenderToServiceObject(ExTimeZone timeZone, ServiceObject serviceObject, byte[] globalObjectIdBytes, int occurrenceScdTime)
			{
				string value = null;
				try
				{
					GlobalObjectId globalObjectId = new GlobalObjectId(globalObjectIdBytes);
					ExDateTime exDateTime = globalObjectId.Date;
					if (exDateTime != ExDateTime.MinValue)
					{
						exDateTime += ICalendar.RecurrenceIdProperty.ConvertSCDTimeToTimeSpan(occurrenceScdTime);
						if (timeZone != null)
						{
							exDateTime = new ExDateTime(timeZone, exDateTime.Year, exDateTime.Month, exDateTime.Day, exDateTime.Hour, exDateTime.Minute, exDateTime.Second, exDateTime.Millisecond);
						}
						value = ExDateTimeConverter.ToUtcXsdDateTime(exDateTime);
					}
				}
				catch (Exception innerException)
				{
					throw new CalendarExceptionInvalidPropertyValue(this.commandContext.PropertyInformation.PropertyPath, innerException);
				}
				serviceObject[this.commandContext.PropertyInformation] = value;
			}

			public static readonly PropertyDefinition[] PropertiesToLoad = new PropertyDefinition[]
			{
				ICalendar.GlobalObjectId,
				ICalendar.StartRecurTime,
				ICalendar.TimeZoneBlob,
				ICalendar.TimeZoneDefinitionRecurring,
				ICalendar.TimeZoneDefinitionStart
			};
		}

		internal sealed class DateTimeStampProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
		{
			private DateTimeStampProperty(CommandContext commandContext) : base(commandContext)
			{
			}

			public static ICalendar.DateTimeStampProperty CreateCommand(CommandContext commandContext)
			{
				return new ICalendar.DateTimeStampProperty(commandContext);
			}

			public void ToXml()
			{
				throw new InvalidOperationException("DateTimeStampProperty.ToXml should not be called.");
			}

			public void ToXmlForPropertyBag()
			{
				throw new InvalidOperationException("DateTimeStampProperty.ToXmlForPropertyBag should not be called.");
			}

			public void ToServiceObject()
			{
				ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
				StoreObject storeObject = commandSettings.StoreObject;
				ExDateTime? valueAsNullable;
				if (ObjectClass.IsMeetingResponse(storeObject.ClassName))
				{
					valueAsNullable = storeObject.GetValueAsNullable<ExDateTime>(ICalendar.AttendeeCriticalChangeTime);
				}
				else
				{
					valueAsNullable = storeObject.GetValueAsNullable<ExDateTime>(ICalendar.OwnerCriticalChangeTime);
				}
				if (valueAsNullable == null)
				{
					valueAsNullable = storeObject.GetValueAsNullable<ExDateTime>(ICalendar.LastModifiedTime);
				}
				if (valueAsNullable == null)
				{
					valueAsNullable = new ExDateTime?(ExDateTime.UtcNow);
				}
				this.RenderToServiceObject(commandSettings.ServiceObject, valueAsNullable.Value);
			}

			public void ToServiceObjectForPropertyBag()
			{
				ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
				IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
				string itemClass;
				if (PropertyCommand.TryGetValueFromPropertyBag<string>(propertyBag, ICalendar.ItemClass, out itemClass))
				{
					ExDateTime? exDateTime = null;
					if (ObjectClass.IsMeetingResponse(itemClass))
					{
						PropertyCommand.TryGetValueFromPropertyBag<ExDateTime?>(propertyBag, ICalendar.AttendeeCriticalChangeTime, out exDateTime);
					}
					else
					{
						PropertyCommand.TryGetValueFromPropertyBag<ExDateTime?>(propertyBag, ICalendar.OwnerCriticalChangeTime, out exDateTime);
					}
					if (exDateTime == null)
					{
						PropertyCommand.TryGetValueFromPropertyBag<ExDateTime?>(propertyBag, ICalendar.LastModifiedTime, out exDateTime);
					}
					if (exDateTime == null)
					{
						exDateTime = new ExDateTime?(ExDateTime.UtcNow);
					}
					this.RenderToServiceObject(commandSettings.ServiceObject, exDateTime.Value);
				}
			}

			private void RenderToServiceObject(ServiceObject serviceObject, ExDateTime dateTime)
			{
				serviceObject[this.commandContext.PropertyInformation] = ExDateTimeConverter.ToUtcXsdDateTime(dateTime);
			}

			public static readonly PropertyDefinition[] PropertiesToLoad = new PropertyDefinition[]
			{
				ICalendar.OwnerCriticalChangeTime,
				ICalendar.AttendeeCriticalChangeTime,
				ICalendar.LastModifiedTime,
				ICalendar.ItemClass
			};
		}
	}
}
