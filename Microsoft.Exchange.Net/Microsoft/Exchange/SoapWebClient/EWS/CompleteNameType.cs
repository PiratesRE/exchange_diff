using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class CompleteNameType
	{
		public string Title;

		public string FirstName;

		public string MiddleName;

		public string LastName;

		public string Suffix;

		public string Initials;

		public string FullName;

		public string Nickname;

		public string YomiFirstName;

		public string YomiLastName;
	}
}
