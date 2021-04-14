using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SubscribeInternalCalendarRequest
	{
		[DataMember]
		public string EmailAddress { get; set; }

		[DataMember]
		private string ParentGroupGuid { get; set; }

		internal ADRecipient Recipient { get; private set; }

		internal SmtpAddress SmtpAddress { get; private set; }

		internal Guid GroupId { get; private set; }

		internal void ValidateRequest()
		{
			Guid groupId;
			if (!Guid.TryParse(this.ParentGroupGuid, out groupId))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(CoreResources.IDs.ErrorInvalidId), FaultParty.Sender);
			}
			this.GroupId = groupId;
			if (this.EmailAddress == null)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(CoreResources.IDs.ErrorInvalidSmtpAddress), FaultParty.Sender);
			}
			try
			{
				this.SmtpAddress = SmtpAddress.Parse(this.EmailAddress);
			}
			catch (FormatException innerException)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(CoreResources.IDs.ErrorInvalidSmtpAddress, innerException), FaultParty.Sender);
			}
			this.RetrieveADRecipient();
		}

		private void RetrieveADRecipient()
		{
			IRecipientSession adrecipientSession = CallContext.Current.ADRecipientSessionContext.GetADRecipientSession();
			this.Recipient = adrecipientSession.FindByProxyAddress(ProxyAddress.Parse(this.SmtpAddress.ToString()));
		}
	}
}
