using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DeliverMeetingRequestsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum DeliverMeetingRequestsType
	{
		None,
		DelegatesOnly,
		DelegatesAndMe,
		DelegatesAndSendInformationToMe,
		NoForward
	}
}
