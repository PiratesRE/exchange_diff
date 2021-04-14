using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("BaseMoveCopyFolderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(MoveFolderRequest))]
	[KnownType(typeof(CopyFolderRequest))]
	[KnownType(typeof(MoveFolderRequest))]
	[XmlInclude(typeof(CopyFolderRequest))]
	public class BaseMoveCopyFolderRequest : BaseRequest
	{
		[XmlElement("ToFolderId")]
		[DataMember(IsRequired = true, Order = 1)]
		public TargetFolderId ToFolderId { get; set; }

		[DataMember(Name = "FolderIds", IsRequired = true, Order = 2)]
		[XmlArrayItem("FolderId", typeof(FolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray("FolderIds")]
		public BaseFolderId[] Ids { get; set; }

		protected override List<ServiceObjectId> GetAllIds()
		{
			return new List<ServiceObjectId>(this.Ids)
			{
				this.ToFolderId.BaseFolderId
			};
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
			return BaseRequest.GetServerInfoForFolderIdHierarchyOperations(callContext, this.ToFolderId.BaseFolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			if (this.toFolderResourceKey == null)
			{
				BaseServerIdInfo serverInfoForFolderIdHierarchyOperations = BaseRequest.GetServerInfoForFolderIdHierarchyOperations(callContext, this.ToFolderId.BaseFolderId);
				if (serverInfoForFolderIdHierarchyOperations != null)
				{
					this.toFolderResourceKey = serverInfoForFolderIdHierarchyOperations.ToResourceKey(true);
				}
			}
			ResourceKey[] array = null;
			BaseServerIdInfo serverInfoForFolderIdHierarchyOperations2 = BaseRequest.GetServerInfoForFolderIdHierarchyOperations(callContext, this.Ids[currentStep]);
			if (serverInfoForFolderIdHierarchyOperations2 != null)
			{
				array = serverInfoForFolderIdHierarchyOperations2.ToResourceKey(false);
			}
			List<ResourceKey> list = new List<ResourceKey>();
			if (this.toFolderResourceKey != null)
			{
				list.AddRange(this.toFolderResourceKey);
			}
			if (array != null)
			{
				list.AddRange(array);
			}
			if (list.Count != 0)
			{
				return list.ToArray();
			}
			return null;
		}

		public const string ToFolderIdElementName = "ToFolderId";

		public const string FolderIdsElementName = "FolderIds";

		private ResourceKey[] toFolderResourceKey;
	}
}
