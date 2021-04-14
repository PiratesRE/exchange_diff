using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging
{
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[DebuggerStepThrough]
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[Serializable]
	public class TextMessagingHostingDataServices
	{
		[XmlElement("Service", Form = XmlSchemaForm.Unqualified)]
		public TextMessagingHostingDataServicesService[] Service
		{
			get
			{
				return this.serviceField;
			}
			set
			{
				this.serviceField = value;
			}
		}

		private TextMessagingHostingDataServicesService[] serviceField;
	}
}
