using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SyncFolderHierarchyType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SyncFolderHierarchyRequest : BaseRequest
	{
		[DataMember(Name = "FolderShape", IsRequired = true, Order = 1)]
		[XmlElement(ElementName = "FolderShape", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public FolderResponseShape FolderShape { get; set; }

		[DataMember(Name = "ShapeName", IsRequired = false, Order = 2)]
		[XmlIgnore]
		public string ShapeName { get; set; }

		[DataMember(Name = "SyncFolderId", IsRequired = false, Order = 3)]
		[XmlElement("SyncFolderId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public TargetFolderId SyncFolderId { get; set; }

		[XmlElement(ElementName = "SyncState", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "SyncState", IsRequired = false, Order = 4)]
		public string SyncState { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ReturnRootFolder", IsRequired = false, Order = 5)]
		public bool ReturnRootFolder { get; set; }

		[DataMember(Name = "ReturnPeopleIKnowFolder", IsRequired = false)]
		[XmlIgnore]
		public bool ReturnPeopleIKnowFolder { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public DistinguishedFolderIdName[] FolderToMoveToTop { get; set; }

		[DataMember(Name = "FolderToMoveToTop", IsRequired = false)]
		[XmlIgnore]
		public string[] FoldersToMoveToTopString
		{
			get
			{
				return EnumUtilities.ToStringArray<DistinguishedFolderIdName>(this.FolderToMoveToTop);
			}
			set
			{
				this.FolderToMoveToTop = EnumUtilities.ParseStringArray<DistinguishedFolderIdName>(value);
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SyncFolderHierarchy(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.SyncFolderId == null || this.SyncFolderId.BaseFolderId == null)
			{
				return callContext.GetServerInfoForEffectiveCaller();
			}
			return BaseRequest.GetServerInfoForFolderId(callContext, this.SyncFolderId.BaseFolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}
	}
}
