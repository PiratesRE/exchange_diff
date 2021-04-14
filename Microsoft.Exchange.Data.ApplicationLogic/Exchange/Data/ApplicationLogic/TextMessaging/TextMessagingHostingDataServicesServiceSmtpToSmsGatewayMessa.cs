using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging
{
	[GeneratedCode("xsd", "4.0.30319.17627")]
	[XmlType(AnonymousType = true)]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRendering
	{
		[XmlElement("Capacity", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
		public TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity[] Capacity
		{
			get
			{
				return this.capacityField;
			}
			set
			{
				this.capacityField = value;
			}
		}

		[XmlAttribute]
		public TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer Container
		{
			get
			{
				return this.containerField;
			}
			set
			{
				this.containerField = value;
			}
		}

		[XmlIgnore]
		public bool ContainerSpecified
		{
			get
			{
				return this.containerFieldSpecified;
			}
			set
			{
				this.containerFieldSpecified = value;
			}
		}

		private TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity[] capacityField;

		private TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer containerField;

		private bool containerFieldSpecified;
	}
}
