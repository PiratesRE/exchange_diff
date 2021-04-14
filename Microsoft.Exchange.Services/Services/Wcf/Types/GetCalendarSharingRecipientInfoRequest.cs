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
	public class GetCalendarSharingRecipientInfoRequest
	{
		[DataMember]
		public string[] EmailAddresses { get; set; }

		[DataMember]
		public ItemId CalendarId { get; set; }

		internal StoreObjectId CalendarStoreId { get; private set; }

		internal List<KeyValuePair<SmtpAddress, ADRecipient>> Recipients { get; private set; }

		internal void ValidateRequest(ADRecipientSessionContext adRecipientSessionContext)
		{
			if (this.EmailAddresses == null || this.EmailAddresses.Length == 0)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			List<SmtpAddress> list = new List<SmtpAddress>(this.EmailAddresses.Length);
			try
			{
				Array.ForEach<string>(this.EmailAddresses, delegate(string email)
				{
					list.Add(SmtpAddress.Parse(email));
				});
			}
			catch (FormatException innerException)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(innerException), FaultParty.Sender);
			}
			if (this.CalendarId != null && this.CalendarId.Id != null)
			{
				this.CalendarStoreId = ServiceIdConverter.ConvertFromConcatenatedId(this.CalendarId.Id, BasicTypes.Folder, null).ToStoreObjectId();
			}
			this.PopulateADRecipients(list, adRecipientSessionContext);
		}

		private void PopulateADRecipients(List<SmtpAddress> smtpList, ADRecipientSessionContext adRecipientSessionContext)
		{
			IRecipientSession adrecipientSession = adRecipientSessionContext.GetADRecipientSession();
			List<ProxyAddress> paddresses = new List<ProxyAddress>();
			smtpList.ForEach(delegate(SmtpAddress smtp)
			{
				paddresses.Add(ProxyAddress.Parse(smtp.ToString()));
			});
			Result<ADRecipient>[] array = adrecipientSession.FindByProxyAddresses(paddresses.ToArray());
			List<KeyValuePair<SmtpAddress, ADRecipient>> list = new List<KeyValuePair<SmtpAddress, ADRecipient>>();
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(new KeyValuePair<SmtpAddress, ADRecipient>(smtpList[i], array[i].Data));
			}
			this.Recipients = list;
		}
	}
}
