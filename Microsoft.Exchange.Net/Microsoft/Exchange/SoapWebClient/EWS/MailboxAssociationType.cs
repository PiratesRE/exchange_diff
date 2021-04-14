using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class MailboxAssociationType
	{
		public GroupLocatorType Group;

		public UserLocatorType User;

		public bool IsMember;

		[XmlIgnore]
		public bool IsMemberSpecified;

		public DateTime JoinDate;

		[XmlIgnore]
		public bool JoinDateSpecified;

		public bool IsPin;

		[XmlIgnore]
		public bool IsPinSpecified;

		public string JoinedBy;
	}
}
