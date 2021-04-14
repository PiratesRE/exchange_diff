using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public sealed class EncryptedSharedFolderData
	{
		[XmlElement]
		public EncryptedDataContainer Token { get; set; }

		[XmlElement]
		public EncryptedDataContainer Data { get; set; }
	}
}
