using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Data.Storage
{
	[XmlRoot(ElementName = "SharedFolderData", Namespace = "http://schemas.microsoft.com/exchange/sharing/2008")]
	[Serializable]
	public sealed class SharedFolderData
	{
		[XmlElement]
		public string DataType { get; set; }

		[XmlElement]
		public string SharingUrl { get; set; }

		[XmlElement]
		public string FederationUri { get; set; }

		[XmlElement]
		public string FolderId { get; set; }

		[XmlElement]
		public string SenderSmtpAddress { get; set; }

		[XmlArrayItem("Recipient")]
		[XmlArray("Recipients")]
		public SharedFolderDataRecipient[] Recipients { get; set; }

		public static SharedFolderData DeserializeFromXmlELement(XmlElement xmlElement)
		{
			new SharedFolderData();
			XmlNodeReader reader = new XmlNodeReader(xmlElement);
			SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(SharedFolderData));
			return safeXmlSerializer.Deserialize(reader) as SharedFolderData;
		}

		public XmlElement SerializeToXmlElement()
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(SharedFolderData));
				safeXmlSerializer.Serialize(memoryStream, this);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				xmlDocument.Load(memoryStream);
			}
			return xmlDocument.DocumentElement;
		}
	}
}
