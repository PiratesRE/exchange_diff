using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	public class LogRowFormatter
	{
		public LogRowFormatter(LogSchema schema) : this(schema, LogRowFormatter.DefaultEscapeLineBreaks)
		{
		}

		public LogRowFormatter(LogSchema schema, bool escapeLineBreaks) : this(schema, escapeLineBreaks, true)
		{
		}

		public LogRowFormatter(LogSchema schema, bool escapeLineBreaks, bool escapeRawData)
		{
			this.fields = new object[schema.Fields.Length];
			this.encodedFields = new byte[schema.Fields.Length][];
			this.escapeLineBreaks = escapeLineBreaks;
			this.escapeRawData = escapeRawData;
		}

		public LogRowFormatter(LogRowFormatter copy)
		{
			this.fields = new object[copy.fields.Length];
			this.encodedFields = new byte[copy.fields.Length][];
			this.escapeLineBreaks = copy.escapeLineBreaks;
			this.escapeRawData = copy.escapeRawData;
			for (int i = 0; i < this.fields.Length; i++)
			{
				this.fields[i] = copy.fields[i];
				this.encodedFields[i] = copy.encodedFields[i];
			}
		}

		public object this[int index]
		{
			get
			{
				return this.fields[index];
			}
			set
			{
				this.fields[index] = value;
				this.encodedFields[index] = this.Encode(value);
			}
		}

		public virtual bool ShouldConvertEncoding
		{
			get
			{
				return true;
			}
		}

		public static string FormatCollection(IEnumerable data)
		{
			bool flag;
			return LogRowFormatter.FormatCollection(data, out flag);
		}

		public static string FormatCollection(IEnumerable data, out bool needsEscaping)
		{
			return LogRowFormatter.FormatCollection(data, LogRowFormatter.DefaultEscapeLineBreaks, out needsEscaping);
		}

		public static string FormatCollection(IEnumerable data, bool escapeLineBreaks, out bool needsEscaping)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			needsEscaping = false;
			foreach (object data2 in data)
			{
				bool flag;
				string text = LogRowFormatter.Format(data2, out flag);
				if (flag)
				{
					needsEscaping = true;
					Utf8Csv.EscapeAndAppendCollectionMember(stringBuilder, text, escapeLineBreaks);
				}
				else
				{
					Utf8Csv.AppendCollectionMember(stringBuilder, text);
				}
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}

		public static string Format(object data)
		{
			bool flag;
			return LogRowFormatter.Format(data, out flag);
		}

		internal void Write(Stream output)
		{
			Utf8Csv.WriteRawRow(output, this.encodedFields);
		}

		protected virtual byte[] Encode(object data)
		{
			if (data == null)
			{
				return null;
			}
			if (data is byte[])
			{
				return this.EncodeBytes((byte[])data);
			}
			if (!(data is string) && data is IEnumerable)
			{
				return this.EncodeCollection((IEnumerable)data);
			}
			bool flag;
			string s = LogRowFormatter.Format(data, out flag);
			if (!flag)
			{
				return Utf8Csv.Encode(s);
			}
			return Utf8Csv.EncodeAndEscape(s, this.escapeLineBreaks);
		}

		protected virtual byte[] EncodeBytes(byte[] data)
		{
			if (this.escapeRawData)
			{
				byte[] data2 = data;
				if (this.ShouldConvertEncoding)
				{
					data2 = Encoding.Convert(Encoding.GetEncoding("iso-8859-1"), Encoding.UTF8, data);
				}
				return Utf8Csv.Escape(data2, true);
			}
			return data;
		}

		protected virtual byte[] EncodeCollection(IEnumerable data)
		{
			bool flag;
			string s = LogRowFormatter.FormatCollection(data, this.escapeLineBreaks, out flag);
			if (!flag)
			{
				return Utf8Csv.Encode(s);
			}
			return Utf8Csv.EncodeAndEscape(s, this.escapeLineBreaks);
		}

		private static string Format(object data, out bool needsEscaping)
		{
			if (data == null)
			{
				needsEscaping = false;
				return string.Empty;
			}
			needsEscaping = true;
			string result;
			if (data is Guid)
			{
				needsEscaping = false;
				result = ((Guid)data).ToString();
			}
			else if (data is DateTime)
			{
				needsEscaping = false;
				result = ((DateTime)data).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo);
			}
			else if (data is int)
			{
				result = ((int)data).ToString(NumberFormatInfo.InvariantInfo);
			}
			else if (data is KeyValuePair<string, object>)
			{
				result = LogRowFormatter.EncodeKeyValuePair((KeyValuePair<string, object>)data);
			}
			else if (data is float)
			{
				result = ((float)data).ToString(NumberFormatInfo.InvariantInfo);
			}
			else if (data is double)
			{
				result = ((double)data).ToString(NumberFormatInfo.InvariantInfo);
			}
			else
			{
				result = data.ToString();
			}
			return result;
		}

		private static string EncodeKeyValuePair(KeyValuePair<string, object> keyValuePair)
		{
			string key = keyValuePair.Key;
			object value = keyValuePair.Value;
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("keyValuePair", "Key cannot be null/empty");
			}
			if (!SpecialCharacters.IsValidKey(key))
			{
				throw new ArgumentException("Invalid key in KeyValuePair", key);
			}
			string arg;
			string arg2;
			if (value is DateTime)
			{
				arg = "Dt";
				arg2 = ((DateTime)value).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo);
			}
			else if (value is int)
			{
				arg = "I32";
				arg2 = ((int)value).ToString(NumberFormatInfo.InvariantInfo);
			}
			else if (value is string)
			{
				arg = "S";
				arg2 = (string)value;
			}
			else if (value is Guid)
			{
				arg = "S";
				arg2 = ((Guid)value).ToString();
			}
			else if (value is float)
			{
				arg = "F";
				arg2 = ((float)value).ToString(NumberFormatInfo.InvariantInfo);
			}
			else if (value is double)
			{
				arg = "Dbl";
				arg2 = ((double)value).ToString(NumberFormatInfo.InvariantInfo);
			}
			else
			{
				arg = "S";
				arg2 = "[LogRowFormatter:EncodeKeyValuePair Invalid Data Type for value]";
			}
			return string.Format("{0}:{1}={2}", arg, keyValuePair.Key, arg2);
		}

		public static readonly bool DefaultEscapeLineBreaks = true;

		private readonly object[] fields;

		private readonly byte[][] encodedFields;

		private readonly bool escapeLineBreaks;

		private readonly bool escapeRawData;
	}
}
