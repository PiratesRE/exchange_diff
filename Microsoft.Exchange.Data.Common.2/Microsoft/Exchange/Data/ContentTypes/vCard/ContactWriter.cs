using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.vCard
{
	public class ContactWriter : IDisposable
	{
		public ContactWriter(Stream stream) : this(stream, Encoding.UTF8)
		{
		}

		public ContactWriter(Stream stream, Encoding encoding)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException(CalendarStrings.StreamMustAllowWrite, "stream");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			this.writer = new ContentLineWriter(stream, encoding);
			this.encoding = encoding;
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

		public void StartVCard()
		{
			this.EndProperty();
			this.AssertValidState(WriteState.Start);
			this.writer.WriteProperty("BEGIN", "VCARD");
			this.state = WriteState.Component;
		}

		public void EndVCard()
		{
			this.EndProperty();
			this.AssertValidState(WriteState.Component);
			this.writer.WriteProperty("END", "VCARD");
			this.state = WriteState.Start;
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
			PropertyId propertyEnum = ContactCommon.GetPropertyEnum(name);
			this.StartProperty(name, propertyEnum);
		}

		public void StartProperty(PropertyId propertyId)
		{
			string propertyString = ContactCommon.GetPropertyString(propertyId);
			if (propertyString == null)
			{
				throw new ArgumentException(CalendarStrings.InvalidPropertyId);
			}
			this.StartProperty(propertyString, propertyId);
		}

		public void StartParameter(string name)
		{
			if (this.validate)
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				if (name.Length == 0)
				{
					throw new ArgumentException();
				}
			}
			this.EndParameter();
			this.AssertValidState(WriteState.Property);
			this.parameter = ContactCommon.GetParameterEnum(name);
			this.writer.StartParameter(name);
			this.firstParameterValue = true;
			this.state = WriteState.Parameter;
		}

		public void StartParameter(ParameterId parameterId)
		{
			string parameterString = ContactCommon.GetParameterString(parameterId);
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
				this.valueType = ContactCommon.GetValueTypeEnum(value);
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

		public void WritePropertyValue(string value, ContactValueSeparators separator)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.PrepareStartPropertyValue((ContentLineParser.Separators)separator);
			if (this.valueType == ContactValueType.Text || this.valueType == ContactValueType.PhoneNumber || this.valueType == ContactValueType.VCard)
			{
				value = ContactWriter.GetEscapedText(value);
			}
			this.writer.WriteToStream(value);
		}

		public void WritePropertyValue(string value)
		{
			this.WritePropertyValue(value, ContactValueSeparators.Comma);
		}

		public void WritePropertyValue(object value, ContactValueSeparators separator)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			switch (this.valueType)
			{
			case ContactValueType.Unknown:
			case ContactValueType.Text:
			case ContactValueType.Uri:
			case ContactValueType.VCard:
			case ContactValueType.PhoneNumber:
			{
				string text = value as string;
				if (text == null)
				{
					throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
				}
				this.WritePropertyValue(text, separator);
				return;
			}
			case ContactValueType.Binary:
			{
				byte[] array = value as byte[];
				if (array == null)
				{
					throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
				}
				this.WritePropertyValue(array);
				return;
			}
			case ContactValueType.Boolean:
				if (!(value is bool))
				{
					throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
				}
				this.WritePropertyValue((bool)value, separator);
				return;
			case ContactValueType.Date:
				if (!(value is DateTime))
				{
					throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
				}
				this.WritePropertyValue((DateTime)value, ContactValueType.Date, separator);
				return;
			case ContactValueType.DateTime:
				if (!(value is DateTime))
				{
					throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
				}
				this.WritePropertyValue((DateTime)value, separator);
				return;
			case ContactValueType.Float:
				if (!(value is double))
				{
					throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
				}
				this.WritePropertyValue((double)value, separator);
				return;
			case ContactValueType.Integer:
				if (!(value is int))
				{
					throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
				}
				this.WritePropertyValue((int)value, separator);
				return;
			case ContactValueType.Time:
				if (!(value is DateTime))
				{
					throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
				}
				this.WritePropertyValue((DateTime)value, ContactValueType.Time, separator);
				return;
			case ContactValueType.UtcOffset:
				if (!(value is TimeSpan))
				{
					throw new ArgumentException(CalendarStrings.InvalidValueTypeForProperty);
				}
				this.WritePropertyValue((TimeSpan)value, separator);
				return;
			default:
				throw new InvalidDataException(CalendarStrings.InvalidValueTypeForProperty);
			}
		}

		public void WritePropertyValue(object value)
		{
			this.WritePropertyValue(value, ContactValueSeparators.Comma);
		}

		public void WritePropertyValue(int value, ContactValueSeparators separator)
		{
			this.WritePropertyValue(value.ToString(NumberFormatInfo.InvariantInfo), separator);
		}

		public void WritePropertyValue(int value)
		{
			this.WritePropertyValue(value.ToString(NumberFormatInfo.InvariantInfo));
		}

		public void WritePropertyValue(double value, ContactValueSeparators separator)
		{
			this.WritePropertyValue(value.ToString(NumberFormatInfo.InvariantInfo), separator);
		}

		public void WritePropertyValue(double value)
		{
			this.WritePropertyValue(value.ToString(NumberFormatInfo.InvariantInfo));
		}

		public void WritePropertyValue(bool value, ContactValueSeparators separator)
		{
			this.WritePropertyValue(value ? "TRUE" : "FALSE", separator);
		}

		public void WritePropertyValue(bool value)
		{
			this.WritePropertyValue(value ? "TRUE" : "FALSE");
		}

		public void WritePropertyValue(DateTime value, ContactValueSeparators separator)
		{
			this.WritePropertyValue(value, this.valueType, separator);
		}

		public void WritePropertyValue(DateTime value)
		{
			this.WritePropertyValue(value, this.valueType, ContactValueSeparators.Comma);
		}

		public void WritePropertyValue(byte[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.PrepareStartPropertyValue(ContentLineParser.Separators.None);
			this.writer.WriteToStream(value);
			this.EndProperty();
		}

		public void WritePropertyValue(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException();
			}
			this.PrepareStartPropertyValue(ContentLineParser.Separators.None);
			if (this.valueType == ContactValueType.Binary)
			{
				byte[] array = new byte[4096];
				for (;;)
				{
					int num = stream.Read(array, 0, array.Length);
					if (num == 0)
					{
						break;
					}
					this.writer.WriteToStream(array, 0, num);
				}
			}
			else
			{
				StreamReader streamReader = new StreamReader(stream, this.encoding);
				char[] array2 = new char[256];
				char[] array3 = new char[array2.Length * 2];
				bool flag = false;
				for (;;)
				{
					int num2 = streamReader.ReadBlock(array2, 0, array2.Length);
					if (num2 == 0)
					{
						break;
					}
					int size = 0;
					int i = 0;
					while (i < num2)
					{
						char c = array2[i];
						if (c <= '\r')
						{
							if (c != '\n')
							{
								if (c != '\r')
								{
									goto IL_12A;
								}
								array3[size++] = '\\';
								array3[size++] = 'n';
								flag = true;
							}
							else
							{
								if (!flag)
								{
									array3[size++] = '\\';
									array3[size++] = 'n';
								}
								flag = false;
							}
						}
						else
						{
							if (c != ',' && c != ';' && c != '\\')
							{
								goto IL_12A;
							}
							array3[size++] = '\\';
							array3[size++] = array2[i];
							flag = false;
						}
						IL_13B:
						i++;
						continue;
						IL_12A:
						array3[size++] = array2[i];
						flag = false;
						goto IL_13B;
					}
					this.writer.WriteChars(array3, 0, size);
				}
			}
			this.EndProperty();
		}

		public void WriteContact(ContactReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.ComplianceMode == ContactComplianceMode.Loose)
			{
				this.SetLooseMode();
			}
			while (reader.ReadNext())
			{
				this.StartVCard();
				ContactPropertyReader propertyReader = reader.PropertyReader;
				while (propertyReader.ReadNextProperty())
				{
					this.WriteProperty(propertyReader);
				}
				this.EndVCard();
			}
		}

		public void WriteProperty(ContactPropertyReader reader)
		{
			this.StartProperty(reader.Name);
			ContactParameterReader parameterReader = reader.ParameterReader;
			while (parameterReader.ReadNextParameter())
			{
				this.WriteParameter(parameterReader);
			}
			ContactValueSeparators separator = ContactValueSeparators.None;
			ContactValueSeparators expectedSeparators = ContactValueSeparators.Comma | ContactValueSeparators.Semicolon;
			ContactValueType contactValueType = reader.ValueType;
			switch (contactValueType)
			{
			case ContactValueType.Binary:
				expectedSeparators = ContactValueSeparators.None;
				break;
			case ContactValueType.Boolean:
				break;
			case ContactValueType.Date:
			case ContactValueType.DateTime:
				goto IL_55;
			default:
				if (contactValueType == ContactValueType.Time)
				{
					goto IL_55;
				}
				break;
			}
			IL_70:
			while (reader.ReadNextValue())
			{
				this.WritePropertyValue(reader.ReadValue(expectedSeparators), separator);
				separator = reader.LastValueSeparator;
			}
			this.EndProperty();
			return;
			IL_55:
			expectedSeparators = ContactValueSeparators.Semicolon;
			goto IL_70;
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

		public void WriteParameter(ContactParameterReader reader)
		{
			this.StartParameter(reader.Name);
			if (reader.Name != null)
			{
				while (reader.ReadNextValue())
				{
					this.WriteParameterValue(reader.ReadValue());
				}
			}
			else
			{
				this.WriteParameterValue(reader.ReadValue());
			}
			this.EndParameter();
		}

		public void WriteParameter(string name, string value)
		{
			this.StartParameter(name);
			this.WriteParameterValue(value);
			this.EndParameter();
		}

		public void WriteValueTypeParameter(ContactValueType type)
		{
			this.StartParameter(ParameterId.ValueType);
			this.WriteParameterValue(ContactCommon.GetValueTypeString(type));
			this.EndParameter();
		}

		public void WriteParameter(ParameterId parameterId, string value)
		{
			this.StartParameter(parameterId);
			this.WriteParameterValue(value);
			this.EndParameter();
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
			int num = data.IndexOfAny(ContactWriter.PropertyValueSpecials);
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
				int num2 = data.IndexOfAny(ContactWriter.PropertyValueSpecials, num);
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

		private void StartProperty(string name, PropertyId p)
		{
			this.EndProperty();
			this.AssertValidState(WriteState.Component);
			this.propertyName = name.ToUpper();
			this.valueType = ContactCommon.GetDefaultValueType(p);
			this.writer.StartProperty(this.propertyName);
			this.firstPropertyValue = true;
			this.state = WriteState.Property;
		}

		private void WritePropertyValue(DateTime value, ContactValueType valueType, ContactValueSeparators separator)
		{
			string value2;
			if (ContactValueType.DateTime == valueType || ContactValueType.Text == valueType)
			{
				value2 = ContactCommon.FormatDateTime(value);
			}
			else if (ContactValueType.Date == valueType)
			{
				value2 = ContactCommon.FormatDate(value);
			}
			else
			{
				if (ContactValueType.Time != valueType)
				{
					throw new ArgumentOutOfRangeException("valueType");
				}
				value2 = ContactCommon.FormatTime(value);
			}
			this.WritePropertyValue(value2, separator);
		}

		private void WritePropertyValue(TimeSpan value, ContactValueSeparators separator)
		{
			if (ContactValueType.UtcOffset != this.valueType)
			{
				throw new ArgumentOutOfRangeException("valueType");
			}
			if (value.Days > 0 && this.validate)
			{
				throw new ArgumentException(CalendarStrings.UtcOffsetTimespanCannotContainDays, "value");
			}
			string value2 = ContactCommon.FormatUtcOffset(value);
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
				this.state = WriteState.Component;
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
			int num = value.IndexOfAny(ContactWriter.ParameterValueSpecials);
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
				throw new ObjectDisposedException("ContactWriter");
			}
			if ((state & this.state) == (WriteState)0)
			{
				throw new InvalidOperationException(CalendarStrings.InvalidStateForOperation);
			}
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

		private ParameterId parameter;

		private ContactValueType valueType;

		private string propertyName;

		private WriteState state = WriteState.Start;

		private ContentLineWriter writer;

		private bool firstPropertyValue;

		private bool firstParameterValue;

		private bool validate = true;

		private Encoding encoding;
	}
}
