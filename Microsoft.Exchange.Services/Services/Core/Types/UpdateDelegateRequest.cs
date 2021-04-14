using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UpdateDelegateType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class UpdateDelegateRequest : BaseDelegateRequest
	{
		[XmlArrayItem("DelegateUser", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public DelegateUserType[] DelegateUsers
		{
			get
			{
				return this.delegateUsers;
			}
			set
			{
				this.delegateUsers = value;
			}
		}

		[XmlElement("DeliverMeetingRequests")]
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

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new UpdateDelegate(callContext, this);
		}

		public UpdateDelegateRequest() : base(true)
		{
		}

		private DelegateUserType[] delegateUsers;

		private DeliverMeetingRequestsType deliverMeetingRequest;
	}
}
