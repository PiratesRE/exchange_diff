using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetFolderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetFolderRequest : BaseRequest, IRemoteArchiveRequest
	{
		[DataMember(Name = "FolderShape", IsRequired = true, Order = 1)]
		[XmlElement(ElementName = "FolderShape")]
		public FolderResponseShape FolderShape { get; set; }

		[DataMember(Name = "ShapeName", IsRequired = false, Order = 2)]
		[XmlIgnore]
		public string ShapeName { get; set; }

		[XmlArray("FolderIds")]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "FolderIds", IsRequired = true, Order = 3)]
		public BaseFolderId[] Ids { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			if (this != null && ((IRemoteArchiveRequest)this).IsRemoteArchiveRequest(callContext))
			{
				return ((IRemoteArchiveRequest)this).GetRemoteArchiveServiceCommand(callContext);
			}
			return new GetFolder(callContext, this);
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
			if (this.Ids == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForFolderIdListHierarchyOperations(callContext, this.Ids);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.Ids == null || this.Ids.Length < taskStep)
			{
				return null;
			}
			if (this != null && ((IRemoteArchiveRequest)this).IsRemoteArchiveRequest(callContext))
			{
				return null;
			}
			return base.GetResourceKeysForFolderIdHierarchyOperations(false, callContext, this.Ids[taskStep]);
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.Ids == null || this.Ids.Length == 0)
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidIdMalformed), FaultParty.Sender);
			}
		}

		protected override List<ServiceObjectId> GetAllIds()
		{
			return new List<ServiceObjectId>(this.Ids);
		}

		ExchangeServiceBinding IRemoteArchiveRequest.ArchiveService { get; set; }

		bool IRemoteArchiveRequest.IsRemoteArchiveRequest(CallContext callContext)
		{
			return ComplianceUtil.TryCreateArchiveService(callContext, this, this.Ids != null, delegate
			{
				((IRemoteArchiveRequest)this).ArchiveService = ComplianceUtil.GetArchiveServiceForFolder(callContext, this.Ids);
			});
		}

		ServiceCommandBase IRemoteArchiveRequest.GetRemoteArchiveServiceCommand(CallContext callContext)
		{
			return new GetRemoteArchiveFolder(callContext, this);
		}

		internal const string FolderIdsElementName = "FolderIds";
	}
}
