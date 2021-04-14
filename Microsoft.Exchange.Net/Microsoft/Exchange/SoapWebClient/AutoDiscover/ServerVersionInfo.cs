using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlRoot(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover", IsNullable = true)]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class ServerVersionInfo : SoapHeader
	{
		public int MajorVersion;

		[XmlIgnore]
		public bool MajorVersionSpecified;

		public int MinorVersion;

		[XmlIgnore]
		public bool MinorVersionSpecified;

		public int MajorBuildNumber;

		[XmlIgnore]
		public bool MajorBuildNumberSpecified;

		public int MinorBuildNumber;

		[XmlIgnore]
		public bool MinorBuildNumberSpecified;

		[XmlElement(IsNullable = true)]
		public string Version;
	}
}
