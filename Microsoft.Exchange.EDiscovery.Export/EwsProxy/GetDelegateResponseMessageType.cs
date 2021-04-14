using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetDelegateResponseMessageType : BaseDelegateResponseMessageType
	{
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

		private DeliverMeetingRequestsType deliverMeetingRequestsField;

		private bool deliverMeetingRequestsFieldSpecified;
	}
}
