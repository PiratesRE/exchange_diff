using System;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.SoapWebClient
{
	internal static class Constants
	{
		public const string MessagesNamespace = "http://schemas.microsoft.com/exchange/services/2006/messages";

		public const string TypesNamespace = "http://schemas.microsoft.com/exchange/services/2006/types";

		public static readonly XmlNamespaceDefinition[] EwsNamespaces = new XmlNamespaceDefinition[]
		{
			new XmlNamespaceDefinition("exm", "http://schemas.microsoft.com/exchange/services/2006/messages"),
			new XmlNamespaceDefinition("ext", "http://schemas.microsoft.com/exchange/services/2006/types")
		};
	}
}
