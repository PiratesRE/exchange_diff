using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class ResponseMessage
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

		public string ResponseCode
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

		public string DescriptiveLink
		{
			get
			{
				return this.descriptiveLinkField;
			}
			set
			{
				this.descriptiveLinkField = value;
			}
		}

		[XmlAnyElement]
		public XmlNode MessageXml
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

		public Path Path
		{
			get
			{
				return this.pathField;
			}
			set
			{
				this.pathField = value;
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

		public const string ExceptionTypeNodeName = "ExceptionType";

		public const string ExceptionCodeNodeName = "ExceptionCode";

		public const string ExceptionServerNodeName = "ExceptionServerName";

		public const string ExceptionMessageNodeName = "ExceptionMessage";

		private string messageTextField;

		private string responseCodeField;

		private string descriptiveLinkField;

		private XmlNode messageXmlField;

		private Path pathField;

		private ResponseClassType responseClassField;
	}
}
