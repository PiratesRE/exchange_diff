using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetBirthdayCalendarViewResponseMessage : ResponseMessage
	{
		public GetBirthdayCalendarViewResponseMessage()
		{
		}

		internal GetBirthdayCalendarViewResponseMessage(ServiceResultCode code, ServiceError error, GetBirthdayCalendarViewResponseMessage response) : base(code, error)
		{
			this.BirthdayEvents = null;
			if (response != null)
			{
				this.BirthdayEvents = response.BirthdayEvents;
			}
		}

		[DataMember]
		public BirthdayEvent[] BirthdayEvents { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetBirthdayCalendarViewResponseMessage;
		}
	}
}
