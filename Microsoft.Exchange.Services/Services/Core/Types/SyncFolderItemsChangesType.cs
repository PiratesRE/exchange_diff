using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SyncFolderItemsChangesType
	{
		public SyncFolderItemsChangesType()
		{
			this.changes = new List<SyncFolderItemsChangeTypeBase>();
			this.changesElementName = new List<SyncFolderItemsChangesEnum>();
		}

		public void AddChange(SyncFolderItemsChangeTypeBase change)
		{
			this.changes.Add(change);
			this.changesElementName.Add(change.ChangeType);
		}

		public int Count
		{
			get
			{
				return this.changes.Count;
			}
		}

		[XmlElement("ReadFlagChange", typeof(SyncFolderItemsReadFlagType))]
		[XmlChoiceIdentifier("ChangesElementName")]
		[DataMember(Name = "Changes", EmitDefaultValue = true)]
		[XmlElement("Create", typeof(SyncFolderItemsCreateOrUpdateType))]
		[XmlElement("Delete", typeof(SyncFolderItemsDeleteType))]
		[XmlElement("Update", typeof(SyncFolderItemsCreateOrUpdateType))]
		public SyncFolderItemsChangeTypeBase[] Changes
		{
			get
			{
				return this.changes.ToArray();
			}
			set
			{
				this.changes = new List<SyncFolderItemsChangeTypeBase>(value);
			}
		}

		[XmlElement("ChangesElementName")]
		[IgnoreDataMember]
		[XmlIgnore]
		public SyncFolderItemsChangesEnum[] ChangesElementName
		{
			get
			{
				return this.changesElementName.ToArray();
			}
			set
			{
				this.changesElementName = new List<SyncFolderItemsChangesEnum>(value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public int TotalCount { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IncludesLastItemInRange { get; set; }

		[IgnoreDataMember]
		[DateTimeString]
		[XmlIgnore]
		public string OldestReceivedTime { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool MoreItemsOnServer { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public string SyncState { get; set; }

		private List<SyncFolderItemsChangeTypeBase> changes;

		private List<SyncFolderItemsChangesEnum> changesElementName;
	}
}
