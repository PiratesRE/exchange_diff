using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SyncFolderHierarchyResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SyncFolderHierarchyResponseMessage : ResponseMessage
	{
		public SyncFolderHierarchyResponseMessage()
		{
		}

		internal SyncFolderHierarchyResponseMessage(ServiceResultCode code, ServiceError error, SyncFolderHierarchyChangesType value) : base(code, error)
		{
			if (value != null)
			{
				this.Changes = value;
				this.SyncState = value.SyncState;
				this.IncludesLastFolderInRange = value.IncludesLastFolderInRange;
				this.RootFolder = value.RootFolder;
				return;
			}
			this.SyncState = string.Empty;
			this.IncludesLastFolderInRange = true;
		}

		[DataMember(Name = "SyncState", IsRequired = true, EmitDefaultValue = true)]
		[XmlElement("SyncState", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string SyncState { get; set; }

		[XmlElement("IncludesLastFolderInRange", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "IncludesLastFolderInRange", IsRequired = true, EmitDefaultValue = true)]
		public bool IncludesLastFolderInRange
		{
			get
			{
				return this.includesLastFolderInRange;
			}
			set
			{
				this.IncludesLastFolderInRangeSpecified = true;
				this.includesLastFolderInRange = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IncludesLastFolderInRangeSpecified { get; set; }

		[XmlElement("Changes")]
		[DataMember(Name = "Changes", IsRequired = true, EmitDefaultValue = true)]
		public SyncFolderHierarchyChangesType Changes { get; set; }

		[DataMember(Name = "RootFolder", IsRequired = false, EmitDefaultValue = false)]
		[XmlIgnore]
		private BaseFolderType RootFolder { get; set; }

		private bool includesLastFolderInRange;
	}
}
