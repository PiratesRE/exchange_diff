using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarSharingRecipient
	{
		[DataMember]
		public EmailAddressWrapper EmailAddress { get; set; }

		[DataMember]
		public string DetailLevel { get; set; }

		[DataMember]
		public bool ViewPrivateAppointments { get; set; }

		internal CalendarSharingDetailLevel DetailLevelType { get; private set; }

		internal SmtpAddress SmtpAddress { get; private set; }

		internal ADRecipient ADRecipient { get; set; }

		internal void ValidateRequest()
		{
			if (!string.IsNullOrEmpty(this.DetailLevel) && Enum.IsDefined(typeof(CalendarSharingDetailLevel), this.DetailLevel))
			{
				this.DetailLevelType = (CalendarSharingDetailLevel)Enum.Parse(typeof(CalendarSharingDetailLevel), this.DetailLevel);
				try
				{
					this.SmtpAddress = SmtpAddress.Parse(this.EmailAddress.EmailAddress);
				}
				catch (FormatException innerException)
				{
					throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(innerException), FaultParty.Sender);
				}
				return;
			}
			throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
		}
	}
}
