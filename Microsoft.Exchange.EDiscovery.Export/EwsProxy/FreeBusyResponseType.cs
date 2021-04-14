using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class FreeBusyResponseType
	{
		public ResponseMessageType ResponseMessage
		{
			get
			{
				return this.responseMessageField;
			}
			set
			{
				this.responseMessageField = value;
			}
		}

		public FreeBusyView FreeBusyView
		{
			get
			{
				return this.freeBusyViewField;
			}
			set
			{
				this.freeBusyViewField = value;
			}
		}

		private ResponseMessageType responseMessageField;

		private FreeBusyView freeBusyViewField;
	}
}
