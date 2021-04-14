using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlInclude(typeof(GetDelegateResponseMessageType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(UpdateDelegateResponseMessageType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(AddDelegateResponseMessageType))]
	[XmlInclude(typeof(RemoveDelegateResponseMessageType))]
	[Serializable]
	public abstract class BaseDelegateResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem(IsNullable = false)]
		public DelegateUserResponseMessageType[] ResponseMessages
		{
			get
			{
				return this.responseMessagesField;
			}
			set
			{
				this.responseMessagesField = value;
			}
		}

		private DelegateUserResponseMessageType[] responseMessagesField;
	}
}
