using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SyncFolderItemsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SyncFolderItemsRequest : BaseRequest
	{
		public SyncFolderItemsRequest()
		{
			this.Init();
		}

		private void Init()
		{
			this.SyncScope = SyncFolderItemsScope.NormalItems;
			this.NumberOfDays = -1;
			this.MinimumCount = -1;
			this.MaximumCount = 5000;
		}

		[XmlElement(ElementName = "ItemShape")]
		[DataMember(Name = "ItemShape", IsRequired = true)]
		public ItemResponseShape ItemShape { get; set; }

		[DataMember(Name = "ShapeName", IsRequired = false)]
		[XmlIgnore]
		public string ShapeName { get; set; }

		[DataMember(Name = "SyncFolderId", IsRequired = true)]
		[XmlElement("SyncFolderId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public TargetFolderId SyncFolderId { get; set; }

		[XmlElement("SyncState", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "SyncState", IsRequired = false)]
		public string SyncState { get; set; }

		[XmlArrayItem("ItemId", typeof(ItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "Ignore", IsRequired = false)]
		[XmlArray("Ignore", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ItemId[] Ignore { get; set; }

		[XmlElement("MaxChangesReturned", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "MaxChangesReturned", IsRequired = true)]
		public int MaxChangesReturned { get; set; }

		[XmlElement("NumberOfDays", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "NumberOfDays", IsRequired = false)]
		public int NumberOfDays { get; set; }

		[XmlIgnore]
		[DataMember(Name = "MinimumCount", IsRequired = false)]
		public int MinimumCount { get; set; }

		[DataMember(Name = "MaximumCount", IsRequired = false)]
		[XmlIgnore]
		public int MaximumCount { get; set; }

		[DataMember(Name = "DoQuickSync", IsRequired = false)]
		[XmlElement("DoQuickSync", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public bool DoQuickSync { get; set; }

		[XmlElement("SyncScope", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[IgnoreDataMember]
		public SyncFolderItemsScope SyncScope { get; set; }

		[XmlIgnore]
		[DataMember(Name = "SyncScope", IsRequired = false)]
		public string SyncScopeString
		{
			get
			{
				return EnumUtilities.ToString<SyncFolderItemsScope>(this.SyncScope);
			}
			set
			{
				this.SyncScope = EnumUtilities.Parse<SyncFolderItemsScope>(value);
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SyncFolderItems(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.SyncFolderId == null || this.SyncFolderId.BaseFolderId == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForFolderId(callContext, this.SyncFolderId.BaseFolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		[OnDeserializing]
		private void Init(StreamingContext context)
		{
			this.Init();
		}
	}
}
