using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("EmptyFolderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class EmptyFolderRequest : BaseRequest
	{
		[DataMember(Name = "FolderIds", IsRequired = true, Order = 1)]
		[XmlArray("FolderIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderId[] FolderIds { get; set; }

		[XmlAttribute("DeleteType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[IgnoreDataMember]
		public DisposalType DeleteType { get; set; }

		[XmlIgnore]
		[DataMember(Name = "DeleteType", IsRequired = true, Order = 2)]
		public string DeleteTypeString
		{
			get
			{
				return this.DeleteType.ToString();
			}
			set
			{
				this.DeleteType = (DisposalType)Enum.Parse(typeof(DisposalType), value);
			}
		}

		[XmlAttribute("DeleteSubFolders", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "DeleteSubFolders", IsRequired = true, Order = 3)]
		public bool DeleteSubFolders { get; set; }

		[XmlIgnore]
		[DataMember(Name = "AllowSearchFolder", IsRequired = false)]
		public bool AllowSearchFolder { get; set; }

		[DateTimeString]
		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string ClientLastSyncTime { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public bool SuppressReadReceipt { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new EmptyFolder(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.FolderIds == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForFolderIdListHierarchyOperations(callContext, this.FolderIds);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.FolderIds == null || this.FolderIds.Length < taskStep)
			{
				return null;
			}
			return base.GetResourceKeysForFolderIdHierarchyOperations(true, callContext, this.FolderIds[taskStep]);
		}
	}
}
