using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	internal sealed class MeetingValidationResultSchema : InMemoryObjectSchema
	{
		public static readonly SimplePropertyDefinition MeetingType = new SimplePropertyDefinition("MeetingType", typeof(string), string.Empty);

		public static readonly SimplePropertyDefinition ValidatingRole = new SimplePropertyDefinition("ValidatingRole", typeof(string), string.Empty);

		public static readonly SimplePropertyDefinition PrimarySmtpAddress = new SimplePropertyDefinition("PrimarySmtpAddress", typeof(SmtpAddress), null);

		public static readonly SimplePropertyDefinition IntervalStartDate = new SimplePropertyDefinition("IntervalStartDate", typeof(ExDateTime), null);

		public static readonly SimplePropertyDefinition IntervalEndDate = new SimplePropertyDefinition("IntervalEndDate", typeof(ExDateTime), null);

		public static readonly SimplePropertyDefinition StartTime = new SimplePropertyDefinition("StartTime", typeof(ExDateTime), null);

		public static readonly SimplePropertyDefinition EndTime = new SimplePropertyDefinition("EndTime", typeof(ExDateTime), null);

		public static readonly SimplePropertyDefinition ErrorDescription = new SimplePropertyDefinition("ErrorDescription", typeof(string), null);

		public static readonly SimplePropertyDefinition MeetingId = new SimplePropertyDefinition("MeetingId", typeof(string), null);

		public static readonly SimplePropertyDefinition GlobalObjectId = new SimplePropertyDefinition("GlobalObjectId", typeof(string), null);

		public static readonly SimplePropertyDefinition CleanGlobalObjectId = new SimplePropertyDefinition("CleanGlobalObjectId", typeof(string), null);

		public static readonly SimplePropertyDefinition CreationTime = new SimplePropertyDefinition("CreationTime", typeof(ExDateTime), null);

		public static readonly SimplePropertyDefinition LastModifiedTime = new SimplePropertyDefinition("LastModifiedTime", typeof(ExDateTime), null);

		public static readonly SimplePropertyDefinition Location = new SimplePropertyDefinition("Location", typeof(string), null);

		public static readonly SimplePropertyDefinition Subject = new SimplePropertyDefinition("Subject", typeof(string), null);

		public static readonly SimplePropertyDefinition Organizer = new SimplePropertyDefinition("Organizer", typeof(string), null);

		public static readonly SimplePropertyDefinition IsConsistent = new SimplePropertyDefinition("IsConsistent", typeof(bool), null);

		public static readonly SimplePropertyDefinition DuplicatesDetected = new SimplePropertyDefinition("DuplicatesDetected", typeof(bool), null);

		public static readonly SimplePropertyDefinition HasConflicts = new SimplePropertyDefinition("HasConflicts", typeof(bool), null);

		public static readonly SimplePropertyDefinition ExtractVersion = new SimplePropertyDefinition("ExtractVersion", typeof(long), null);

		public static readonly SimplePropertyDefinition ExtractTime = new SimplePropertyDefinition("ExtractTime", typeof(ExDateTime), null);

		public static readonly SimplePropertyDefinition NumDelegates = new SimplePropertyDefinition("NumDelegates", typeof(int), null);

		public static readonly SimplePropertyDefinition InternetMessageId = new SimplePropertyDefinition("InternetMessageId", typeof(string), null);

		public static readonly SimplePropertyDefinition SequenceNumber = new SimplePropertyDefinition("SequenceNumber", typeof(int), null);

		public static readonly SimplePropertyDefinition OwnerApptId = new SimplePropertyDefinition("OwnerApptId", typeof(int), null);

		public static readonly SimplePropertyDefinition OwnerCriticalChangeTime = new SimplePropertyDefinition("OwnerCriticalChangeTime", typeof(ExDateTime), null);

		public static readonly SimplePropertyDefinition AttendeeCriticalChangeTime = new SimplePropertyDefinition("AttendeeCriticalChangeTime", typeof(ExDateTime), null);

		public static readonly SimplePropertyDefinition WasValidationSuccessful = new SimplePropertyDefinition("WasValidationSuccessful", typeof(bool), null);
	}
}
