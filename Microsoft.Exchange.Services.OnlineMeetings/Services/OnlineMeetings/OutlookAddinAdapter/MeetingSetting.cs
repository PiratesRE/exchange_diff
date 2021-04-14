using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[XmlType("Settings")]
	[DataContract(Name = "Settings")]
	[KnownType(typeof(MeetingSetting))]
	public class MeetingSetting
	{
		[DataMember(Name = "Public")]
		[XmlElement("Public")]
		public bool IsPublic { get; set; }

		[XmlElement("ConferenceID")]
		[DataMember(Name = "ConferenceID")]
		public string ConferenceID { get; set; }

		[DataMember(Name = "HttpJoinLink")]
		[XmlElement("HttpJoinLink")]
		public string HttpJoinLink { get; set; }

		[DataMember(Name = "ConfJoinLink")]
		[XmlElement("ConfJoinLink")]
		public string ConfJoinLink { get; set; }

		[DataMember(Name = "Subject", EmitDefaultValue = true)]
		[XmlElement("Subject")]
		public string Subject { get; set; }

		[XmlIgnore]
		public DateTime? ExpiryDate { get; set; }

		[XmlElement("ExpiryDate")]
		[DataMember(Name = "ExpiryDate", EmitDefaultValue = false)]
		public string ExpiryDateString
		{
			get
			{
				if (this.ExpiryDate == null)
				{
					return string.Empty;
				}
				return this.ExpiryDate.Value.ToString(CultureInfo.InvariantCulture.DateTimeFormat);
			}
			set
			{
				DateTime value2;
				if (DateTime.TryParse(value, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out value2))
				{
					this.ExpiryDate = new DateTime?(value2);
					return;
				}
				this.ExpiryDate = null;
			}
		}

		[XmlElement("AutoPromote")]
		[DataMember(Name = "AutoPromote")]
		public AutoPromote AutoPromote { get; set; }

		[DataMember(Name = "BodyLanguage")]
		[XmlElement("BodyLanguage")]
		public string BodyLanguage { get; set; }

		[XmlElement("Participants")]
		[DataMember(Name = "Participants")]
		public Participants Participants { get; set; }

		[XmlElement("Permissions")]
		[DataMember(Name = "Permissions")]
		public Permissions Permissions { get; set; }

		[DataMember(Name = "MeetingOwner")]
		[XmlElement("MeetingOwner")]
		public MeetingOwner MeetingOwner { get; set; }

		[XmlElement("Audio")]
		[DataMember(Name = "Audio")]
		public Audio Audio { get; set; }

		internal static string GetBodyLanguage(DialInRegions dialInRegions)
		{
			if (dialInRegions.Count == 0 || dialInRegions[0].Languages == null || dialInRegions[0].Languages.Count == 0)
			{
				return string.Empty;
			}
			string result = string.Empty;
			try
			{
				result = new CultureInfo(dialInRegions[0].Languages[0]).LCID.ToString("x");
			}
			catch (CultureNotFoundException ex)
			{
				ExTraceGlobals.OnlineMeetingTracer.TraceError<string, string>(0, 0L, "MeetingSetting::GetBodyLanguage. CultureNotFoundException when parsing language {0}. Exception message: {1}", dialInRegions[0].Languages[0], (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
			}
			return result;
		}
	}
}
