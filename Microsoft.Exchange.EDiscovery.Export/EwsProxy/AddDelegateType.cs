using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class AddDelegateType : BaseDelegateType
	{
		[XmlArrayItem("DelegateUser", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public DelegateUserType[] DelegateUsers
		{
			get
			{
				return this.delegateUsersField;
			}
			set
			{
				this.delegateUsersField = value;
			}
		}

		public DeliverMeetingRequestsType DeliverMeetingRequests
		{
			get
			{
				return this.deliverMeetingRequestsField;
			}
			set
			{
				this.deliverMeetingRequestsField = value;
			}
		}

		[XmlIgnore]
		public bool DeliverMeetingRequestsSpecified
		{
			get
			{
				return this.deliverMeetingRequestsFieldSpecified;
			}
			set
			{
				this.deliverMeetingRequestsFieldSpecified = value;
			}
		}

		private DelegateUserType[] delegateUsersField;

		private DeliverMeetingRequestsType deliverMeetingRequestsField;

		private bool deliverMeetingRequestsFieldSpecified;
	}
}
