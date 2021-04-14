using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[XmlRoot(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ServerVersionInfo : SoapHeader
	{
		[XmlAttribute]
		public int MajorVersion;

		[XmlIgnore]
		public bool MajorVersionSpecified;

		[XmlAttribute]
		public int MinorVersion;

		[XmlIgnore]
		public bool MinorVersionSpecified;

		[XmlAttribute]
		public int MajorBuildNumber;

		[XmlIgnore]
		public bool MajorBuildNumberSpecified;

		[XmlAttribute]
		public int MinorBuildNumber;

		[XmlIgnore]
		public bool MinorBuildNumberSpecified;

		[XmlAttribute]
		public string Version;
	}
}
