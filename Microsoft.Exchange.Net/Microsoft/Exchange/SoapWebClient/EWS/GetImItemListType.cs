﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetImItemListType : BaseRequestType
	{
		[XmlArrayItem("ExtendedProperty", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public PathToExtendedFieldType[] ExtendedProperties;
	}
}