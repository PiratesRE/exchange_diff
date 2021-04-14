using System;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal sealed class Timestamp
	{
		public Timestamp(string id, DateTime created, DateTime expires)
		{
			this.id = id;
			this.created = created;
			this.expires = expires;
		}

		public DateTime Created
		{
			get
			{
				return this.created;
			}
		}

		public DateTime Expires
		{
			get
			{
				return this.expires;
			}
		}

		public static Timestamp Parse(XmlElement timestampXml)
		{
			return new Timestamp(null, Timestamp.GetDateTimeElement(timestampXml, WSSecurityUtility.Created), Timestamp.GetDateTimeElement(timestampXml, WSSecurityUtility.Expires));
		}

		public XmlElement GetXml(XmlDocument xmlDocument)
		{
			XmlAttribute xmlAttribute = WSSecurityUtility.Id.CreateAttribute(xmlDocument, this.id);
			XmlElement xmlElement = WSSecurityUtility.Created.CreateElement(xmlDocument, Timestamp.FormatDateTimeForSoap(this.created));
			XmlElement xmlElement2 = WSSecurityUtility.Expires.CreateElement(xmlDocument, Timestamp.FormatDateTimeForSoap(this.expires));
			return WSSecurityUtility.Timestamp.CreateElement(xmlDocument, new XmlAttribute[]
			{
				xmlAttribute
			}, new XmlElement[]
			{
				xmlElement,
				xmlElement2
			});
		}

		private static DateTime GetDateTimeElement(XmlElement parent, XmlElementDefinition elementDefinition)
		{
			XmlElement singleElementByName = elementDefinition.GetSingleElementByName(parent);
			if (singleElementByName == null)
			{
				Timestamp.Tracer.TraceError<XmlElementDefinition, string>(0L, "Failed to find XML element: {0} in this context {1}", elementDefinition, parent.OuterXml);
				throw new SoapXmlMalformedException(parent, elementDefinition);
			}
			DateTime result;
			if (!DateTime.TryParse(singleElementByName.InnerText, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
			{
				throw new SoapXmlMalformedException(parent, elementDefinition);
			}
			return result;
		}

		private static string FormatDateTimeForSoap(DateTime dateTime)
		{
			return dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture);
		}

		private static readonly Trace Tracer = ExTraceGlobals.WSTrustTracer;

		private string id;

		private DateTime created;

		private DateTime expires;
	}
}
