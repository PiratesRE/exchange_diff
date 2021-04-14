using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetMasterCategoryListRequest
	{
		[DataMember]
		public string EmailAddress { get; set; }

		internal SmtpAddress SmtpAddress { get; private set; }

		internal void ValidateRequest()
		{
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
		}
	}
}
