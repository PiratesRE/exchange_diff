using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "GetDelegateResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetDelegateResponseMessage : DelegateResponseMessage
	{
		public GetDelegateResponseMessage()
		{
		}

		internal GetDelegateResponseMessage(ServiceResultCode code, ServiceError error, DelegateUserResponseMessageType[] delegateUsers, DeliverMeetingRequestsType deliverMeetingRequest) : base(code, error, delegateUsers, ResponseType.GetDelegateResponseMessage)
		{
			this.deliverMeetingRequest = deliverMeetingRequest;
		}

		[XmlElement("DeliverMeetingRequests")]
		[DefaultValue(DeliverMeetingRequestsType.None)]
		public DeliverMeetingRequestsType DeliverMeetingRequests
		{
			get
			{
				return this.deliverMeetingRequest;
			}
			set
			{
				this.deliverMeetingRequest = value;
			}
		}

		private DeliverMeetingRequestsType deliverMeetingRequest;
	}
}
