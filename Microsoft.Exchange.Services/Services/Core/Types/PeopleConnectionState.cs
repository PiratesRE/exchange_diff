using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "PeopleConnectionStateType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum PeopleConnectionState
	{
		Disconnected,
		Connected,
		ConnectedNeedsToken
	}
}
