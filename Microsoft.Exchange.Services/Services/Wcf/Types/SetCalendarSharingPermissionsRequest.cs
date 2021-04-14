using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetCalendarSharingPermissionsRequest : CalendarSharingRequestBase
	{
		[DataMember]
		public CalendarSharingPermissionInfo[] Recipients
		{
			get
			{
				return this.recipients;
			}
			set
			{
				this.recipients = value;
				if (value != null)
				{
					this.CreateRecipientIndexStringDictionary();
				}
			}
		}

		[DataMember]
		public DeliverMeetingRequestsType SelectedDeliveryOption { get; set; }

		internal CalendarSharingPermissionInfo PublishedCalendarPermissions { get; private set; }

		private Dictionary<string, CalendarSharingPermissionInfo> RecipientGivenIndexStringDictionary
		{
			get
			{
				return this.recipientGivenIndexStringDictionary;
			}
		}

		internal override void ValidateRequest()
		{
			base.ValidateRequest();
			this.recipients = (this.recipients ?? new CalendarSharingPermissionInfo[0]);
			foreach (CalendarSharingPermissionInfo calendarSharingPermissionInfo in this.Recipients)
			{
				if (calendarSharingPermissionInfo.FromPermissionsTable && string.IsNullOrEmpty(calendarSharingPermissionInfo.ID))
				{
					throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
				}
				if (string.IsNullOrEmpty(calendarSharingPermissionInfo.CurrentDetailLevel))
				{
					throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
				}
				CalendarSharingDetailLevel calendarSharingDetailLevel;
				if (!Enum.TryParse<CalendarSharingDetailLevel>(calendarSharingPermissionInfo.CurrentDetailLevel, out calendarSharingDetailLevel))
				{
					throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
				}
			}
		}

		internal CalendarSharingPermissionInfo RecipientGivenIndexString(string indexString)
		{
			CalendarSharingPermissionInfo result = null;
			if (indexString != null)
			{
				this.RecipientGivenIndexStringDictionary.TryGetValue(indexString, out result);
			}
			return result;
		}

		private void CreateRecipientIndexStringDictionary()
		{
			this.recipientGivenIndexStringDictionary = new Dictionary<string, CalendarSharingPermissionInfo>(this.Recipients.Length);
			this.recipients = (this.recipients ?? new CalendarSharingPermissionInfo[0]);
			foreach (CalendarSharingPermissionInfo calendarSharingPermissionInfo in this.Recipients)
			{
				if (!calendarSharingPermissionInfo.FromPermissionsTable)
				{
					this.PublishedCalendarPermissions = calendarSharingPermissionInfo;
				}
				else
				{
					this.recipientGivenIndexStringDictionary[calendarSharingPermissionInfo.ID] = calendarSharingPermissionInfo;
				}
			}
		}

		private Dictionary<string, CalendarSharingPermissionInfo> recipientGivenIndexStringDictionary;

		private CalendarSharingPermissionInfo[] recipients;
	}
}
