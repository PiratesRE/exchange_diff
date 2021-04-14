﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class TokenIssuer
	{
		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string Uri;

		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string Endpoint;
	}
}
