﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class UserSettingError
	{
		public ErrorCode ErrorCode;

		[XmlElement(IsNullable = true)]
		public string ErrorMessage;

		[XmlElement(IsNullable = true)]
		public string SettingName;
	}
}
