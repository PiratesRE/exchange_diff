using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarComponentBase
	{
		protected CalendarComponentBase(ICalContext context) : this(null, context)
		{
		}

		protected CalendarComponentBase(CalendarComponentBase root) : this(root, root.context)
		{
		}

		private CalendarComponentBase(CalendarComponentBase root, ICalContext context)
		{
			this.root = (root ?? this);
			this.context = context;
		}

		protected virtual void ProcessProperty(CalendarPropertyBase calendarProperty)
		{
		}

		protected virtual bool ValidateProperty(CalendarPropertyBase calendarProperty)
		{
			return true;
		}

		protected virtual bool ProcessSubComponent(CalendarComponentBase calendarComponent)
		{
			return true;
		}

		protected virtual bool ValidateProperties()
		{
			return true;
		}

		protected virtual bool ValidateStructure()
		{
			return true;
		}

		protected ICalOutboundContext OutboundContext
		{
			get
			{
				return (ICalOutboundContext)this.context;
			}
		}

		protected ICalInboundContext InboundContext
		{
			get
			{
				return (ICalInboundContext)this.context;
			}
		}

		protected ICalContext Context
		{
			get
			{
				return this.context;
			}
		}

		internal bool Parse(CalendarReader calReader)
		{
			this.componentId = calReader.ComponentId;
			this.componentName = calReader.ComponentName;
			this.icalProperties = new List<CalendarPropertyBase>();
			this.subComponents = new List<CalendarComponentBase>();
			bool result;
			try
			{
				result = (this.ParseProperties(calReader) && this.ParseSubComponents(calReader));
			}
			catch (ArgumentException)
			{
				this.Context.AddError(ServerStrings.InvalidICalElement(this.componentName));
				result = false;
			}
			return result;
		}

		internal bool Validate()
		{
			foreach (CalendarPropertyBase calendarProperty in this.icalProperties)
			{
				if (!this.ValidateProperty(calendarProperty))
				{
					return false;
				}
			}
			return this.ValidateProperties();
		}

		internal ComponentId ComponentId
		{
			get
			{
				return this.componentId;
			}
		}

		internal string ComponentName
		{
			get
			{
				return this.componentName;
			}
		}

		internal List<CalendarPropertyBase> ICalProperties
		{
			get
			{
				return this.icalProperties;
			}
		}

		internal bool ValidateTimeZoneInfo(bool isRecursive)
		{
			bool flag = this.InboundContext.DeclaredTimeZones.Count == 0;
			foreach (CalendarDateTime calendarDateTime in this.icalProperties.OfType<CalendarDateTime>())
			{
				string timeZoneId = calendarDateTime.TimeZoneId;
				if (!string.IsNullOrEmpty(timeZoneId))
				{
					if (flag)
					{
						this.Context.AddError(ServerStrings.TimeZoneReferenceWithNullTimeZone(timeZoneId));
						return false;
					}
					if (!this.InboundContext.DeclaredTimeZones.ContainsKey(timeZoneId))
					{
						this.Context.AddError(ServerStrings.WrongTimeZoneReference(timeZoneId));
						return false;
					}
				}
				if (!this.NormalizeCalendarDateTime(calendarDateTime))
				{
					return false;
				}
			}
			if (isRecursive)
			{
				foreach (CalendarComponentBase calendarComponentBase in this.subComponents)
				{
					if (!calendarComponentBase.ValidateTimeZoneInfo(isRecursive))
					{
						return false;
					}
				}
			}
			return true;
		}

		protected bool NormalizeCalendarDateTime(CalendarDateTime property)
		{
			if (property == null)
			{
				return true;
			}
			bool flag = true;
			ExTimeZone timeZone = ExTimeZone.UnspecifiedTimeZone;
			if (!string.IsNullOrEmpty(property.TimeZoneId))
			{
				timeZone = this.InboundContext.DeclaredTimeZones[property.TimeZoneId];
			}
			List<object> list = property.Value as List<object>;
			object value2;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					object value;
					if (!CalendarComponentBase.NormalizeSingleDateTime(timeZone, list[i], out value))
					{
						flag = false;
						break;
					}
					list[i] = value;
				}
			}
			else if (!CalendarComponentBase.NormalizeSingleDateTime(timeZone, property.Value, out value2))
			{
				flag = false;
			}
			else
			{
				property.Value = value2;
			}
			if (!flag)
			{
				this.Context.AddError(ServerStrings.InvalidICalElement(property.CalendarPropertyId.ToString()));
			}
			return flag;
		}

		private static bool NormalizeSingleDateTime(ExTimeZone timeZone, object rawValue, out object normalizedValue)
		{
			bool result = true;
			normalizedValue = rawValue;
			if (rawValue is DateTime)
			{
				try
				{
					normalizedValue = new ExDateTime(timeZone, (DateTime)rawValue);
					return result;
				}
				catch (ArgumentOutOfRangeException)
				{
					ExTraceGlobals.ICalTracer.TraceError(0L, "CalendarComponentBase::NormalizeSingleDateTime. Invalid DateTime value found. Value:'{0}'", new object[]
					{
						rawValue
					});
					return false;
				}
			}
			ExTraceGlobals.ICalTracer.TraceDebug(0L, "CalendarComponentBase::NormalizeSingleDateTime. Non DateTime value found for CalendarDateTime. Value:'{0}'", new object[]
			{
				rawValue
			});
			result = false;
			return result;
		}

		private bool ParseProperties(CalendarReader calReader)
		{
			CalendarPropertyReader propertyReader = calReader.PropertyReader;
			while (propertyReader.ReadNextProperty())
			{
				if (!this.ParseProperty(propertyReader) && !string.IsNullOrEmpty(propertyReader.Name))
				{
					this.Context.AddError(ServerStrings.InvalidICalElement(propertyReader.Name));
					return false;
				}
			}
			return true;
		}

		private bool ParseProperty(CalendarPropertyReader propertyReader)
		{
			bool result = false;
			CalendarPropertyBase calendarPropertyBase = this.NewProperty(propertyReader);
			if (calendarPropertyBase.Parse(propertyReader))
			{
				this.ProcessProperty(calendarPropertyBase);
				this.icalProperties.Add(calendarPropertyBase);
				result = true;
			}
			return result;
		}

		private bool ParseSubComponents(CalendarReader calReader)
		{
			if (calReader.ReadFirstChildComponent())
			{
				while (this.ParseSubComponent(calReader))
				{
					if (!calReader.ReadNextSiblingComponent())
					{
						goto IL_37;
					}
				}
				ExTraceGlobals.ICalTracer.TraceError<string>((long)this.GetHashCode(), "CalendarComponentBase::ParseSubComponents. Failed to parse subcomponent: {0}", calReader.ComponentName);
				return false;
			}
			IL_37:
			return this.ValidateStructure();
		}

		private bool ParseSubComponent(CalendarReader calReader)
		{
			CalendarComponentBase calendarComponentBase = this.NewComponent(calReader);
			bool result;
			if (calendarComponentBase.Parse(calReader))
			{
				if (this.ProcessSubComponent(calendarComponentBase))
				{
					this.subComponents.Add(calendarComponentBase);
					result = true;
				}
				else
				{
					ExTraceGlobals.ICalTracer.TraceError<string>((long)this.GetHashCode(), "CalendarComponentBase::ParseSubComponent. Failed to process component: {0}", calendarComponentBase.componentName);
					result = false;
				}
			}
			else
			{
				ExTraceGlobals.ICalTracer.TraceError<string>((long)this.GetHashCode(), "CalendarComponentBase::ParseSubComponent. Failed to parse component: {0}", calendarComponentBase.componentName);
				result = false;
			}
			return result;
		}

		private CalendarPropertyBase NewProperty(CalendarPropertyReader pr)
		{
			CalendarValueType valueType = pr.ValueType;
			if (valueType <= CalendarValueType.Date)
			{
				if (valueType == CalendarValueType.CalAddress)
				{
					return new CalendarAttendee();
				}
				if (valueType != CalendarValueType.Date)
				{
					goto IL_83;
				}
			}
			else if (valueType != CalendarValueType.DateTime)
			{
				if (valueType != CalendarValueType.Text)
				{
					goto IL_83;
				}
				if (string.Compare(pr.Name, "X-MS-OLK-ORIGINALSTART", StringComparison.CurrentCultureIgnoreCase) == 0 || string.Compare(pr.Name, "X-MS-OLK-ORIGINALEND", StringComparison.CurrentCultureIgnoreCase) == 0 || string.Compare(pr.Name, "X-MICROSOFT-EXDATE", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					return new CalendarDateTime();
				}
				return new CalendarPropertyBase();
			}
			return new CalendarDateTime();
			IL_83:
			return new CalendarPropertyBase();
		}

		private CalendarComponentBase NewComponent(CalendarReader calReader)
		{
			ComponentId componentId = calReader.ComponentId;
			if (componentId <= ComponentId.VTimeZone)
			{
				if (componentId == ComponentId.VEvent)
				{
					return new VEvent(this.root);
				}
				if (componentId == ComponentId.VTodo)
				{
					return new VTodo(this.root);
				}
				if (componentId == ComponentId.VTimeZone)
				{
					return new VTimeZone(this.root);
				}
			}
			else
			{
				if (componentId == ComponentId.VAlarm)
				{
					return new VAlarm(this.root);
				}
				if (componentId == ComponentId.Standard)
				{
					return new TimeZoneRule(this.root);
				}
				if (componentId == ComponentId.Daylight)
				{
					return new TimeZoneRule(this.root);
				}
			}
			return new CalendarComponentBase(this.root);
		}

		private ComponentId componentId;

		private string componentName;

		private CalendarComponentBase root;

		private List<CalendarPropertyBase> icalProperties;

		private List<CalendarComponentBase> subComponents;

		private ICalContext context;
	}
}
