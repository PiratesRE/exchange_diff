using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum FlaggedForActionType
	{
		Any,
		Call,
		DoNotForward,
		FollowUp,
		FYI,
		Forward,
		NoResponseNecessary,
		Read,
		Reply,
		ReplyToAll,
		Review
	}
}
