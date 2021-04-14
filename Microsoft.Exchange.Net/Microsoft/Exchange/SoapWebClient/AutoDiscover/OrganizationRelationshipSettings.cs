using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class OrganizationRelationshipSettings
	{
		public bool DeliveryReportEnabled;

		[XmlArrayItem("Domain")]
		[XmlArray(IsNullable = true)]
		public string[] DomainNames;

		public bool FreeBusyAccessEnabled;

		[XmlElement(IsNullable = true)]
		public string FreeBusyAccessLevel;

		public bool MailTipsAccessEnabled;

		[XmlElement(IsNullable = true)]
		public string MailTipsAccessLevel;

		public bool MailboxMoveEnabled;

		[XmlElement(IsNullable = true)]
		public string Name;

		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string TargetApplicationUri;

		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string TargetAutodiscoverEpr;

		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string TargetSharingEpr;
	}
}
