using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SyncConversationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SyncConversationRequest : BaseRequest
	{
		[DataMember(Name = "SyncState", IsRequired = false)]
		[XmlElement("SyncState", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string SyncState { get; set; }

		[DataMember(Name = "MaxChangesReturned", IsRequired = true)]
		[XmlElement("MaxChangesReturned", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public int MaxChangesReturned { get; set; }

		[XmlElement("NumberOfDays", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "NumberOfDays", EmitDefaultValue = false, IsRequired = false)]
		public int NumberOfDays { get; set; }

		[DataMember(Name = "MinimumCount", EmitDefaultValue = false, IsRequired = false)]
		[XmlElement("MinimumCount", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public int MinimumCount { get; set; }

		[DataMember(Name = "MaximumCount", EmitDefaultValue = false, IsRequired = false)]
		[XmlElement("MaximumCount", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public int MaximumCount { get; set; }

		[XmlElement("FolderIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem("FolderId", typeof(TargetFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "FolderIds", IsRequired = true)]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderId[] FolderIds { get; set; }

		[DataMember(Name = "IsPartialFolderList", IsRequired = false)]
		[XmlElement("IsPartialFolderList", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public bool IsPartialFolderList { get; set; }

		[DataMember(Name = "DoQuickSync", IsRequired = false)]
		[XmlElement("DoQuickSync", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public bool DoQuickSync { get; set; }

		[XmlElement]
		[DataMember(Name = "ConversationShape", IsRequired = false)]
		public ConversationResponseShape ConversationShape { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ShapeName", IsRequired = false)]
		public string ShapeName { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SyncConversation(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.FolderIds == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForFolderIdList(callContext, this.FolderIds);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}
	}
}
