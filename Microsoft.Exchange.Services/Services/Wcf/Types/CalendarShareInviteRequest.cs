using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarShareInviteRequest
	{
		[DataMember]
		public ItemId CalendarId { get; set; }

		[DataMember]
		public string Subject { get; set; }

		[DataMember]
		public BodyContentType Body { get; set; }

		[DataMember]
		public CalendarSharingRecipient[] SharingRecipients { get; set; }

		internal StoreObjectId CalendarStoreId { get; private set; }

		internal string CalendarName { get; private set; }

		internal void ValidateRequest(MailboxSession session, ADRecipientSessionContext adRecipientSessionContext)
		{
			if (this.CalendarId == null || string.IsNullOrEmpty(this.CalendarId.Id) || this.Body == null || this.Subject == null || this.SharingRecipients == null || this.SharingRecipients.Length == 0 || this.SharingRecipients.Length > 50)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			Array.ForEach<CalendarSharingRecipient>(this.SharingRecipients, delegate(CalendarSharingRecipient action)
			{
				action.ValidateRequest();
			});
			this.CalendarStoreId = ServiceIdConverter.ConvertFromConcatenatedId(this.CalendarId.Id, BasicTypes.Folder, null).ToStoreObjectId();
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(session, this.CalendarStoreId))
			{
				this.CalendarStoreId = calendarFolder.StoreObjectId;
				this.CalendarName = calendarFolder.DisplayName;
			}
			this.PopulateADRecipients(adRecipientSessionContext);
		}

		private void PopulateADRecipients(ADRecipientSessionContext adRecipientSessionContext)
		{
			IRecipientSession adrecipientSession = adRecipientSessionContext.GetADRecipientSession();
			List<ProxyAddress> paddresses = new List<ProxyAddress>(this.SharingRecipients.Length);
			Array.ForEach<CalendarSharingRecipient>(this.SharingRecipients, delegate(CalendarSharingRecipient rep)
			{
				paddresses.Add(ProxyAddress.Parse(rep.EmailAddress.EmailAddress));
			});
			Result<ADRecipient>[] array = adrecipientSession.FindByProxyAddresses(paddresses.ToArray());
			for (int i = 0; i < array.Length; i++)
			{
				this.SharingRecipients[i].ADRecipient = array[i].Data;
			}
		}

		private const int MaxNumberOfRecipientsAllowed = 50;
	}
}
