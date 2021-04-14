using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	internal static class BulkUserProvisioningCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (BulkUserProvisioningCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in BulkUserProvisioningCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchange Bulk User Provisioning";

		public static readonly ExPerformanceCounter NumberOfRecipientsAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Recipients Attempted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfRecipientsAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Recipients Attempted Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfRecipientsCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Recipients Created", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfRecipientsCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Recipients Created Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfRecipientsFailed = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Recipients Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMailboxesAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Mailboxes Attempted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfMailboxesAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Mailboxes Attempted Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMailboxesCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Mailboxes Created", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfMailboxesCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Mailboxes Created Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMailboxesFailed = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Mailboxes Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfContactsAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Contacts Attempted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfContactsAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Contacts Attempted Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfContactsCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Contacts Created", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfContactsCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Contacts Created Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfContactsFailed = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Contacts Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfGroupsAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Groups Attempted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfGroupsAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Groups Attempted Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfGroupsCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Groups Created", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfGroupsCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Groups Created Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfGroupsFailed = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Groups Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMembersAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Members Attempted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfMembersAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Members Attempted Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMembersCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Members Created", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfMembersCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Members Created Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMembersFailed = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Members Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfUpdateContactsAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total UpdateContacts Attempted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfUpdateContactsAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "UpdateContacts Attempted Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfUpdateContactsCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total UpdateContacts Created", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfUpdateContactsCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "UpdateContacts Created Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfUpdateContactsFailed = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total UpdateContacts Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfUpdateUserAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total UpdateUser Attempted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfUpdateUserAttempted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "UpdateUser Attempted Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfUpdateUserCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total UpdateUser Created", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RateOfUpdateUserCreated = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "UpdateUser Created Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfUpdateUserFailed = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total UpdateUser Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfRequestsInQueue = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Requests Loaded For Processing", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfRequestsCompleted = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Requests Completed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfRequestsWithTransientError = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Total Requests With Transient Error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfRequestsWithoutProgressInThisRound = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Requests Without Progress", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfRequestsWithProgressInThisRound = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Requests With Progress", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfRoundsWithRequestsWithoutProgress = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Rounds With Requests Without Progress", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfRoundsWithoutProgress = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Rounds Without Progress", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastRecipientAttemptedTimestamp = new ExPerformanceCounter("MSExchange Bulk User Provisioning", "Last Attempted Recipient Timestamp", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			BulkUserProvisioningCounters.NumberOfRecipientsAttempted,
			BulkUserProvisioningCounters.RateOfRecipientsAttempted,
			BulkUserProvisioningCounters.NumberOfRecipientsCreated,
			BulkUserProvisioningCounters.RateOfRecipientsCreated,
			BulkUserProvisioningCounters.NumberOfRecipientsFailed,
			BulkUserProvisioningCounters.NumberOfMailboxesAttempted,
			BulkUserProvisioningCounters.RateOfMailboxesAttempted,
			BulkUserProvisioningCounters.NumberOfMailboxesCreated,
			BulkUserProvisioningCounters.RateOfMailboxesCreated,
			BulkUserProvisioningCounters.NumberOfMailboxesFailed,
			BulkUserProvisioningCounters.NumberOfContactsAttempted,
			BulkUserProvisioningCounters.RateOfContactsAttempted,
			BulkUserProvisioningCounters.NumberOfContactsCreated,
			BulkUserProvisioningCounters.RateOfContactsCreated,
			BulkUserProvisioningCounters.NumberOfContactsFailed,
			BulkUserProvisioningCounters.NumberOfGroupsAttempted,
			BulkUserProvisioningCounters.RateOfGroupsAttempted,
			BulkUserProvisioningCounters.NumberOfGroupsCreated,
			BulkUserProvisioningCounters.RateOfGroupsCreated,
			BulkUserProvisioningCounters.NumberOfGroupsFailed,
			BulkUserProvisioningCounters.NumberOfMembersAttempted,
			BulkUserProvisioningCounters.RateOfMembersAttempted,
			BulkUserProvisioningCounters.NumberOfMembersCreated,
			BulkUserProvisioningCounters.RateOfMembersCreated,
			BulkUserProvisioningCounters.NumberOfMembersFailed,
			BulkUserProvisioningCounters.NumberOfUpdateContactsAttempted,
			BulkUserProvisioningCounters.RateOfUpdateContactsAttempted,
			BulkUserProvisioningCounters.NumberOfUpdateContactsCreated,
			BulkUserProvisioningCounters.RateOfUpdateContactsCreated,
			BulkUserProvisioningCounters.NumberOfUpdateContactsFailed,
			BulkUserProvisioningCounters.NumberOfUpdateUserAttempted,
			BulkUserProvisioningCounters.RateOfUpdateUserAttempted,
			BulkUserProvisioningCounters.NumberOfUpdateUserCreated,
			BulkUserProvisioningCounters.RateOfUpdateUserCreated,
			BulkUserProvisioningCounters.NumberOfUpdateUserFailed,
			BulkUserProvisioningCounters.NumberOfRequestsInQueue,
			BulkUserProvisioningCounters.NumberOfRequestsCompleted,
			BulkUserProvisioningCounters.NumberOfRequestsWithTransientError,
			BulkUserProvisioningCounters.NumberOfRequestsWithoutProgressInThisRound,
			BulkUserProvisioningCounters.NumberOfRequestsWithProgressInThisRound,
			BulkUserProvisioningCounters.NumberOfRoundsWithRequestsWithoutProgress,
			BulkUserProvisioningCounters.NumberOfRoundsWithoutProgress,
			BulkUserProvisioningCounters.LastRecipientAttemptedTimestamp
		};
	}
}
