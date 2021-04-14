using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("UpdateFolderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class UpdateFolderRequest : BaseRequest
	{
		[XmlArrayItem(ElementName = "FolderChange", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(FolderChange))]
		[DataMember(Name = "FolderChanges", IsRequired = true)]
		[XmlArray(ElementName = "FolderChanges", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public FolderChange[] FolderChanges { get; set; }

		protected override List<ServiceObjectId> GetAllIds()
		{
			List<ServiceObjectId> list = new List<ServiceObjectId>();
			foreach (FolderChange folderChange in this.FolderChanges)
			{
				list.Add(folderChange.FolderId);
			}
			return list;
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new UpdateFolder(callContext, this);
		}

		internal override bool IsHierarchicalOperation
		{
			get
			{
				return true;
			}
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.FolderChanges == null || this.FolderChanges.Length == 0)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForFolderIdHierarchyOperations(callContext, this.FolderChanges[0].FolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.FolderChanges == null || this.FolderChanges.Length == 0)
			{
				return null;
			}
			BaseServerIdInfo serverInfoForFolderIdHierarchyOperations = BaseRequest.GetServerInfoForFolderIdHierarchyOperations(callContext, this.FolderChanges[taskStep].FolderId);
			return BaseRequest.ServerInfoToResourceKeys(true, serverInfoForFolderIdHierarchyOperations);
		}

		internal const string FolderChangesElementName = "FolderChanges";
	}
}
