using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Internal;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.ContentTypes.vCard
{
	public struct ContactPropertyReader
	{
		internal ContactPropertyReader(ContentLineReader reader)
		{
			this.reader = reader;
			this.lastSeparator = ContactValueSeparators.None;
		}

		public ContactValueSeparators LastValueSeparator
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
				return ContactCommon.GetPropertyEnum(this.reader.PropertyName);
			}
		}

		public ContactValueType ValueType
		{
			get
			{
				this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
				ContactValueTypeContainer contactValueTypeContainer = this.reader.ValueType as ContactValueTypeContainer;
				return contactValueTypeContainer.ValueType;
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

		public ContactParameterReader ParameterReader
		{
			get
			{
				this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
				return new ContactParameterReader(this.reader);
			}
		}

		public object ReadValue(ContactValueSeparators expectedSeparators)
		{
			return this.ReadValue(new ContactValueSeparators?(expectedSeparators));
		}

		private object ReadValue(ContactValueSeparators? expectedSeparators)
		{
			while (this.reader.ReadNextParameter())
			{
			}
			switch (this.ValueType)
			{
			case ContactValueType.Binary:
				return this.ReadValueAsBytes();
			case ContactValueType.Boolean:
				return this.ReadValueAsBoolean(expectedSeparators);
			case ContactValueType.Date:
			case ContactValueType.DateTime:
			case ContactValueType.Time:
				return this.ReadValueAsDateTime(this.ValueType, expectedSeparators);
			case ContactValueType.Float:
				return this.ReadValueAsDouble(expectedSeparators);
			case ContactValueType.Integer:
				return this.ReadValueAsInt32(expectedSeparators);
			case ContactValueType.UtcOffset:
				return this.ReadValueAsTimeSpan(expectedSeparators);
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

		private string ReadValueAsString(ContactValueSeparators? expectedSeparators)
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
			this.lastSeparator = (ContactValueSeparators)separators;
			return result;
		}

		public string ReadValueAsString(ContactValueSeparators expectedSeparators)
		{
			return this.ReadValueAsString(new ContactValueSeparators?(expectedSeparators));
		}

		public string ReadValueAsString()
		{
			return this.ReadValueAsString(null);
		}

		private DateTime ReadValueAsDateTime(ContactValueType type, ContactValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(type);
			if (type == ContactValueType.DateTime)
			{
				return ContactCommon.ParseDateTime(s, this.reader.ComplianceTracker);
			}
			if (type == ContactValueType.Time)
			{
				return ContactCommon.ParseTime(s, this.reader.ComplianceTracker);
			}
			return ContactCommon.ParseDate(s, this.reader.ComplianceTracker);
		}

		public DateTime ReadValueAsDateTime(ContactValueType type, ContactValueSeparators expectedSeparators)
		{
			return this.ReadValueAsDateTime(type, new ContactValueSeparators?(expectedSeparators));
		}

		public DateTime ReadValueAsDateTime(ContactValueType type)
		{
			return this.ReadValueAsDateTime(type, null);
		}

		private TimeSpan ReadValueAsTimeSpan(ContactValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(ContactValueType.UtcOffset);
			return ContactCommon.ParseUtcOffset(s, this.reader.ComplianceTracker);
		}

		public TimeSpan ReadValueAsTimeSpan(ContactValueSeparators expectedSeparators)
		{
			return this.ReadValueAsTimeSpan(new ContactValueSeparators?(expectedSeparators));
		}

		public TimeSpan ReadValueAsTimeSpan()
		{
			return this.ReadValueAsTimeSpan(null);
		}

		private bool ReadValueAsBoolean(ContactValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string value = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(ContactValueType.Boolean);
			bool result;
			if (!bool.TryParse(value, out result))
			{
				this.reader.ComplianceTracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
			return result;
		}

		public bool ReadValueAsBoolean(ContactValueSeparators expectedSeparators)
		{
			return this.ReadValueAsBoolean(new ContactValueSeparators?(expectedSeparators));
		}

		public bool ReadValueAsBoolean()
		{
			return this.ReadValueAsBoolean(null);
		}

		private double ReadValueAsDouble(ContactValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(ContactValueType.Float);
			double result;
			if (!double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
			{
				this.reader.ComplianceTracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
			return result;
		}

		public double ReadValueAsDouble(ContactValueSeparators expectedSeparators)
		{
			return this.ReadValueAsDouble(new ContactValueSeparators?(expectedSeparators));
		}

		public double ReadValueAsDouble()
		{
			return this.ReadValueAsDouble(null);
		}

		private int ReadValueAsInt32(ContactValueSeparators? expectedSeparators)
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			string s = this.ReadValueAsString(expectedSeparators).Trim();
			this.CheckType(ContactValueType.Integer);
			int result;
			if (!int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
			{
				this.reader.ComplianceTracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
			return result;
		}

		public int ReadValueAsInt32(ContactValueSeparators expectedSeparators)
		{
			return this.ReadValueAsInt32(new ContactValueSeparators?(expectedSeparators));
		}

		public int ReadValueAsInt32()
		{
			return this.ReadValueAsInt32(null);
		}

		public bool ReadNextValue()
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property | ContentLineNodeType.DocumentEnd);
			return this.reader.ReadNextPropertyValue();
		}

		public bool ReadNextProperty()
		{
			this.reader.AssertValidState(ContentLineNodeType.ComponentStart | ContentLineNodeType.ComponentEnd | ContentLineNodeType.Parameter | ContentLineNodeType.Property | ContentLineNodeType.BeforeComponentStart | ContentLineNodeType.BeforeComponentEnd | ContentLineNodeType.DocumentEnd);
			while (this.reader.ReadNextProperty())
			{
				if (this.Name != string.Empty)
				{
					return true;
				}
			}
			return false;
		}

		public void ApplyValueOverrides(Encoding charset, ByteEncoder decoder)
		{
			this.reader.ApplyValueOverrides(charset, decoder);
		}

		public Stream GetValueReadStream()
		{
			this.reader.AssertValidState(ContentLineNodeType.Parameter | ContentLineNodeType.Property);
			this.lastSeparator = ContactValueSeparators.None;
			return this.reader.GetValueReadStream();
		}

		private void CheckType(ContactValueType type)
		{
			if (this.ValueType != type && this.ValueType != ContactValueType.Text)
			{
				this.reader.ComplianceTracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidValueFormat);
			}
		}

		private ContentLineReader reader;

		private ContactValueSeparators lastSeparator;
	}
}
