using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public sealed class EncryptedDataContainer
	{
		[XmlAnyElement]
		public XmlElement EncryptedData { get; set; }
	}
}
