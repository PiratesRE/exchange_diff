using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class DelegateUserResponseMessageType : ResponseMessageType
	{
		public DelegateUserType DelegateUser
		{
			get
			{
				return this.delegateUserField;
			}
			set
			{
				this.delegateUserField = value;
			}
		}

		private DelegateUserType delegateUserField;
	}
}
