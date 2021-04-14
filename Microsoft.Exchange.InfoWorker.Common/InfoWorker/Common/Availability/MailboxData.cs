using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MailboxData
	{
		[XmlElement]
		[DataMember]
		public EmailAddress Email
		{
			get
			{
				return this.email;
			}
			set
			{
				this.email = value;
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public MeetingAttendeeType AttendeeType
		{
			get
			{
				return this.attendeeType;
			}
			set
			{
				this.attendeeType = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "AttendeeType")]
		public string AttendeeTypeString
		{
			get
			{
				return EnumUtil.ToString<MeetingAttendeeType>(this.AttendeeType);
			}
			set
			{
				this.AttendeeType = EnumUtil.Parse<MeetingAttendeeType>(value);
			}
		}

		[XmlElement]
		[DataMember]
		public bool ExcludeConflicts
		{
			get
			{
				return this.excludeConflicts;
			}
			set
			{
				this.excludeConflicts = value;
			}
		}

		public override string ToString()
		{
			return string.Format("EmailAddress = {0}, Attendee Type = {1}, Exclude Conflicts = {2}", this.email, this.attendeeType, this.excludeConflicts);
		}

		public MailboxData()
		{
			this.Init();
		}

		internal MailboxData(EmailAddress email)
		{
			this.Init();
			this.email = email;
		}

		internal StoreObjectId AssociatedFolderId
		{
			get
			{
				return this.associatedFolderId;
			}
			set
			{
				this.associatedFolderId = value;
			}
		}

		[OnDeserializing]
		private void Init(StreamingContext context)
		{
			this.Init();
		}

		private void Init()
		{
			this.attendeeType = MeetingAttendeeType.Required;
		}

		private EmailAddress email;

		private MeetingAttendeeType attendeeType;

		private bool excludeConflicts;

		private StoreObjectId associatedFolderId;
	}
}
