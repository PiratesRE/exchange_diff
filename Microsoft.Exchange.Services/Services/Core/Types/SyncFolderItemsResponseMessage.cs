using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("SyncFolderItemsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SyncFolderItemsResponseMessage : ResponseMessage
	{
		public SyncFolderItemsResponseMessage()
		{
		}

		internal SyncFolderItemsResponseMessage(ServiceResultCode code, ServiceError error, SyncFolderItemsChangesType value) : base(code, error)
		{
			if (value != null)
			{
				this.SyncState = value.SyncState;
				this.IncludesLastItemInRange = value.IncludesLastItemInRange;
				this.Changes = value;
				this.MoreItemsOnServer = value.MoreItemsOnServer;
				this.OldestReceivedTime = value.OldestReceivedTime;
				this.TotalCount = value.TotalCount;
				return;
			}
			ExTraceGlobals.SynchronizationTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItemsChangesType value is null.");
			this.SyncState = string.Empty;
			this.IncludesLastItemInRange = true;
			this.MoreItemsOnServer = true;
			this.OldestReceivedTime = ExDateTime.UtcNow.ToISOString();
			this.TotalCount = 0;
		}

		[XmlElement("SyncState", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "SyncState", IsRequired = true)]
		public string SyncState { get; set; }

		[XmlElement("IncludesLastItemInRange", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "IncludesLastItemInRange", IsRequired = false, EmitDefaultValue = false)]
		public bool IncludesLastItemInRange
		{
			get
			{
				return this.includesLastItemInRange;
			}
			set
			{
				this.IncludesLastItemInRangeSpecified = true;
				this.includesLastItemInRange = value;
			}
		}

		[DataMember(Name = "TotalCount", EmitDefaultValue = true)]
		[XmlIgnore]
		public int TotalCount { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IncludesLastItemInRangeSpecified { get; set; }

		[XmlIgnore]
		[DateTimeString]
		[DataMember(Name = "OldestReceivedTime", IsRequired = false, EmitDefaultValue = false)]
		public string OldestReceivedTime { get; set; }

		[XmlIgnore]
		[DataMember(Name = "MoreItemsOnServer", IsRequired = false, EmitDefaultValue = false)]
		public bool MoreItemsOnServer { get; set; }

		[XmlElement("Changes")]
		[DataMember(Name = "Changes", IsRequired = false, EmitDefaultValue = false)]
		public SyncFolderItemsChangesType Changes { get; set; }

		private bool includesLastItemInRange;
	}
}
