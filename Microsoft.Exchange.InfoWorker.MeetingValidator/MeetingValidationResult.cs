using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	public class MeetingValidationResult : IXmlSerializable
	{
		private MeetingValidationResult()
		{
			this.isConsistent = true;
			this.wasValidationSuccessful = false;
			this.ResultsPerAttendee = new Dictionary<string, MeetingComparisonResult>();
		}

		internal MeetingValidationResult(ExDateTime intervalStartDate, ExDateTime intervalEndDate, UserObject mailboxUser, MeetingData meetingData, bool duplicatesDetected, string errorDescription) : this()
		{
			this.numberOfDelegates = mailboxUser.ExchangePrincipal.Delegates.Count<ADObjectId>();
			this.meetingData = meetingData;
			this.duplicatesDetected = duplicatesDetected;
			this.intervalStartDate = intervalStartDate;
			this.intervalEndDate = intervalEndDate;
			this.errorDescription = errorDescription;
		}

		internal MeetingValidationResult(ExDateTime intervalStartDate, ExDateTime intervalEndDate, UserObject mailboxUser, MeetingData meetingDataKept, List<MeetingValidationResult> duplicates) : this()
		{
			this.numberOfDelegates = mailboxUser.ExchangePrincipal.Delegates.Count<ADObjectId>();
			this.meetingData = meetingDataKept;
			this.duplicatesDetected = true;
			this.intervalStartDate = intervalStartDate;
			this.DuplicateResults = duplicates;
		}

		internal bool WasValidationSuccessful
		{
			get
			{
				return this.wasValidationSuccessful;
			}
			set
			{
				this.wasValidationSuccessful = value;
			}
		}

		internal bool DuplicatesDetected
		{
			get
			{
				return this.duplicatesDetected;
			}
		}

		internal bool IsDuplicate { get; set; }

		internal bool IsDuplicateRemoved { get; set; }

		internal StoreId MeetingId
		{
			get
			{
				return this.meetingData.Id;
			}
		}

		internal MeetingData MeetingData
		{
			get
			{
				return this.meetingData;
			}
		}

		internal GlobalObjectId GlobalObjectId
		{
			get
			{
				return this.meetingData.GlobalObjectId;
			}
		}

		internal string CleanGlobalObjectId
		{
			get
			{
				return this.meetingData.CleanGlobalObjectId;
			}
		}

		internal string Subject
		{
			get
			{
				return this.meetingData.Subject;
			}
		}

		internal CalendarItemType ItemType
		{
			get
			{
				return this.meetingData.CalendarItemType;
			}
		}

		internal ExDateTime StartTime
		{
			get
			{
				return this.meetingData.StartTime;
			}
		}

		internal ExDateTime EndTime
		{
			get
			{
				return this.meetingData.EndTime;
			}
		}

		internal ExDateTime IntervalStartDate
		{
			get
			{
				return this.intervalStartDate;
			}
		}

		internal ExDateTime IntervalEndDate
		{
			get
			{
				return this.intervalEndDate;
			}
		}

		internal string MailboxUserPrimarySmtpAddress
		{
			get
			{
				return this.meetingData.MailboxUserPrimarySmtpAddress;
			}
		}

		internal int NumberOfDelegates
		{
			get
			{
				return this.numberOfDelegates;
			}
		}

		internal string OrganizerPrimarySmtpAddress
		{
			get
			{
				return this.meetingData.OrganizerPrimarySmtpAddress;
			}
		}

		internal bool IsOrganizer
		{
			get
			{
				return this.meetingData.IsOrganizer;
			}
		}

		internal bool IsConsistent
		{
			get
			{
				return this.isConsistent;
			}
			set
			{
				this.isConsistent = value;
			}
		}

		internal string FixupDescription
		{
			get
			{
				return this.fixupDescription;
			}
			set
			{
				this.fixupDescription = value;
			}
		}

		internal Dictionary<string, MeetingComparisonResult> ResultsPerAttendee { get; private set; }

		internal string ErrorDescription
		{
			get
			{
				return this.errorDescription;
			}
			set
			{
				this.errorDescription = value;
			}
		}

		internal string Location
		{
			get
			{
				return this.meetingData.Location;
			}
		}

		internal ExDateTime CreationTime
		{
			get
			{
				return this.meetingData.CreationTime;
			}
		}

		internal ExDateTime LastModifiedTime
		{
			get
			{
				return this.meetingData.LastModifiedTime;
			}
		}

		internal List<MeetingValidationResult> DuplicateResults { get; private set; }

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotSupportedException("XML deserialization is not supported.");
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("type", this.ItemType.ToString());
			writer.WriteAttributeString("validating", this.IsOrganizer ? "organizer" : "attendee");
			if (this.MeetingId != null)
			{
				writer.WriteElementString("MeetingId", this.MeetingId.ToBase64String());
			}
			else
			{
				writer.WriteElementString("MeetingId", "0");
			}
			if (!this.wasValidationSuccessful)
			{
				writer.WriteElementString("ErrorDescription", this.errorDescription);
			}
			writer.WriteElementString("GlobalObjectId", this.GlobalObjectId.ToString());
			writer.WriteElementString("CleanGlobalObjectId", this.CleanGlobalObjectId);
			writer.WriteElementString("CreationTime", this.CreationTime.ToString());
			writer.WriteElementString("LastModifiedTime", this.LastModifiedTime.ToString());
			writer.WriteElementString("Subject", this.Subject);
			writer.WriteElementString("StartTime", this.StartTime.ToString());
			writer.WriteElementString("EndTime", this.EndTime.ToString());
			writer.WriteElementString("Location", this.Location);
			writer.WriteElementString("Organizer", this.OrganizerPrimarySmtpAddress);
			writer.WriteElementString("IsConsistent", this.isConsistent.ToString());
			writer.WriteElementString("DuplicatesDetected", this.duplicatesDetected.ToString());
			writer.WriteElementString("IsDuplicate", this.IsDuplicate.ToString());
			writer.WriteElementString("MeetingDeleted", this.IsDuplicateRemoved.ToString());
			writer.Flush();
			if (this.MeetingData != null)
			{
				writer.WriteElementString("ExtractVersion", string.Format("0x{0:X}", this.MeetingData.ExtractVersion));
				writer.WriteElementString("ExtractTime", this.MeetingData.ExtractTime.ToString());
				writer.WriteElementString("HasConflicts", this.MeetingData.HasConflicts.ToString());
				writer.WriteElementString("NumDelegates", this.numberOfDelegates.ToString());
				writer.WriteElementString("InternetMessageId", this.MeetingData.InternetMessageId);
				if (this.MeetingData.SequenceNumber != -2147483648)
				{
					writer.WriteElementString("SequenceNumber", this.MeetingData.SequenceNumber.ToString());
				}
				if (this.MeetingData.LastSequenceNumber != -2147483648)
				{
					writer.WriteElementString("LastSequenceNumber", this.MeetingData.LastSequenceNumber.ToString());
				}
				if (this.MeetingData.OwnerAppointmentId != null)
				{
					writer.WriteElementString("OwnerApptId", this.MeetingData.OwnerAppointmentId.Value.ToString());
				}
				if (this.MeetingData.OwnerCriticalChangeTime != ExDateTime.MinValue)
				{
					writer.WriteElementString("OwnerCriticalChangeTime", this.MeetingData.OwnerCriticalChangeTime.ToString());
				}
				if (this.MeetingData.AttendeeCriticalChangeTime != ExDateTime.MinValue)
				{
					writer.WriteElementString("AttendeeCriticalChangeTime", this.MeetingData.AttendeeCriticalChangeTime.ToString());
				}
			}
			foreach (KeyValuePair<string, MeetingComparisonResult> keyValuePair in this.ResultsPerAttendee)
			{
				writer.WriteStartElement("ResultsPerAttendee");
				writer.WriteAttributeString("attendee", keyValuePair.Key);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConsistencyCheckResult));
				foreach (ConsistencyCheckResult o in keyValuePair.Value)
				{
					xmlSerializer.Serialize(writer, o);
				}
				writer.WriteEndElement();
			}
			writer.Flush();
		}

		private bool wasValidationSuccessful;

		private MeetingData meetingData;

		private bool isConsistent = true;

		private string errorDescription;

		private bool duplicatesDetected;

		private string fixupDescription;

		private ExDateTime intervalStartDate;

		private ExDateTime intervalEndDate;

		private int numberOfDelegates;
	}
}
