using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(FolderId))]
	[KnownType(typeof(DistinguishedFolderId))]
	[XmlType("DeleteFolderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class DeleteFolderRequest : BaseRequest
	{
		[XmlArray("FolderIds")]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "FolderIds", IsRequired = true, Order = 1)]
		public BaseFolderId[] Ids { get; set; }

		[XmlAttribute("DeleteType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[IgnoreDataMember]
		public DisposalType DeleteType { get; set; }

		[DataMember(Name = "DeleteType", IsRequired = true, Order = 2)]
		[XmlIgnore]
		public string DeleteTypeString
		{
			get
			{
				return EnumUtilities.ToString<DisposalType>(this.DeleteType);
			}
			set
			{
				this.DeleteType = EnumUtilities.Parse<DisposalType>(value);
			}
		}

		protected override List<ServiceObjectId> GetAllIds()
		{
			return new List<ServiceObjectId>(this.Ids);
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new DeleteFolder(callContext, this);
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
			return base.GetResourceKeysForFolderIdHierarchyOperations(true, callContext, this.Ids[taskStep]);
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.Ids == null || this.Ids.Length == 0)
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidIdMalformed), FaultParty.Sender);
			}
		}

		internal const string FolderIdsElementName = "FolderIds";

		internal const string DeleteTypeElementName = "DeleteType";
	}
}
