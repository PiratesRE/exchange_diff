using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class ServicePermanentExceptionWithXmlNodeArray : ServicePermanentException, IProvideXmlNodeArray
	{
		public ServicePermanentExceptionWithXmlNodeArray(Enum messageId) : base(messageId)
		{
		}

		public ServicePermanentExceptionWithXmlNodeArray(Enum messageId, Exception innerException) : base(messageId, innerException)
		{
		}

		protected XmlElement SerializeObjectToXml(object serializationObject, XmlSerializer serializer)
		{
			XmlElement documentElement;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream))
				{
					serializer.Serialize(xmlWriter, serializationObject);
				}
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (XmlReader xmlReader = SafeXmlFactory.CreateSafeXmlReader(memoryStream))
				{
					SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
					safeXmlDocument.Load(xmlReader);
					documentElement = safeXmlDocument.DocumentElement;
				}
			}
			return documentElement;
		}

		XmlNodeArray IProvideXmlNodeArray.NodeArray
		{
			get
			{
				return this.xmlNodeArray;
			}
		}

		internal XmlNodeArray NodeArray
		{
			get
			{
				return this.xmlNodeArray;
			}
		}

		private XmlNodeArray xmlNodeArray = new XmlNodeArray();
	}
}
