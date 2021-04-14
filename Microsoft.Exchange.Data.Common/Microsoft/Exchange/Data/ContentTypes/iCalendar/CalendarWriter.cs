using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Internal;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	public class CalendarWriter : IDisposable
	{
		public CalendarWriter(Stream stream) : this(stream, "utf-8")
		{
		}

		public CalendarWriter(Stream stream, string encodingName)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException(CalendarStrings.StreamMustAllowWrite, "stream");
			}
			if (encodingName == null)
			{
				throw new ArgumentNullException("encodingName");
			}
			this.writer = new ContentLineWriter(stream, Charset.GetEncoding(encodingName));
		}

		public void Flush()
		{
			this.AssertValidState(WriteState.Component | WriteState.Property | WriteState.Parameter);
			this.writer.Flush();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Close()
		{
			this.Dispose();
		}

		public void StartComponent(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (this.validate && name.Length == 0)
			{
				throw new ArgumentException();
			}
			this.EndProperty();
			this.AssertValidState(WriteState.Start | WriteState.Component);
			this.Save();
			this.componentName = name.ToUpper();
			this.componentId = CalendarCommon.GetComponentEnum(name);
			this.writer.WriteProperty("BEGIN", this.componentName);
			this.state = WriteState.Component;
		}

		public void StartComponent(ComponentId componentId)
		{
			if (componentId == ComponentId.Unknown || componentId == ComponentId.None)
			{
				throw new ArgumentException(CalendarStrings.InvalidComponentId);
			}
			this.StartComponent(CalendarCommon.GetComponentString(componentId));
		}

		public void EndComponent()
		{
			this.EndProperty();
			this.AssertValidState(WriteState.Component);
			this.writer.WriteProperty("END", this.componentName);
			this.Load();
			this.state = WriteState.Component;
		}

		public void StartProperty(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (this.validate && name.Length == 0)
			{
				throw new ArgumentException();
			}
			this.EndProperty();
			this.AssertValidState(WriteState.Component);
			PropertyId propertyEnum = CalendarCommon.GetPropertyEnum(name);
			this.propertyName = name.ToUpper();
			this.property = propertyEnum;
			this.Save();
			this.valueType = CalendarCommon.GetDefaultValueType(propertyEnum);
			this.writer.StartProperty(this.propertyName);
			this.firstPropertyValue = true;
			this.state = WriteState.Property;
		}

		public void StartProperty(PropertyId propertyId)
		{
			string propertyString = CalendarCommon.GetPropertyString(propertyId);
			if (propertyString == null)
			{
				throw new ArgumentException(CalendarStrings.InvalidPropertyId);
			}
			this.StartProperty(propertyString);
		}

		public void StartParameter(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (this.validate && name.Length == 0)
			{
				throw new ArgumentException();
			}
			this.EndParameter();
			this.AssertValidState(WriteState.Property);
			this.parameter = CalendarCommon.GetParameterEnum(name);
			this.writer.StartParameter(name);
			this.firstParameterValue = true;
			this.state = WriteState.Parameter;
		}

		public void StartParameter(ParameterId parameterId)
		{
			string parameterString = CalendarCommon.GetParameterString(parameterId);
			if (parameterString == null)
			{
				throw new ArgumentException(CalendarStrings.InvalidParameterId);
			}
			this.StartParameter(parameterString);
		}

		public void WriteParameterValue(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.AssertValidState(WriteState.Parameter);
			if (this.firstParameterValue)
			{
				this.writer.WriteStartValue();
				this.firstParameterValue = false;
			}
			else
			{
				this.writer.WriteNextValue(ContentLineParser.Separators.Comma);
			}
			if (this.parameter == ParameterId.ValueType && value.Length > 0)
			{
				this.valueType = CalendarCommon.GetValueTypeEnum(value);
			}
			bool flag = this.IsQuotingRequired(value);
			if (flag)
			{
				this.writer.WriteToStream(34);
			}
			this.writer.WriteToStream(value);
			if (flag)
			{
				this.writer.WriteToStream(34);
			}
		}

		public void WritePropertyValue(string value)
		{
			this.WritePropertyValue(value, CalendarValueSeparators.Comma);
		}

		public void WritePropertyValue(object value)
		{
			this.WritePropertyValue(value, CalendarValueSeparators.Comma);
		}

		public void WritePropertyValue(CalendarPeriod value)
		{
			this.WritePropertyValue(value.ToString());
		}

		public void WritePropertyValue(Recurrence value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.WritePropertyValue(value.ToString());
		}

		public void WritePropertyValue(int value)
		{
			this.WritePropertyValue(value.ToString(NumberFormatInfo.InvariantInfo));
		}

		public void WritePropertyValue(float value)
		{
			this.WritePropertyValue(value.ToString(NumberFormatInfo.InvariantInfo));
		}

		public void WritePropertyValue(bool value)
		{
			this.WritePropertyValue(value ? "TRUE" : "FALSE");
		}

		public void WritePropertyValue(DateTime value)
		{
			this.WritePropertyValue(value, this.valueType);
		}

		public void WritePropertyValue(DateTime value, CalendarValueType valueType)
		{
			this.WritePropertyValue(value, valueType, CalendarValueSeparators.Comma);
		}

		public void WritePropertyValue(CalendarTime value)
		{
			this.WritePropertyValue(value.ToString());
		}

		public void WritePropertyValue(TimeSpan value)
		{
			this.WritePropertyValue(value, this.valueType, CalendarValueSeparators.Comma);
		}

		public void WriteComponent(CalendarReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.Depth > 100)
			{
				return;
			}
			this.StartComponent(reader.ComponentName);
			CalendarPropertyReader propertyReader = reader.PropertyReader;
			while (propertyReader.ReadNextProperty())
			{
				this.WriteProperty(propertyReader);
			}
			if (reader.ReadFirstChildComponent())
			{
				this.WriteComponent(reader);
				while (reader.ReadNextSiblingComponent())
				{
					this.WriteComponent(reader);
				}
			}
			this.EndComponent();
		}

		public void WriteProperty(CalendarPropertyReader reader)
		{
			CalendarParameterReader parameterReader = reader.ParameterReader;
			this.StartProperty(reader.Name);
			while (parameterReader.ReadNextParameter())
			{
				this.WriteParameter(parameterReader);
			}
			CalendarValueSeparators separator = CalendarValueSeparators.None;
			while (reader.ReadNextValue())
			{
				this.WritePropertyValue(reader.ReadValue(CalendarValueSeparators.Comma | CalendarValueSeparators.Semicolon), separator);
				separator = reader.LastValueSeparator;
			}
			this.EndProperty();
		}

		public void WriteProperty(string name, string value)
		{
			this.StartProperty(name);
			this.WritePropertyValue(value);
			this.EndProperty();
		}

		public void WriteProperty(PropertyId propertyId, string value)
		{
			this.StartProperty(propertyId);
			this.WritePropertyValue(value);
			this.EndProperty();
		}

		public void WriteParameter(CalendarParameterReader reader)
		{
			this.StartParameter(reader.Name);
			while (reader.ReadNextValue())
			{
				this.WriteParameterValue(reader.ReadValue());
			}
			if (this.firstParameterValue)
			{
				this.WriteParameterValue(string.Empty);
			}
			this.EndParameter();
		}

		public void WriteParameter(string name, string value)
		{
			this.StartParameter(name);
			this.WriteParameterValue(value);
			this.EndParameter();
		}

		public void WriteParameter(ParameterId parameterId, string value)
		{
			this.StartParameter(parameterId);
			this.WriteParameterValue(value);
			this.EndParameter();
		}

		internal void WritePropertyValue(string value, CalendarValueSeparators separator)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.PrepareStartPropertyValue((ContentLineParser.Separators)separator);
			if (CalendarValueType.Text == this.valueType)
			{
				value = CalendarWriter.GetEscapedText(value);
			}
			this.writer.WriteToStream(value);
		}

		internal void WritePropertyValue(object value, CalendarValueSeparators separator)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			CalendarValueType calendarValueType = this.valueType;
			if (calendarValueType <= CalendarValueType.Float)
			{
				if (calendarValueType <= CalendarValueType.Date)
				{
					switch (calendarValueType)
					{
					case CalendarValueType.Unknown:
						break;
					case CalendarValueType.Binary:
					{
						byte[] array = value as byte[];
						if (array == null)
						{
							throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
						}
						this.PrepareStartPropertyValue(ContentLineParser.Separators.None);
						this.writer.WriteToStream(array);
						this.EndProperty();
						return;
					}
					case CalendarValueType.Unknown | CalendarValueType.Binary:
						goto IL_276;
					case CalendarValueType.Boolean:
						if (!(value is bool))
						{
							throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
						}
						this.WritePropertyValue((bool)value, separator);
						return;
					default:
						if (calendarValueType != CalendarValueType.CalAddress)
						{
							if (calendarValueType != CalendarValueType.Date)
							{
								goto IL_276;
							}
							if (!(value is DateTime))
							{
								throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
							}
							this.WritePropertyValue((DateTime)value, CalendarValueType.Date, separator);
							return;
						}
						break;
					}
				}
				else if (calendarValueType != CalendarValueType.DateTime)
				{
					if (calendarValueType != CalendarValueType.Duration)
					{
						if (calendarValueType != CalendarValueType.Float)
						{
							goto IL_276;
						}
						if (!(value is float))
						{
							throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
						}
						this.WritePropertyValue((float)value, separator);
						return;
					}
					else
					{
						if (!(value is TimeSpan))
						{
							throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
						}
						this.WritePropertyValue((TimeSpan)value, CalendarValueType.Duration, separator);
						return;
					}
				}
				else
				{
					if (!(value is DateTime))
					{
						throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
					}
					this.WritePropertyValue((DateTime)value, separator);
					return;
				}
			}
			else if (calendarValueType <= CalendarValueType.Recurrence)
			{
				if (calendarValueType != CalendarValueType.Integer)
				{
					if (calendarValueType != CalendarValueType.Period)
					{
						if (calendarValueType != CalendarValueType.Recurrence)
						{
							goto IL_276;
						}
						Recurrence recurrence = value as Recurrence;
						if (recurrence == null)
						{
							throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
						}
						this.WritePropertyValue(recurrence);
						return;
					}
					else
					{
						if (!(value is CalendarPeriod))
						{
							throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
						}
						this.WritePropertyValue((CalendarPeriod)value, separator);
						return;
					}
				}
				else
				{
					if (!(value is int))
					{
						throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
					}
					this.WritePropertyValue((int)value, separator);
					return;
				}
			}
			else if (calendarValueType <= CalendarValueType.Time)
			{
				if (calendarValueType != CalendarValueType.Text)
				{
					if (calendarValueType != CalendarValueType.Time)
					{
						goto IL_276;
					}
					if (!(value is CalendarTime))
					{
						throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
					}
					this.WritePropertyValue((CalendarTime)value, separator);
					return;
				}
			}
			else if (calendarValueType != CalendarValueType.Uri)
			{
				if (calendarValueType != CalendarValueType.UtcOffset)
				{
					goto IL_276;
				}
				if (!(value is TimeSpan))
				{
					throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
				}
				this.WritePropertyValue((TimeSpan)value, CalendarValueType.UtcOffset, separator);
				return;
			}
			string text = value as string;
			if (text == null)
			{
				throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
			}
			this.WritePropertyValue(text, separator);
			return;
			IL_276:
			throw new InvalidDataException(CalendarStrings.InvalidValueTypeForProperty);
		}

		internal void WritePropertyValue(CalendarPeriod value, CalendarValueSeparators separator)
		{
			this.WritePropertyValue(value.ToString(), separator);
		}

		internal void WritePropertyValue(int value, CalendarValueSeparators separator)
		{
			this.WritePropertyValue(value.ToString(NumberFormatInfo.InvariantInfo), separator);
		}

		internal void WritePropertyValue(float value, CalendarValueSeparators separator)
		{
			this.WritePropertyValue(value.ToString(NumberFormatInfo.InvariantInfo), separator);
		}

		internal void WritePropertyValue(bool value, CalendarValueSeparators separator)
		{
			this.WritePropertyValue(value ? "TRUE" : "FALSE", separator);
		}

		internal void WritePropertyValue(DateTime value, CalendarValueSeparators separator)
		{
			this.WritePropertyValue(value, this.valueType, separator);
		}

		internal void WritePropertyValue(CalendarTime value, CalendarValueSeparators separator)
		{
			this.WritePropertyValue(value.ToString(), separator);
		}

		internal void WritePropertyValue(TimeSpan value, CalendarValueSeparators separator)
		{
			this.WritePropertyValue(value, this.valueType, separator);
		}

		internal void SetLooseMode()
		{
			this.validate = false;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.state != WriteState.Closed)
			{
				this.writer.Dispose();
			}
			this.state = WriteState.Closed;
		}

		private static string GetEscapedText(string data)
		{
			int num = data.IndexOfAny(CalendarWriter.PropertyValueSpecials);
			if (-1 == num)
			{
				return data;
			}
			int length = data.Length;
			StringBuilder stringBuilder = new StringBuilder(data, 0, num, length);
			for (;;)
			{
				stringBuilder.Append('\\');
				bool flag = '\r' == data[num];
				if (flag || '\n' == data[num])
				{
					stringBuilder.Append('n');
					num++;
					if (flag && num < data.Length && '\n' == data[num])
					{
						num++;
					}
				}
				else
				{
					stringBuilder.Append(data[num++]);
				}
				if (num == data.Length)
				{
					goto IL_C1;
				}
				int num2 = data.IndexOfAny(CalendarWriter.PropertyValueSpecials, num);
				if (-1 == num2)
				{
					break;
				}
				stringBuilder.Append(data, num, num2 - num);
				num = num2;
			}
			stringBuilder.Append(data, num, length - num);
			IL_C1:
			return stringBuilder.ToString();
		}

		private void WritePropertyValue(DateTime value, CalendarValueType valueType, CalendarValueSeparators separator)
		{
			string value2;
			if (CalendarValueType.DateTime == valueType || CalendarValueType.Text == valueType)
			{
				value2 = CalendarCommon.FormatDateTime(value);
			}
			else
			{
				if (CalendarValueType.Date != valueType)
				{
					throw new ArgumentOutOfRangeException("valueType");
				}
				value2 = CalendarCommon.FormatDate(value);
			}
			this.WritePropertyValue(value2, separator);
		}

		private void WritePropertyValue(TimeSpan value, CalendarValueType valueType, CalendarValueSeparators separator)
		{
			string value2;
			if (CalendarValueType.Duration == valueType)
			{
				value2 = CalendarCommon.FormatDuration(value);
			}
			else
			{
				if (CalendarValueType.UtcOffset != valueType)
				{
					throw new ArgumentOutOfRangeException("valueType");
				}
				if (value.Days > 0 && this.validate)
				{
					throw new ArgumentException(CalendarStrings.UtcOffsetTimespanCannotContainDays, "value");
				}
				value2 = CalendarCommon.FormatUtcOffset(value);
			}
			this.WritePropertyValue(value2, separator);
		}

		private void EndProperty()
		{
			this.EndParameter();
			if (this.state == WriteState.Property)
			{
				if (this.writer.State != ContentLineWriteState.PropertyValue)
				{
					this.writer.WriteStartValue();
				}
				this.writer.EndProperty();
				this.Load();
			}
		}

		private void EndParameter()
		{
			if (this.state == WriteState.Parameter)
			{
				this.writer.EndParameter();
				this.state = WriteState.Property;
			}
		}

		private bool IsQuotingRequired(string value)
		{
			int num = value.IndexOfAny(CalendarWriter.ParameterValueSpecials);
			if (-1 == num)
			{
				return false;
			}
			if (-1 != value.IndexOf('"', num) && this.validate)
			{
				throw new ArgumentException(CalendarStrings.ParameterValuesCannotContainDoubleQuote);
			}
			return true;
		}

		private void PrepareStartPropertyValue(ContentLineParser.Separators separator)
		{
			this.EndParameter();
			this.AssertValidState(WriteState.Property);
			if (this.firstPropertyValue)
			{
				this.writer.WriteStartValue();
				this.firstPropertyValue = false;
				return;
			}
			this.writer.WriteNextValue(separator);
		}

		private void AssertValidState(WriteState state)
		{
			if (this.state == WriteState.Closed)
			{
				throw new ObjectDisposedException("CalendarWriter");
			}
			if ((state & this.state) == (WriteState)0)
			{
				throw new InvalidOperationException(CalendarStrings.InvalidStateForOperation);
			}
		}

		private void Save()
		{
			CalendarWriter.WriterState item;
			item.ComponentId = this.componentId;
			item.Property = this.property;
			item.PropertyName = this.propertyName;
			item.ComponentName = this.componentName;
			item.ValueType = this.valueType;
			item.State = this.state;
			this.stateStack.Push(item);
		}

		private void Load()
		{
			CalendarWriter.WriterState writerState = this.stateStack.Pop();
			this.componentId = writerState.ComponentId;
			this.property = writerState.Property;
			this.propertyName = writerState.PropertyName;
			this.componentName = writerState.ComponentName;
			this.valueType = writerState.ValueType;
			this.state = writerState.State;
		}

		private const char CR = '\r';

		private const char LF = '\n';

		private const string ComponentStartTag = "BEGIN";

		private const string ComponentEndTag = "END";

		private static readonly char[] ParameterValueSpecials = new char[]
		{
			',',
			':',
			';',
			'"'
		};

		private static readonly char[] PropertyValueSpecials = new char[]
		{
			',',
			';',
			'\\',
			'\r',
			'\n'
		};

		private ComponentId componentId;

		private PropertyId property;

		private ParameterId parameter = ParameterId.Unknown;

		private CalendarValueType valueType = CalendarValueType.Unknown;

		private string componentName;

		private string propertyName;

		private WriteState state = WriteState.Start;

		private ContentLineWriter writer;

		private Stack<CalendarWriter.WriterState> stateStack = new Stack<CalendarWriter.WriterState>();

		private bool firstPropertyValue;

		private bool firstParameterValue;

		private bool validate = true;

		private struct WriterState
		{
			public ComponentId ComponentId;

			public PropertyId Property;

			public CalendarValueType ValueType;

			public string ComponentName;

			public string PropertyName;

			public WriteState State;
		}
	}
}
