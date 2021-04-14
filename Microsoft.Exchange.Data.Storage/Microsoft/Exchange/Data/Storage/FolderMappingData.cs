using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	public class FolderMappingData
	{
		public string ShortId { get; set; }

		public string LongId { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute("FolderType")]
		public string DefaultFolderType { get; set; }

		public string Exception { get; set; }
	}
}
