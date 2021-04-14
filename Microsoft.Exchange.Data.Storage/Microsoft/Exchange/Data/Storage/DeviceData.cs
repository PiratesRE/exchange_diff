using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	public class DeviceData
	{
		[XmlIgnore]
		internal StoreObjectId FolderId { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public DateTime Created { get; set; }

		public List<SyncStateFolderData> SyncFolders { get; set; }

		[XmlIgnore]
		public bool ShouldAdd { get; set; }
	}
}
