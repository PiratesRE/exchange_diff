using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(GetMailTipsResponseMessageType))]
	[XmlInclude(typeof(MailTipsResponseMessageType))]
	[DebuggerStepThrough]
	[Serializable]
	public class ResponseMessageType
	{
		public string MessageText
		{
			get
			{
				return this.messageTextField;
			}
			set
			{
				this.messageTextField = value;
			}
		}

		public ResponseCodeType ResponseCode
		{
			get
			{
				return this.responseCodeField;
			}
			set
			{
				this.responseCodeField = value;
			}
		}

		[XmlIgnore]
		public bool ResponseCodeSpecified
		{
			get
			{
				return this.responseCodeFieldSpecified;
			}
			set
			{
				this.responseCodeFieldSpecified = value;
			}
		}

		public int DescriptiveLinkKey
		{
			get
			{
				return this.descriptiveLinkKeyField;
			}
			set
			{
				this.descriptiveLinkKeyField = value;
			}
		}

		[XmlIgnore]
		public bool DescriptiveLinkKeySpecified
		{
			get
			{
				return this.descriptiveLinkKeyFieldSpecified;
			}
			set
			{
				this.descriptiveLinkKeyFieldSpecified = value;
			}
		}

		public ResponseMessageTypeMessageXml MessageXml
		{
			get
			{
				return this.messageXmlField;
			}
			set
			{
				this.messageXmlField = value;
			}
		}

		[XmlAttribute]
		public ResponseClassType ResponseClass
		{
			get
			{
				return this.responseClassField;
			}
			set
			{
				this.responseClassField = value;
			}
		}

		private string messageTextField;

		private ResponseCodeType responseCodeField;

		private bool responseCodeFieldSpecified;

		private int descriptiveLinkKeyField;

		private bool descriptiveLinkKeyFieldSpecified;

		private ResponseMessageTypeMessageXml messageXmlField;

		private ResponseClassType responseClassField;
	}
}
