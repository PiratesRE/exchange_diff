using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SyncFolderHierarchyChangesType
	{
		public SyncFolderHierarchyChangesType()
		{
			this.changes = new List<SyncFolderHierarchyChangeBase>();
			this.changesElementName = new List<SyncFolderHierarchyChangesEnum>();
		}

		public void AddChange(SyncFolderHierarchyChangeBase change)
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

		[XmlElement("Delete", typeof(SyncFolderHierarchyDeleteType))]
		[XmlElement("Create", typeof(SyncFolderHierarchyCreateOrUpdateType))]
		[XmlElement("Update", typeof(SyncFolderHierarchyCreateOrUpdateType))]
		[XmlChoiceIdentifier("ChangesElementName")]
		[DataMember(Name = "Changes", EmitDefaultValue = true)]
		public SyncFolderHierarchyChangeBase[] Changes
		{
			get
			{
				return this.changes.ToArray();
			}
			set
			{
				this.changes = new List<SyncFolderHierarchyChangeBase>(value);
			}
		}

		[XmlIgnore]
		[XmlElement("ChangesElementName")]
		[IgnoreDataMember]
		public SyncFolderHierarchyChangesEnum[] ChangesElementName
		{
			get
			{
				return this.changesElementName.ToArray();
			}
			set
			{
				this.changesElementName = new List<SyncFolderHierarchyChangesEnum>(value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IncludesLastFolderInRange { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public string SyncState { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public BaseFolderType RootFolder { get; set; }

		private List<SyncFolderHierarchyChangeBase> changes;

		private List<SyncFolderHierarchyChangesEnum> changesElementName;
	}
}
