using System;
using System.ServiceModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	[XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
	public class GetStreamingEventsSoapResponse : BaseSoapResponse, IXmlSerializable
	{
		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotSupportedException();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("soap11", "Header", "http://schemas.xmlsoap.org/soap/envelope/");
			GetStreamingEventsSoapResponse.serverVersionSerializer.Serialize(writer, this.ServerVersionInfo);
			writer.WriteEndElement();
			writer.WriteStartElement("soap11", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
			GetStreamingEventsSoapResponse.bodySerializer.Serialize(writer, this.Body);
			writer.WriteEndElement();
		}

		[MessageBodyMember(Name = "GetStreamingEventsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Order = 0)]
		[XmlElement("GetStreamingEventsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetStreamingEventsResponse Body;

		private static SafeXmlSerializer serverVersionSerializer = new SafeXmlSerializer(typeof(ServerVersionInfo));

		private static SafeXmlSerializer bodySerializer = new SafeXmlSerializer(typeof(GetStreamingEventsResponse));
	}
}
