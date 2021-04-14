using System;
using System.Collections;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal class MeetingRequestData : INestedData
	{
		public MeetingRequestData(int protocolVersion)
		{
			this.protocolVersion = protocolVersion;
			this.subProperties = new Hashtable(MeetingRequestData.keysV141.Length);
		}

		public bool AllDayEvent
		{
			get
			{
				return this.subProperties.Contains(MeetingRequestData.keysPreV14[0]) && ((string)this.subProperties[MeetingRequestData.keysPreV14[0]]).Equals("1", StringComparison.Ordinal);
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[0]] = (value ? "1" : "0");
			}
		}

		public bool DisallowNewTimeProposal
		{
			get
			{
				return this.subProperties.Contains(MeetingRequestData.keysV14[16]) && ((string)this.subProperties[MeetingRequestData.keysV14[16]]).Equals("1", StringComparison.Ordinal);
			}
			set
			{
				this.subProperties[MeetingRequestData.keysV14[16]] = (value ? "1" : "0");
			}
		}

		public int BusyStatus
		{
			get
			{
				if (!this.subProperties.Contains(MeetingRequestData.keysPreV14[12]))
				{
					return -1;
				}
				int result;
				if (!int.TryParse((string)this.subProperties[MeetingRequestData.keysPreV14[12]], NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
				{
					return -1;
				}
				return result;
			}
			set
			{
				if (value >= MeetingRequestData.busyStatusArray.Length || value < 0)
				{
					this.subProperties[MeetingRequestData.keysPreV14[12]] = MeetingRequestData.busyStatusArray[2];
					return;
				}
				this.subProperties[MeetingRequestData.keysPreV14[12]] = MeetingRequestData.busyStatusArray[value];
			}
		}

		public string[] Categories
		{
			get
			{
				if (this.subProperties.Contains(MeetingRequestData.keysPreV14[15]))
				{
					return (string[])this.subProperties[MeetingRequestData.keysPreV14[15]];
				}
				return new string[0];
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[15]] = value;
			}
		}

		public ExDateTime DtStamp
		{
			get
			{
				if (this.subProperties.Contains(MeetingRequestData.keysPreV14[2]))
				{
					return TimeZoneConverter.Parse((string)this.subProperties[MeetingRequestData.keysPreV14[2]], "yyyy-MM-dd\\THH:mm:ss.fff\\Z", this.protocolVersion, "DtStamp");
				}
				return ExDateTime.MinValue;
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[2]] = TimeZoneConverter.ToString(value, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", this.protocolVersion);
			}
		}

		public ExDateTime EndTime
		{
			get
			{
				if (this.subProperties.Contains(MeetingRequestData.keysPreV14[3]))
				{
					return TimeZoneConverter.Parse((string)this.subProperties[MeetingRequestData.keysPreV14[3]], "yyyy-MM-dd\\THH:mm:ss.fff\\Z", this.protocolVersion, "EndTime");
				}
				return ExDateTime.MinValue;
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[3]] = TimeZoneConverter.ToString(value, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", this.protocolVersion);
			}
		}

		public string GlobalObjId
		{
			get
			{
				if (this.subProperties.Contains(MeetingRequestData.keysPreV14[14]))
				{
					return (string)this.subProperties[MeetingRequestData.keysPreV14[14]];
				}
				return string.Empty;
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[14]] = value;
			}
		}

		public int InstanceType
		{
			get
			{
				if (!this.subProperties.Contains(MeetingRequestData.keysPreV14[4]))
				{
					return -1;
				}
				int result;
				if (int.TryParse((string)this.subProperties[MeetingRequestData.keysPreV14[4]], NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				return -1;
			}
			set
			{
				if (value >= MeetingRequestData.instanceTypeArray.Length || value < 0)
				{
					throw new ConversionException("InstanceType=" + value);
				}
				this.subProperties[MeetingRequestData.keysPreV14[4]] = MeetingRequestData.instanceTypeArray[value].ToString(CultureInfo.InvariantCulture);
			}
		}

		public string Location
		{
			get
			{
				if (this.subProperties.Contains(MeetingRequestData.keysPreV14[5]))
				{
					return (string)this.subProperties[MeetingRequestData.keysPreV14[5]];
				}
				return string.Empty;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.subProperties[MeetingRequestData.keysPreV14[5]] = value;
				}
			}
		}

		public EnhancedLocationData EnhancedLocation
		{
			get
			{
				if (this.subProperties.Contains(MeetingRequestData.keysPreV14[5]))
				{
					return (EnhancedLocationData)this.subProperties[MeetingRequestData.keysPreV14[5]];
				}
				return null;
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[5]] = value;
			}
		}

		public string Organizer
		{
			get
			{
				if (this.subProperties.Contains(MeetingRequestData.keysPreV14[6]))
				{
					return (string)this.subProperties[MeetingRequestData.keysPreV14[6]];
				}
				return string.Empty;
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[6]] = value;
			}
		}

		public ExDateTime RecurrenceId
		{
			get
			{
				if (!this.subProperties.Contains(MeetingRequestData.keysPreV14[7]))
				{
					return ExDateTime.MinValue;
				}
				ExDateTime result;
				if (!ExDateTime.TryParseExact((string)this.subProperties[MeetingRequestData.keysPreV14[7]], "yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidDateTimeInMeetingRequestData3"
					};
				}
				return result;
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[7]] = value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo);
			}
		}

		public RecurrenceData Recurrences
		{
			get
			{
				if (this.subProperties.Contains(MeetingRequestData.keysPreV14[10]))
				{
					return (RecurrenceData)this.subProperties[MeetingRequestData.keysPreV14[10]];
				}
				return null;
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[10]] = value;
			}
		}

		public int Reminder
		{
			get
			{
				if (!this.subProperties.Contains(MeetingRequestData.keysPreV14[8]))
				{
					return -1;
				}
				int result;
				if (int.TryParse((string)this.subProperties[MeetingRequestData.keysPreV14[8]], NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				return -1;
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[8]] = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public bool ResponseRequested
		{
			get
			{
				return !this.subProperties.Contains(MeetingRequestData.keysPreV14[9]) || !((string)this.subProperties[MeetingRequestData.keysPreV14[9]]).Equals("0", StringComparison.Ordinal);
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[9]] = (value ? "1" : "0");
			}
		}

		public int Sensitivity
		{
			get
			{
				if (this.subProperties.Contains(MeetingRequestData.keysPreV14[11]))
				{
					return int.Parse((string)this.subProperties[MeetingRequestData.keysPreV14[11]], CultureInfo.InvariantCulture);
				}
				return -1;
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[11]] = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public ExDateTime StartTime
		{
			get
			{
				if (this.subProperties.Contains(MeetingRequestData.keysPreV14[1]))
				{
					return TimeZoneConverter.Parse((string)this.subProperties[MeetingRequestData.keysPreV14[1]], "yyyy-MM-dd\\THH:mm:ss.fff\\Z", this.protocolVersion, "StartTime");
				}
				return ExDateTime.MinValue;
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[1]] = TimeZoneConverter.ToString(value, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", this.protocolVersion);
			}
		}

		public AirSyncMeetingMessageType MeetingMessageType
		{
			set
			{
				IDictionary dictionary = this.subProperties;
				object key = MeetingRequestData.keysV141[17];
				int num = (int)value;
				dictionary[key] = num.ToString(CultureInfo.InvariantCulture);
			}
		}

		public IDictionary SubProperties
		{
			get
			{
				return this.subProperties;
			}
		}

		public string TimeZoneSubProperty
		{
			get
			{
				if (this.subProperties.Contains(MeetingRequestData.keysPreV14[13]))
				{
					return (string)this.subProperties[MeetingRequestData.keysPreV14[13]];
				}
				return string.Empty;
			}
			set
			{
				this.subProperties[MeetingRequestData.keysPreV14[13]] = value;
			}
		}

		public static string[] GetKeysForVersion(int version)
		{
			if (version < 140)
			{
				return MeetingRequestData.keysPreV14;
			}
			if (version == 140)
			{
				return MeetingRequestData.keysV14;
			}
			return MeetingRequestData.keysV141;
		}

		public static string GetEmailNamespaceForKey(int keyIndex)
		{
			if (keyIndex < MeetingRequestData.keysV14.Length)
			{
				return "Email:";
			}
			if (keyIndex < MeetingRequestData.keysV141.Length)
			{
				return "Email2:";
			}
			throw new InvalidOperationException(string.Format("keyIndex value {0} is invalid.", keyIndex));
		}

		public bool HasSubProperty(MeetingRequestData.Tags tag)
		{
			return null != this.subProperties[MeetingRequestData.keysPreV14[(int)tag]];
		}

		public void Clear()
		{
			this.subProperties.Clear();
		}

		private static readonly string[] keysPreV14 = new string[]
		{
			"AllDayEvent",
			"StartTime",
			"DtStamp",
			"EndTime",
			"InstanceType",
			"Location",
			"Organizer",
			"RecurrenceId",
			"Reminder",
			"ResponseRequested",
			"Recurrences",
			"Sensitivity",
			"IntDBusyStatus",
			"TimeZone",
			"GlobalObjId",
			"Categories"
		};

		private static readonly string[] keysV14 = new string[]
		{
			"AllDayEvent",
			"StartTime",
			"DtStamp",
			"EndTime",
			"InstanceType",
			"Location",
			"Organizer",
			"RecurrenceId",
			"Reminder",
			"ResponseRequested",
			"Recurrences",
			"Sensitivity",
			"IntDBusyStatus",
			"TimeZone",
			"GlobalObjId",
			"Categories",
			"DisallowNewTimeProposal"
		};

		private static readonly string[] keysV141 = new string[]
		{
			"AllDayEvent",
			"StartTime",
			"DtStamp",
			"EndTime",
			"InstanceType",
			"Location",
			"Organizer",
			"RecurrenceId",
			"Reminder",
			"ResponseRequested",
			"Recurrences",
			"Sensitivity",
			"IntDBusyStatus",
			"TimeZone",
			"GlobalObjId",
			"Categories",
			"DisallowNewTimeProposal",
			"MeetingMessageType"
		};

		private static readonly string[] keysV160 = new string[]
		{
			"AllDayEvent",
			"StartTime",
			"DtStamp",
			"EndTime",
			"InstanceType",
			"Location",
			"Organizer",
			"Reminder",
			"ResponseRequested",
			"Recurrences",
			"Sensitivity",
			"IntDBusyStatus",
			"GlobalObjId",
			"Categories",
			"DisallowNewTimeProposal",
			"MeetingMessageType"
		};

		private static readonly string[] busyStatusArray = new string[]
		{
			"0",
			"1",
			"2",
			"3"
		};

		private static readonly int[] instanceTypeArray = new int[]
		{
			0,
			2,
			3,
			1
		};

		private readonly int protocolVersion;

		private IDictionary subProperties;

		public enum Tags
		{
			AllDayEvent,
			StartTime,
			DtStamp,
			EndTime,
			InstanceType,
			Location,
			Organizer,
			RecurrenceId,
			Reminder,
			ResponseRequested,
			Recurrences,
			Sensitivity,
			BusyStatus,
			TimeZone,
			GlobalObjId,
			Categories,
			DisallowNewTimeProposal,
			MeetingMessageType
		}
	}
}
