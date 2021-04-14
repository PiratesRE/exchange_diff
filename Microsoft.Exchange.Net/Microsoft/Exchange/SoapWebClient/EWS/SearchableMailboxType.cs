using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SearchableMailboxType
	{
		public string Guid;

		public string PrimarySmtpAddress;

		public bool IsExternalMailbox;

		public string ExternalEmailAddress;

		public string DisplayName;

		public bool IsMembershipGroup;

		public string ReferenceId;
	}
}
