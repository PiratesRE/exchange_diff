using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal abstract class CalendarFolderDataSourceBase : ICalendarDataSource
	{
		public CalendarFolderDataSourceBase(DateRange[] dateRanges, PropertyDefinition[] properties)
		{
			if (dateRanges == null || dateRanges.Length == 0)
			{
				throw new ArgumentNullException("dateRanges");
			}
			if (properties == null || properties.Length == 0)
			{
				throw new ArgumentNullException("properties");
			}
			this.propertyIndexes = new Hashtable(properties.Length);
			for (int i = 0; i < properties.Length; i++)
			{
				this.propertyIndexes[properties[i]] = i;
			}
			this.dateRanges = dateRanges;
			this.properties = properties;
		}

		protected void Load(CalendarFolderDataSourceBase.GetPropertiesDelegate getProperties)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "CalendarFolderDataSourceBase.Load");
			ExTraceGlobals.CalendarTracer.TraceDebug((long)this.GetHashCode(), "Calling XSO's GetCalendarView to do the calendar query");
			Stopwatch watch = Utilities.StartWatch();
			this.allData = getProperties(DateRange.GetMinStartTimeInRangeArray(this.dateRanges), DateRange.GetMaxEndTimeInRangeArray(this.dateRanges));
			Utilities.StopWatch(watch, "GetCalendarView");
			if (this.allData.Length == 0)
			{
				return;
			}
			this.viewData = new ArrayList(2 * this.dateRanges.Length);
			for (int i = 0; i < this.allData.Length; i++)
			{
				object obj = this.allData[i][(int)this.propertyIndexes[CalendarItemInstanceSchema.StartTime]];
				if (obj != null && !(obj is PropertyError) && obj is ExDateTime)
				{
					ExDateTime exDateTime = (ExDateTime)obj;
					obj = this.allData[i][(int)this.propertyIndexes[CalendarItemInstanceSchema.EndTime]];
					if (obj != null && !(obj is PropertyError) && obj is ExDateTime)
					{
						ExDateTime exDateTime2 = (ExDateTime)obj;
						if (exDateTime > exDateTime2)
						{
							ExTraceGlobals.CalendarDataTracer.TraceDebug(0L, "Skipping appointment with an end time earlier than a start time");
						}
						else
						{
							for (int j = 0; j < this.dateRanges.Length; j++)
							{
								if (this.dateRanges[j].Intersects(exDateTime, exDateTime2))
								{
									this.viewData.Add(this.allData[i]);
									break;
								}
							}
						}
					}
				}
			}
		}

		public int Count
		{
			get
			{
				if (this.viewData != null)
				{
					return this.viewData.Count;
				}
				return 0;
			}
		}

		protected object GetPropertyValue(int itemIndex, PropertyDefinition propertyDefinition)
		{
			if (this.viewData == null)
			{
				throw new OwaInvalidOperationException("Can't call GetPropertyValue if the data source is empty");
			}
			object[] array = (object[])this.viewData[itemIndex];
			return array[(int)this.propertyIndexes[propertyDefinition]];
		}

		protected bool TryGetPropertyValue<T>(int itemIndex, PropertyDefinition propertyDefinition, out T propertyValue)
		{
			object propertyValue2 = this.GetPropertyValue(itemIndex, propertyDefinition);
			if (propertyValue2 != null && propertyValue2 is T)
			{
				propertyValue = (T)((object)propertyValue2);
				return true;
			}
			propertyValue = default(T);
			return false;
		}

		protected string GetStringPropertyValue(int itemIndex, PropertyDefinition propertyDefinition)
		{
			string text = this.GetPropertyValue(itemIndex, propertyDefinition) as string;
			if (text == null)
			{
				ExTraceGlobals.CalendarDataTracer.TraceDebug(0L, "Error retrieving calendar item property with id={0}, defaulting to empty string");
				return string.Empty;
			}
			return text;
		}

		protected ExDateTime GetDateTimePropertyValue(int itemIndex, PropertyDefinition propertyDefinition)
		{
			object propertyValue = this.GetPropertyValue(itemIndex, propertyDefinition);
			if (propertyValue == null || !(propertyValue is ExDateTime))
			{
				ExTraceGlobals.CalendarDataTracer.TraceDebug<PropertyDefinition>(0L, "Error retrieving calendar item property with id={0}, defaulting to dateTime.MinValue", propertyDefinition);
				return ExDateTime.MinValue;
			}
			return (ExDateTime)propertyValue;
		}

		protected bool GetBoolPropertyValue(int itemIndex, PropertyDefinition propertyDefinition)
		{
			object propertyValue = this.GetPropertyValue(itemIndex, propertyDefinition);
			if (propertyValue == null || !(propertyValue is bool))
			{
				ExTraceGlobals.CalendarDataTracer.TraceDebug<PropertyDefinition>(0L, "Error retrieving calendar item property with id={0}, defaulting to false", propertyDefinition);
				return false;
			}
			return (bool)propertyValue;
		}

		public abstract OwaStoreObjectId GetItemId(int index);

		public abstract string GetChangeKey(int index);

		public ExDateTime GetStartTime(int index)
		{
			return this.GetDateTimePropertyValue(index, CalendarItemInstanceSchema.StartTime);
		}

		public ExDateTime GetEndTime(int index)
		{
			return this.GetDateTimePropertyValue(index, CalendarItemInstanceSchema.EndTime);
		}

		public string GetSubject(int index)
		{
			return this.GetStringPropertyValue(index, ItemSchema.Subject);
		}

		public string GetLocation(int index)
		{
			return this.GetStringPropertyValue(index, CalendarItemBaseSchema.Location);
		}

		public bool IsMeeting(int index)
		{
			int valueToTest;
			return this.TryGetPropertyValue<int>(index, CalendarItemBaseSchema.AppointmentState, out valueToTest) && Utilities.IsFlagSet(valueToTest, 1);
		}

		public bool IsCancelled(int index)
		{
			int valueToTest;
			return this.TryGetPropertyValue<int>(index, CalendarItemBaseSchema.AppointmentState, out valueToTest) && Utilities.IsFlagSet(valueToTest, 4);
		}

		public virtual bool HasAttachment(int index)
		{
			return this.GetBoolPropertyValue(index, ItemSchema.HasAttachment);
		}

		public virtual bool IsPrivate(int index)
		{
			return this.IsPrivate(index, new bool?(false)).Value;
		}

		public bool? IsPrivate(int index, bool? defaultValue)
		{
			Sensitivity sensitivity;
			if (this.TryGetPropertyValue<Sensitivity>(index, ItemSchema.Sensitivity, out sensitivity))
			{
				return new bool?(sensitivity == Sensitivity.Private);
			}
			ExTraceGlobals.CalendarDataTracer.TraceDebug(0L, "Error retrieving calendar item sensitivity, defaulting to normal");
			return defaultValue;
		}

		public CalendarItemTypeWrapper GetWrappedItemType(int index)
		{
			CalendarItemType calendarItemType;
			if (!this.TryGetPropertyValue<CalendarItemType>(index, CalendarItemBaseSchema.CalendarItemType, out calendarItemType))
			{
				ExTraceGlobals.CalendarDataTracer.TraceDebug(0L, "Error retrieving calendar item type, defaulting to single");
				return CalendarItemTypeWrapper.Single;
			}
			switch (calendarItemType)
			{
			case CalendarItemType.Single:
				return CalendarItemTypeWrapper.Single;
			case CalendarItemType.Occurrence:
				return CalendarItemTypeWrapper.Occurrence;
			case CalendarItemType.Exception:
				return CalendarItemTypeWrapper.Exception;
			default:
				return CalendarItemTypeWrapper.Single;
			}
		}

		public virtual string GetOrganizerDisplayName(int index)
		{
			return this.GetStringPropertyValue(index, CalendarItemBaseSchema.OrganizerDisplayName);
		}

		public BusyTypeWrapper GetWrappedBusyType(int index)
		{
			int num;
			if (!this.TryGetPropertyValue<int>(index, CalendarItemBaseSchema.FreeBusyStatus, out num))
			{
				return BusyTypeWrapper.Free;
			}
			switch (num)
			{
			case 0:
				return BusyTypeWrapper.Free;
			case 1:
				return BusyTypeWrapper.Tentative;
			case 2:
				return BusyTypeWrapper.Busy;
			case 3:
				return BusyTypeWrapper.OOF;
			default:
				return BusyTypeWrapper.Unknown;
			}
		}

		public object GetBusyType(int index)
		{
			return this.GetPropertyValue(index, CalendarItemBaseSchema.FreeBusyStatus);
		}

		public bool IsOrganizer(int index)
		{
			return this.GetBoolPropertyValue(index, CalendarItemBaseSchema.IsOrganizer);
		}

		public string[] GetCategories(int index)
		{
			return this.GetPropertyValue(index, ItemSchema.Categories) as string[];
		}

		public abstract string GetCssClassName(int index);

		public string GetInviteesDisplayNames(int index)
		{
			string stringPropertyValue = this.GetStringPropertyValue(index, CalendarItemBaseSchema.DisplayAttendeesTo);
			string stringPropertyValue2 = this.GetStringPropertyValue(index, CalendarItemBaseSchema.DisplayAttendeesCc);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(stringPropertyValue);
			if (stringPropertyValue != string.Empty && stringPropertyValue2 != string.Empty)
			{
				stringBuilder.Append("; ");
			}
			stringBuilder.Append(stringPropertyValue2);
			return stringBuilder.ToString();
		}

		public abstract SharedType SharedType { get; }

		public abstract WorkingHours WorkingHours { get; }

		public abstract bool UserCanReadItem { get; }

		public abstract bool UserCanCreateItem { get; }

		public abstract string FolderClassName { get; }

		public const string DefaultClass = "noClrCal";

		protected DateRange[] dateRanges;

		protected PropertyDefinition[] properties;

		protected Hashtable propertyIndexes;

		protected ArrayList viewData;

		protected object[][] allData;

		protected delegate object[][] GetPropertiesDelegate(ExDateTime start, ExDateTime end);
	}
}
