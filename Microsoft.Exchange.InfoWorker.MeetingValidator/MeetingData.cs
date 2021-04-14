using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal class MeetingData : IComparable<MeetingData>
	{
		private MeetingData()
		{
		}

		private static MeetingData CreateDummyInstance(UserObject mailboxUser, UserObject organizer)
		{
			MeetingData meetingData = new MeetingData();
			meetingData.MailboxUserPrimarySmtpAddress = mailboxUser.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			if (organizer == null)
			{
				meetingData.OrganizerPrimarySmtpAddress = string.Empty;
			}
			else if (organizer.ExchangePrincipal == null)
			{
				meetingData.OrganizerPrimarySmtpAddress = organizer.EmailAddress;
			}
			else
			{
				meetingData.OrganizerPrimarySmtpAddress = organizer.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			}
			return meetingData;
		}

		internal static MeetingData CreateInstance(UserObject mailboxUser, UserObject organizer, CalendarItemBase calendarItem)
		{
			MeetingData meetingData = MeetingData.CreateDummyInstance(mailboxUser, organizer);
			meetingData.ExtractDataFromCalendarItem(calendarItem);
			return meetingData;
		}

		internal static MeetingData CreateInstance(UserObject mailboxUser, Item item)
		{
			MeetingData meetingData = MeetingData.CreateDummyInstance(mailboxUser, null);
			meetingData.ExtractDataFromItem(item);
			return meetingData;
		}

		internal static MeetingData CreateInstance(UserObject mailboxUser, StoreId id)
		{
			MeetingData meetingData = MeetingData.CreateDummyInstance(mailboxUser, null);
			meetingData.SetDefaultCalendarItemBasedProperties();
			meetingData.Id = id;
			return meetingData;
		}

		internal static MeetingData CreateInstance(UserObject mailboxUser, StoreId id, GlobalObjectId globalObjectId, int appointmentSequenceNumber, ExDateTime lastModifiedTime, ExDateTime ownerCriticalChangeTime, CalendarItemType itemType, string subject, int? itemVersion, int documentId)
		{
			MeetingData meetingData = MeetingData.CreateInstance(mailboxUser, id, globalObjectId, itemType);
			meetingData.SequenceNumber = appointmentSequenceNumber;
			meetingData.LastModifiedTime = lastModifiedTime;
			meetingData.OwnerCriticalChangeTime = ownerCriticalChangeTime;
			meetingData.CalendarItemType = itemType;
			meetingData.Subject = subject;
			meetingData.ItemVersion = itemVersion;
			meetingData.DocumentId = documentId;
			return meetingData;
		}

		internal static MeetingData CreateInstance(UserObject mailboxUser, StoreId meetingId, GlobalObjectId globalObjectId, CalendarItemType itemType)
		{
			MeetingData meetingData = MeetingData.CreateDummyInstance(mailboxUser, null);
			meetingData.Id = meetingId;
			meetingData.GlobalObjectId = globalObjectId;
			meetingData.CalendarItemType = itemType;
			return meetingData;
		}

		internal static MeetingData CreateInstance(UserObject mailboxUser, StoreId meetingId, GlobalObjectId globalObjectId, Exception exception)
		{
			MeetingData meetingData = MeetingData.CreateDummyInstance(mailboxUser, null);
			meetingData.Id = meetingId;
			meetingData.GlobalObjectId = globalObjectId;
			meetingData.Exception = exception;
			return meetingData;
		}

		public int CompareTo(MeetingData other)
		{
			if (other == null)
			{
				return 1;
			}
			if (this.Id == other.Id)
			{
				return 0;
			}
			if (this.Exception != null || other.Exception != null)
			{
				return ((other.Exception != null) ? 1 : 0) - ((this.Exception != null) ? 1 : 0);
			}
			int num;
			if (GlobalObjectId.CompareCleanGlobalObjectIds(this.GlobalObjectId.CleanGlobalObjectIdBytes, other.GlobalObjectId.CleanGlobalObjectIdBytes))
			{
				num = Nullable.Compare<int>(new int?(this.SequenceNumber), new int?(other.SequenceNumber));
				if (num != 0)
				{
					return num;
				}
				num = this.LastModifiedTime.CompareTo(other.LastModifiedTime, MeetingData.LastModifiedTimeTreshold);
				if (num != 0)
				{
					return num;
				}
				num = this.OwnerCriticalChangeTime.CompareTo(other.OwnerCriticalChangeTime);
				if (num != 0)
				{
					return num;
				}
				num = this.DocumentId.CompareTo(other.DocumentId);
				if (num != 0)
				{
					return num;
				}
				if (this.ItemVersion != null && other.ItemVersion != null)
				{
					num = Nullable.Compare<int>(this.ItemVersion, other.ItemVersion);
					if (num != 0)
					{
						return num;
					}
				}
			}
			else
			{
				ExDateTime startTime = this.StartTime;
				ExDateTime startTime2 = other.StartTime;
				num = ExDateTime.Compare(this.StartTime, other.StartTime);
			}
			return num;
		}

		public GlobalObjectId GlobalObjectId { get; private set; }

		public string CleanGlobalObjectId
		{
			get
			{
				if (this.cleanGlobaObjectId == null)
				{
					this.cleanGlobaObjectId = ((this.GlobalObjectId != null) ? GlobalObjectId.ByteArrayToHexString(this.GlobalObjectId.CleanGlobalObjectIdBytes) : string.Empty);
				}
				return this.cleanGlobaObjectId;
			}
		}

		public string Subject { get; private set; }

		public CalendarItemType CalendarItemType { get; private set; }

		public ExDateTime StartTime { get; private set; }

		public ExDateTime EndTime { get; private set; }

		public int? OwnerAppointmentId { get; private set; }

		public int? ItemVersion { get; private set; }

		public int DocumentId { get; private set; }

		internal bool HasConflicts { get; private set; }

		internal string MailboxUserPrimarySmtpAddress { get; private set; }

		internal string OrganizerPrimarySmtpAddress { get; private set; }

		internal bool IsOrganizer { get; private set; }

		internal ExDateTime ExtractTime { get; private set; }

		internal ExDateTime OwnerCriticalChangeTime { get; private set; }

		internal ExDateTime AttendeeCriticalChangeTime { get; private set; }

		internal long ExtractVersion { get; private set; }

		internal int SequenceNumber { get; private set; }

		internal int LastSequenceNumber { get; private set; }

		internal string InternetMessageId { get; private set; }

		internal ExDateTime CreationTime { get; private set; }

		internal ExDateTime LastModifiedTime { get; private set; }

		internal string Location { get; private set; }

		internal StoreId Id { get; private set; }

		internal Exception Exception { get; set; }

		internal bool HasDuplicates { get; set; }

		private void ExtractDataFromCalendarItem(CalendarItemBase calendarItem)
		{
			if (this.GetPropertiesFromStoreObject(calendarItem))
			{
				this.IsOrganizer = calendarItem.IsOrganizer();
				this.GlobalObjectId = calendarItem.GlobalObjectId;
				this.OwnerAppointmentId = calendarItem.OwnerAppointmentId;
				this.Subject = calendarItem.Subject;
				this.StartTime = calendarItem.StartTime;
				this.EndTime = calendarItem.EndTime;
				this.CalendarItemType = calendarItem.CalendarItemType;
				this.SequenceNumber = calendarItem.AppointmentSequenceNumber;
				this.LastSequenceNumber = calendarItem.AppointmentLastSequenceNumber;
				this.OwnerCriticalChangeTime = calendarItem.OwnerCriticalChangeTime;
				this.AttendeeCriticalChangeTime = calendarItem.AttendeeCriticalChangeTime;
				this.CreationTime = calendarItem.CreationTime;
				this.LastModifiedTime = calendarItem.LastModifiedTime;
				this.Location = calendarItem.Location;
			}
		}

		private void ExtractDataFromItem(Item item)
		{
			if (this.GetPropertiesFromStoreObject(item))
			{
				this.IsOrganizer = false;
				this.GlobalObjectId = new GlobalObjectId(item.GetValueOrDefault<byte[]>(CalendarItemBaseSchema.GlobalObjectId));
				this.OwnerAppointmentId = item.GetValueAsNullable<int>(CalendarItemBaseSchema.OwnerAppointmentID);
				this.Subject = item.GetValueOrDefault<string>(ItemSchema.Subject, string.Empty);
				this.StartTime = item.GetValueOrDefault<ExDateTime>(CalendarItemInstanceSchema.StartTime, ExDateTime.MinValue);
				this.EndTime = item.GetValueOrDefault<ExDateTime>(CalendarItemInstanceSchema.EndTime, ExDateTime.MinValue);
				this.CalendarItemType = item.GetValueOrDefault<CalendarItemType>(CalendarItemBaseSchema.CalendarItemType, CalendarItemType.Single);
				this.SequenceNumber = item.GetValueOrDefault<int>(CalendarItemBaseSchema.AppointmentSequenceNumber);
				this.LastSequenceNumber = item.GetValueOrDefault<int>(CalendarItemBaseSchema.AppointmentLastSequenceNumber);
				this.OwnerCriticalChangeTime = item.GetValueOrDefault<ExDateTime>(CalendarItemBaseSchema.OwnerCriticalChangeTime, ExDateTime.MinValue);
				this.AttendeeCriticalChangeTime = item.GetValueOrDefault<ExDateTime>(CalendarItemBaseSchema.AttendeeCriticalChangeTime, ExDateTime.MinValue);
				this.CreationTime = item.GetValueOrDefault<ExDateTime>(StoreObjectSchema.CreationTime, ExDateTime.MinValue);
				this.LastModifiedTime = item.GetValueOrDefault<ExDateTime>(StoreObjectSchema.LastModifiedTime, ExDateTime.MinValue);
				this.Location = item.GetValueOrDefault<string>(CalendarItemBaseSchema.Location, string.Empty);
				this.ItemVersion = item.GetValueAsNullable<int>(CalendarItemBaseSchema.ItemVersion);
			}
		}

		private bool GetPropertiesFromStoreObject(StoreObject storeObject)
		{
			if (storeObject == null)
			{
				this.SetDefaultCalendarItemBasedProperties();
				return false;
			}
			this.Id = storeObject.Id;
			this.ExtractTime = storeObject.GetValueOrDefault<ExDateTime>(CalendarItemBaseSchema.AppointmentExtractTime, ExDateTime.MinValue);
			this.ExtractVersion = storeObject.GetValueOrDefault<long>(CalendarItemBaseSchema.AppointmentExtractVersion, long.MinValue);
			this.HasConflicts = storeObject.GetValueOrDefault<bool>(MessageItemSchema.MessageInConflict);
			this.InternetMessageId = storeObject.GetValueOrDefault<string>(ItemSchema.InternetMessageId, string.Empty);
			this.DocumentId = storeObject.GetValueOrDefault<int>(ItemSchema.DocumentId, int.MinValue);
			return true;
		}

		private void SetDefaultCalendarItemBasedProperties()
		{
			this.IsOrganizer = false;
			this.Id = null;
			this.GlobalObjectId = null;
			this.OwnerAppointmentId = null;
			this.Subject = null;
			this.StartTime = ExDateTime.MinValue;
			this.EndTime = ExDateTime.MinValue;
			this.CalendarItemType = CalendarItemType.Single;
			this.ExtractTime = ExDateTime.MinValue;
			this.ExtractVersion = 0L;
			this.HasConflicts = false;
			this.InternetMessageId = null;
			this.SequenceNumber = 0;
			this.LastSequenceNumber = 0;
			this.OwnerCriticalChangeTime = ExDateTime.MinValue;
			this.AttendeeCriticalChangeTime = ExDateTime.MinValue;
			this.CreationTime = ExDateTime.MinValue;
			this.LastModifiedTime = ExDateTime.MinValue;
			this.Location = string.Empty;
			this.DocumentId = int.MinValue;
		}

		private static readonly TimeSpan LastModifiedTimeTreshold = TimeSpan.FromSeconds(5.0);

		private string cleanGlobaObjectId;
	}
}
