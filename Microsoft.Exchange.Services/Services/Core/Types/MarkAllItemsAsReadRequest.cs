using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("MarkAllItemsAsReadRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MarkAllItemsAsReadRequest : BaseRequest
	{
		[XmlArray("FolderIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "FolderIds", IsRequired = true, Order = 1)]
		[XmlArrayItem("FolderId", typeof(FolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderId[] FolderIds { get; set; }

		[DataMember(Name = "ReadFlag", IsRequired = true, Order = 2)]
		[XmlElement("ReadFlag", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public bool ReadFlag { get; set; }

		[DataMember(Name = "SuppressReadReceipts", IsRequired = true, Order = 3)]
		[XmlElement("SuppressReadReceipts", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public bool SuppressReadReceipts { get; set; }

		[DataMember(Name = "FromFilter", IsRequired = false)]
		[XmlIgnore]
		public string FromFilter { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			if (string.IsNullOrEmpty(this.FromFilter))
			{
				return new MarkAllItemsAsRead(callContext, this);
			}
			return new PeopleIKnowMarkAllAsReadCommand(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.FolderIds == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForFolderIdList(callContext, this.FolderIds);
		}

		internal override void Validate()
		{
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.FolderIds == null || this.FolderIds.Length < taskStep)
			{
				return null;
			}
			return base.GetResourceKeysForFolderId(true, callContext, this.FolderIds[taskStep]);
		}
	}
}
