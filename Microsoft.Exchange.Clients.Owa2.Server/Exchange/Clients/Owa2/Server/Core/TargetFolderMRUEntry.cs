using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[SimpleConfiguration("TargetFolderMRU", "TargetFolderMRU")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class TargetFolderMRUEntry
	{
		public TargetFolderMRUEntry()
		{
		}

		public TargetFolderMRUEntry(OwaStoreObjectId owaStoreObjectIdFolderId)
		{
			this.owaStoreObjectIdFolderId = owaStoreObjectIdFolderId.ToString();
		}

		[SimpleConfigurationProperty("folderId")]
		public string FolderId
		{
			get
			{
				return this.owaStoreObjectIdFolderId;
			}
			set
			{
				this.owaStoreObjectIdFolderId = value;
			}
		}

		[SimpleConfigurationProperty("ewsFolderId")]
		[DataMember]
		public string EwsFolderIdEntry
		{
			get
			{
				return this.ewsFolderIdEntry;
			}
			set
			{
				this.ewsFolderIdEntry = value;
			}
		}

		private string owaStoreObjectIdFolderId;

		private string ewsFolderIdEntry;
	}
}
