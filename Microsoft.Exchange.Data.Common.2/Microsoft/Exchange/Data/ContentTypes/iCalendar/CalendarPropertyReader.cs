using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	public struct CalendarPropertyReader
	{
		internal CalendarPropertyReader(ContentLineReader reader)
		{
			this.reader = reader;
			this.lastSeparator = CalendarValueSeparators.None;
		}

		internal CalendarValueSeparators LastValueSeparator
		{
			get
			{
				return this.lastSeparator;
			}
		}

		public PropertyId PropertyId
		{
			get
			{
				this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
				return CalendarCommon.GetPropertyEnum(this.reader.PropertyName);
			}
		}

		public CalendarValueType ValueType
		{
			get
			{
				this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
				CalendarValueTypeContainer calendarValueTypeContainer = this.reader.ValueType as CalendarValueTypeContainer;
				return calendarValueTypeContainer.ValueType;
			}
		}

		public string Name
		{
			get
			{
				this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
				return this.reader.PropertyName;
			}
		}

		public CalendarParameterReader ParameterReader
		{
			get
			{
				this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
				return new CalendarParameterReader(this.reader);
			}
		}

		internal object ReadValue(CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValue(new CalendarValueSeparators?(expectedSeparators));
		}

		private object ReadValue(CalendarValueSeparators? expectedSeparators)
		{
			while (this.reader.ReadNextParameter())
			{
			}
			CalendarValueType valueType = this.ValueType;
			if (valueType <= CalendarValueType.Float)
			{
				if (valueType <= CalendarValueType.Date)
				{
					switch (valueType)
					{
					case CalendarValueType.Binary:
						return this.ReadValueAsBytes();
					case CalendarValueType.Unknown | CalendarValueType.Binary:
						break;
					case CalendarValueType.Boolean:
						return this.ReadValueAsBoolean(expectedSeparators);
					default:
						if (valueType == CalendarValueType.Date)
						{
							return this.ReadValueAsDateTime(expectedSeparators);
						}
						break;
					}
				}
				else
				{
					if (valueType == CalendarValueType.DateTime)
					{
						return this.ReadValueAsDateTime(expectedSeparators);
					}
					if (valueType == CalendarValueType.Duration)
					{
						return this.ReadValueAsTimeSpan(expectedSeparators);
					}
					if (valueType == CalendarValueType.Float)
					{
						return this.ReadValueAsFloat(expectedSeparators);
					}
				}
			}
			else if (valueType <= CalendarValueType.Period)
			{
				if (valueType == CalendarValueType.Integer)
				{
					return this.ReadValueAsInt32(expectedSeparators);
				}
				if (valueType == CalendarValueType.Period)
				{
					return this.ReadValueAsCalendarPeriod(expectedSeparators);
				}
			}
			else
			{
				if (valueType == CalendarValueType.Recurrence)
				{
					return this.ReadValueAsRecurrence(expectedSeparators);
				}
				if (valueType == CalendarValueType.Time)
				{
					return this.ReadValueAsCalendarTime(expectedSeparators);
				}
				if (valueType == CalendarValueType.UtcOffset)
				{
					return this.ReadValueAsTimeSpan(expectedSeparators);
				}
			}
			return this.ReadValueAsString(expectedSeparators);
		}

		public object ReadValue()
		{
			return this.ReadValue(null);
		}

		public byte[] ReadValueAsBytes()
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(256))
			{
				using (Stream valueReadStream = this.GetValueReadStream())
				{
					byte[] array = new byte[256];
					for (int i = valueReadStream.Read(array, 0, array.Length); i > 0; i = valueReadStream.Read(array, 0, array.Length))
					{
						memoryStream.Write(array, 0, i);
					}
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		private string ReadValueAsString(CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			ContentLineParser.Separators separators;
			string result;
			if (expectedSeparators != null)
			{
				result = this.reader.ReadPropertyValue(true, (ContentLineParser.Separators)expectedSeparators.Value, false, out separators);
			}
			else
			{
				result = this.reader.ReadPropertyValue(true, ContentLineParser.Separators.None, true, out separators);
			}
			this.lastSeparator = (CalendarValueSeparators)separators;
			return result;
		}

		internal string ReadValueAsString(CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsString(new CalendarValueSeparators?(expectedSeparators));
		}

		public string ReadValueAsString()
		{
			return this.ReadValueAsString(null);
		}

		private CalendarTime ReadValueAsCalendarTime(CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(CalendarValueType.Time);
			return new CalendarTime(s, this.reader.ComplianceTracker);
		}

		internal CalendarTime ReadValueAsCalendarTime(CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsCalendarTime(new CalendarValueSeparators?(expectedSeparators));
		}

		public CalendarTime ReadValueAsCalendarTime()
		{
			return this.ReadValueAsCalendarTime(null);
		}

		private DateTime ReadValueAsDateTime(CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			if (CalendarValueType.Date == this.ValueType)
			{
				return CalendarCommon.ParseDate(s, this.reader.ComplianceTracker);
			}
			this.CheckType(CalendarValueType.DateTime);
			return CalendarCommon.ParseDateTime(s, this.reader.ComplianceTracker);
		}

		internal DateTime ReadValueAsDateTime(CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsDateTime(new CalendarValueSeparators?(expectedSeparators));
		}

		public DateTime ReadValueAsDateTime()
		{
			return this.ReadValueAsDateTime(null);
		}

		private DateTime ReadValueAsDateTime(CalendarValueType valueType, CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			if (CalendarValueType.DateTime == valueType || CalendarValueType.Text == valueType)
			{
				this.CheckType(CalendarValueType.DateTime);
				return CalendarCommon.ParseDateTime(s, this.reader.ComplianceTracker);
			}
			if (CalendarValueType.Date == valueType)
			{
				this.CheckType(CalendarValueType.Date);
				return CalendarCommon.ParseDate(s, this.reader.ComplianceTracker);
			}
			throw new ArgumentOutOfRangeException("valueType");
		}

		internal DateTime ReadValueAsDateTime(CalendarValueType valueType, CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsDateTime(valueType, new CalendarValueSeparators?(expectedSeparators));
		}

		public DateTime ReadValueAsDateTime(CalendarValueType valueType)
		{
			return this.ReadValueAsDateTime(valueType, null);
		}

		private TimeSpan ReadValueAsTimeSpan(CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			if (CalendarValueType.UtcOffset == this.ValueType)
			{
				return CalendarCommon.ParseUtcOffset(s, this.reader.ComplianceTracker);
			}
			this.CheckType(CalendarValueType.Duration);
			return CalendarCommon.ParseDuration(s, this.reader.ComplianceTracker);
		}

		internal TimeSpan ReadValueAsTimeSpan(CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsTimeSpan(new CalendarValueSeparators?(expectedSeparators));
		}

		public TimeSpan ReadValueAsTimeSpan()
		{
			return this.ReadValueAsTimeSpan(null);
		}

		private TimeSpan ReadValueAsTimeSpan(CalendarValueType valueType, CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			if (CalendarValueType.Duration == valueType)
			{
				this.CheckType(CalendarValueType.Duration);
				return CalendarCommon.ParseDuration(s, this.reader.ComplianceTracker);
			}
			if (CalendarValueType.UtcOffset == valueType)
			{
				this.CheckType(CalendarValueType.UtcOffset);
				return CalendarCommon.ParseUtcOffset(s, this.reader.ComplianceTracker);
			}
			throw new ArgumentOutOfRangeException("valueType");
		}

		internal TimeSpan ReadValueAsTimeSpan(CalendarValueType valueType, CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsTimeSpan(valueType, new CalendarValueSeparators?(expectedSeparators));
		}

		public TimeSpan ReadValueAsTimeSpan(CalendarValueType valueType)
		{
			return this.ReadValueAsTimeSpan(valueType, null);
		}

		private bool ReadValueAsBoolean(CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string value = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(CalendarValueType.Boolean);
			bool result;
			if (!bool.TryParse(value, out result))
			{
				this.reader.ComplianceTracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
			return result;
		}

		internal bool ReadValueAsBoolean(CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsBoolean(new CalendarValueSeparators?(expectedSeparators));
		}

		public bool ReadValueAsBoolean()
		{
			return this.ReadValueAsBoolean(null);
		}

		private float ReadValueAsFloat(CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(CalendarValueType.Float);
			float result;
			if (!float.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
			{
				this.reader.ComplianceTracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
			return result;
		}

		internal float ReadValueAsFloat(CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsFloat(new CalendarValueSeparators?(expectedSeparators));
		}

		public float ReadValueAsFloat()
		{
			return this.ReadValueAsFloat(null);
		}

		private double ReadValueAsDouble(CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(CalendarValueType.Float);
			double result;
			if (!double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
			{
				this.reader.ComplianceTracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
			return result;
		}

		internal double ReadValueAsDouble(CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsDouble(new CalendarValueSeparators?(expectedSeparators));
		}

		public double ReadValueAsDouble()
		{
			return this.ReadValueAsDouble(null);
		}

		private int ReadValueAsInt32(CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(CalendarValueType.Integer);
			int result;
			if (!int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
			{
				this.reader.ComplianceTracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
			return result;
		}

		internal int ReadValueAsInt32(CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsInt32(new CalendarValueSeparators?(expectedSeparators));
		}

		public int ReadValueAsInt32()
		{
			return this.ReadValueAsInt32(null);
		}

		private CalendarPeriod ReadValueAsCalendarPeriod(CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(CalendarValueType.Period);
			return new CalendarPeriod(s, this.reader.ComplianceTracker);
		}

		internal CalendarPeriod ReadValueAsCalendarPeriod(CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsCalendarPeriod(new CalendarValueSeparators?(expectedSeparators));
		}

		public CalendarPeriod ReadValueAsCalendarPeriod()
		{
			return this.ReadValueAsCalendarPeriod(null);
		}

		private Recurrence ReadValueAsRecurrence(CalendarValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(CalendarValueType.Recurrence);
			return new Recurrence(s, this.reader.ComplianceTracker);
		}

		internal Recurrence ReadValueAsRecurrence(CalendarValueSeparators expectedSeparators)
		{
			return this.ReadValueAsRecurrence(new CalendarValueSeparators?(expectedSeparators));
		}

		public Recurrence ReadValueAsRecurrence()
		{
			return this.ReadValueAsRecurrence(null);
		}

		public bool ReadNextValue()
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property | ContentLineNodeType.DocumentEnd);
			return this.reader.ReadNextPropertyValue();
		}

		public bool ReadNextProperty()
		{
			this.reader.AssertValidState(ContentLineNodeType.ComponentStart | ContentLineNodeType.ComponentEnd | ContentLineNodeType.Parameter | ContentLineNodeType.Property | ContentLineNodeType.BeforeComponentStart | ContentLineNodeType.BeforeComponentEnd | ContentLineNodeType.DocumentEnd);
			return this.reader.ReadNextProperty();
		}

		public Stream GetValueReadStream()
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			this.lastSeparator = CalendarValueSeparators.None;
			return this.reader.GetValueReadStream();
		}

		private void CheckType(CalendarValueType type)
		{
			if (this.ValueType != type && this.ValueType != CalendarValueType.Text)
			{
				this.reader.ComplianceTracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
		}

		private ContentLineReader reader;

		private CalendarValueSeparators lastSeparator;
	}
}
