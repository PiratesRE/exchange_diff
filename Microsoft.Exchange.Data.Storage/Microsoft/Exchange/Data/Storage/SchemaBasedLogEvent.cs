using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SchemaBasedLogEvent<T> : ILogEvent, IEnumerable
	{
		public SchemaBasedLogEvent()
		{
			this.data = new Dictionary<string, object>(StringComparer.Ordinal);
			this.recalculateStringValue = true;
		}

		public string EventId
		{
			get
			{
				return SchemaBasedLogEvent<T>.eventId;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return this.data;
		}

		public void Add(KeyValuePair<T, object> entry)
		{
			this.Add(entry.Key, entry.Value);
		}

		public void Add(T fieldId, object value)
		{
			string valueString = ContactLinkingStrings.GetValueString(value);
			if (valueString != null)
			{
				string fieldIdString = SchemaBasedLogEvent<T>.GetFieldIdString(fieldId);
				this.data[fieldIdString] = valueString;
				this.recalculateStringValue = true;
			}
		}

		public override string ToString()
		{
			if (this.recalculateStringValue)
			{
				bool flag;
				string text = LogRowFormatter.FormatCollection(this.GetEventData(), true, out flag);
				this.stringValue = string.Format("S:EventId:{0};{1}", this.EventId, flag ? SchemaBasedLogEvent<T>.GetCsvEscapedString(text) : text);
				this.recalculateStringValue = false;
			}
			return this.stringValue;
		}

		private static string GetFieldIdString(T fieldId)
		{
			if (SchemaBasedLogEvent<T>.fieldIdStrings == null)
			{
				T[] array = (T[])Enum.GetValues(typeof(T));
				Dictionary<T, string> dictionary = new Dictionary<T, string>(array.Length);
				foreach (T key in array)
				{
					dictionary.Add(key, key.ToString());
				}
				SchemaBasedLogEvent<T>.fieldIdStrings = dictionary;
			}
			return SchemaBasedLogEvent<T>.fieldIdStrings[fieldId];
		}

		private static string GetCsvEscapedString(object value)
		{
			return Encoding.UTF8.GetString(Utf8Csv.EncodeAndEscape(value.ToString(), true));
		}

		private static readonly string eventId = typeof(T).Name;

		private readonly Dictionary<string, object> data;

		private static Dictionary<T, string> fieldIdStrings;

		private string stringValue;

		private bool recalculateStringValue;
	}
}
