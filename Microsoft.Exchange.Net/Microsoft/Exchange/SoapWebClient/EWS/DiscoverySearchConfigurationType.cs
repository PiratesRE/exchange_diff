using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class DiscoverySearchConfigurationType
	{
		public string SearchId;

		public string SearchQuery;

		[XmlArrayItem("SearchableMailbox", IsNullable = false)]
		public SearchableMailboxType[] SearchableMailboxes;

		public string InPlaceHoldIdentity;

		public string ManagedByOrganization;

		public string Language;
	}
}
