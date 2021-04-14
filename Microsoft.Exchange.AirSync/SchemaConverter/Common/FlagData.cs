using System;
using System.Collections;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal class FlagData : INestedData
	{
		public FlagData()
		{
			this.subProperties = new Hashtable();
		}

		public ExDateTime? CompleteTime
		{
			get
			{
				return this.GetDateTimeProperty("CompleteTime");
			}
			set
			{
				this.subProperties["CompleteTime"] = ((value != null) ? value.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo) : null);
			}
		}

		public ExDateTime? DateCompleted
		{
			get
			{
				return this.GetDateTimeProperty("DateCompleted");
			}
			set
			{
				this.subProperties["DateCompleted"] = ((value != null) ? value.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo) : null);
			}
		}

		public ExDateTime? DueDate
		{
			get
			{
				return this.GetDateTimeProperty("DueDate");
			}
			set
			{
				this.subProperties["DueDate"] = ((value != null) ? value.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo) : null);
			}
		}

		public ExDateTime? OrdinalDate
		{
			get
			{
				return this.GetDateTimeProperty("OrdinalDate");
			}
			set
			{
				this.subProperties["OrdinalDate"] = ((value != null) ? value.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo) : null);
			}
		}

		public bool? ReminderSet
		{
			get
			{
				string text = this.subProperties["ReminderSet"] as string;
				if (text == null)
				{
					return null;
				}
				return new bool?(text == "1");
			}
			set
			{
				this.subProperties["ReminderSet"] = ((value != null) ? (value.Value ? "1" : "0") : null);
			}
		}

		public ExDateTime? ReminderTime
		{
			get
			{
				return this.GetDateTimeProperty("ReminderTime");
			}
			set
			{
				this.subProperties["ReminderTime"] = ((value != null) ? value.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo) : null);
			}
		}

		public ExDateTime? StartDate
		{
			get
			{
				return this.GetDateTimeProperty("StartDate");
			}
			set
			{
				this.subProperties["StartDate"] = ((value != null) ? value.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo) : null);
			}
		}

		public int? Status
		{
			get
			{
				string text = this.subProperties["Status"] as string;
				if (text == null)
				{
					return null;
				}
				return new int?(int.Parse(text, CultureInfo.InvariantCulture));
			}
			set
			{
				this.subProperties["Status"] = ((value != null) ? value.Value.ToString(CultureInfo.InvariantCulture) : null);
			}
		}

		public string Subject
		{
			get
			{
				return this.subProperties["Subject"] as string;
			}
			set
			{
				this.subProperties["Subject"] = value;
			}
		}

		public string SubOrdinalDate
		{
			get
			{
				return this.subProperties["SubOrdinalDate"] as string;
			}
			set
			{
				this.subProperties["SubOrdinalDate"] = value;
			}
		}

		public IDictionary SubProperties
		{
			get
			{
				return this.subProperties;
			}
		}

		public string Type
		{
			get
			{
				return this.subProperties["FlagType"] as string;
			}
			set
			{
				this.subProperties["FlagType"] = value;
			}
		}

		public ExDateTime? UtcDueDate
		{
			get
			{
				return this.GetDateTimeProperty("UtcDueDate");
			}
			set
			{
				this.subProperties["UtcDueDate"] = ((value != null) ? value.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo) : null);
			}
		}

		public ExDateTime? UtcStartDate
		{
			get
			{
				return this.GetDateTimeProperty("UtcStartDate");
			}
			set
			{
				this.subProperties["UtcStartDate"] = ((value != null) ? value.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo) : null);
			}
		}

		public static bool IsTaskProperty(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}
			return propertyName == "StartDate" || propertyName == "UtcStartDate" || propertyName == "DueDate" || propertyName == "UtcDueDate" || propertyName == "DateCompleted" || propertyName == "ReminderSet" || propertyName == "ReminderTime" || propertyName == "Subject" || propertyName == "OrdinalDate" || propertyName == "SubOrdinalDate";
		}

		public void Clear()
		{
			this.subProperties.Clear();
		}

		public bool ContainsValidData()
		{
			return this.subProperties.Count > 0;
		}

		private ExDateTime? GetDateTimeProperty(string propertyName)
		{
			string text = this.subProperties[propertyName] as string;
			if (text == null)
			{
				return null;
			}
			ExDateTime value;
			if (!ExDateTime.TryParseExact(text, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out value))
			{
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
				{
					ErrorStringForProtocolLogger = "InvalidDateTimeInFlagData"
				};
			}
			return new ExDateTime?(value);
		}

		private IDictionary subProperties;
	}
}
