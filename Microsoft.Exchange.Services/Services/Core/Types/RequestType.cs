using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum RequestType
	{
		None,
		NewMeetingRequest,
		FullUpdate = 65536,
		InformationalUpdate = 131072,
		SilentUpdate = 262144,
		Outdated = 524288,
		PrincipalWantsCopy = 1048576
	}
}
