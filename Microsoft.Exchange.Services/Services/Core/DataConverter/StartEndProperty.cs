using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class StartEndProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		private StartEndProperty(CommandContext commandContext, PropertyDefinition propertyDefinition) : base(commandContext)
		{
			this.propertyDefinition = propertyDefinition;
		}

		private bool IsWallClockProperty
		{
			get
			{
				return this.propertyDefinition == CalendarItemInstanceSchema.StartWallClock || this.propertyDefinition == CalendarItemInstanceSchema.EndWallClock;
			}
		}

		public static StartEndProperty CreateCommandForStart(CommandContext commandContext)
		{
			return new StartEndProperty(commandContext, CalendarItemInstanceSchema.StartTime);
		}

		public static StartEndProperty CreateCommandForEnd(CommandContext commandContext)
		{
			return new StartEndProperty(commandContext, CalendarItemInstanceSchema.EndTime);
		}

		public static StartEndProperty CreateCommandForProposedStart(CommandContext commandContext)
		{
			return new StartEndProperty(commandContext, MeetingResponseSchema.AppointmentCounterStartWhole);
		}

		public static StartEndProperty CreateCommandForProposedEnd(CommandContext commandContext)
		{
			return new StartEndProperty(commandContext, MeetingResponseSchema.AppointmentCounterEndWhole);
		}

		public static StartEndProperty CreateCommandForStartWallClock(CommandContext commandContext)
		{
			return new StartEndProperty(commandContext, CalendarItemInstanceSchema.StartWallClock);
		}

		public static StartEndProperty CreateCommandForEndWallClock(CommandContext commandContext)
		{
			return new StartEndProperty(commandContext, CalendarItemInstanceSchema.EndWallClock);
		}

		public static string ConvertDateTimeToString(ExDateTime? dateTime, bool preserveTimeZone)
		{
			if (dateTime == null)
			{
				return null;
			}
			return StartEndProperty.ConvertDateTimeToString(dateTime.Value, preserveTimeZone);
		}

		public static string ConvertDateTimeToString(ExDateTime dateTime, bool preserveTimeZone)
		{
			if (dateTime == ExDateTime.MinValue)
			{
				return null;
			}
			if (preserveTimeZone)
			{
				return ExDateTimeConverter.ToOffsetXsdDateTime(dateTime, dateTime.TimeZone);
			}
			return ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(dateTime);
		}

		public void Set()
		{
		}

		void ISetCommand.SetPhase3()
		{
			this.SetPhase3();
			this.ValidateStartEnd(base.GetCommandSettings<SetCommandSettings>().StoreObject);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			CalendarItemBase calendarItemBase = (CalendarItemBase)updateCommandSettings.StoreObject;
			string valueOrDefault = setPropertyUpdate.ServiceObject.GetValueOrDefault<string>(this.commandContext.PropertyInformation);
			this.SetProperty(calendarItemBase, valueOrDefault);
		}

		void IUpdateCommand.PostUpdate()
		{
			this.PostUpdate();
			this.ValidateStartEnd(base.GetCommandSettings<UpdateCommandSettings>().StoreObject);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("StartEndProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("StartEndProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			StoreObject storeObject = commandSettings.StoreObject;
			if (PropertyCommand.StorePropertyExists(storeObject, this.propertyDefinition))
			{
				ExDateTime dateTime = (ExDateTime)storeObject[this.propertyDefinition];
				serviceObject[propertyInformation] = StartEndProperty.ConvertDateTimeToString(dateTime, this.IsWallClockProperty);
			}
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ExDateTime dateTime;
			if (PropertyCommand.TryGetValueFromPropertyBag<ExDateTime>(propertyBag, this.propertyDefinition, out dateTime))
			{
				serviceObject[propertyInformation] = StartEndProperty.ConvertDateTimeToString(dateTime, this.IsWallClockProperty);
			}
		}

		private void ValidateStartEnd(StoreObject storeObject)
		{
			bool flag = PropertyCommand.StorePropertyExists(storeObject, CalendarItemInstanceSchema.StartTime);
			bool flag2 = PropertyCommand.StorePropertyExists(storeObject, CalendarItemInstanceSchema.EndTime);
			if (flag && flag2)
			{
				ExDateTime exDateTime = (ExDateTime)storeObject[CalendarItemInstanceSchema.StartTime];
				ExDateTime exDateTime2 = (ExDateTime)storeObject[CalendarItemInstanceSchema.EndTime];
				if (exDateTime.CompareTo(exDateTime2) > 0)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(CallContext.Current.ProtocolLog, "InvalidStartTime", string.Format("Start: {0} End: {1}", exDateTime, exDateTime2));
					throw new CalendarExceptionEndDateIsEarlierThanStartDate();
				}
				ExDateTime other = ExDateTime.MaxValue.AddYears(-5);
				if (exDateTime.CompareTo(other) < 0)
				{
					exDateTime = exDateTime.AddYears(5);
				}
				else
				{
					exDateTime2 = exDateTime2.AddYears(-5);
				}
				if (exDateTime.CompareTo(exDateTime2) < 0)
				{
					throw new CalendarExceptionDurationIsTooLong();
				}
			}
		}

		private void SetProperty(CalendarItemBase calendarItemBase, string value)
		{
			ExTimeZone timeZone;
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
			{
				timeZone = ((this.propertyDefinition == CalendarItemInstanceSchema.StartTime) ? calendarItemBase.StartTimeZone : calendarItemBase.EndTimeZone);
			}
			else
			{
				timeZone = calendarItemBase.Session.ExTimeZone;
			}
			ExDateTime exDateTime = ExDateTimeConverter.ParseTimeZoneRelated(value, timeZone);
			calendarItemBase[this.propertyDefinition] = exDateTime;
		}

		void ISetCommand.SetPhase2()
		{
			this.SetPhase2();
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			CalendarItemBase calendarItemBase = (CalendarItemBase)commandSettings.StoreObject;
			string value;
			if (commandSettings.ServiceObject != null)
			{
				value = (commandSettings.ServiceObject[this.commandContext.PropertyInformation] as string);
			}
			else
			{
				value = ServiceXml.GetXmlTextNodeValue(commandSettings.ServiceProperty);
			}
			this.SetProperty(calendarItemBase, value);
		}

		private const int MaxCalendarDurationInYears = 5;

		private PropertyDefinition propertyDefinition;
	}
}
