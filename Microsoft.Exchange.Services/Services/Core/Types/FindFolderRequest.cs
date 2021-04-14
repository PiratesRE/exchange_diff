using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "FindFolderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class FindFolderRequest : BaseRequest, IRemoteArchiveRequest
	{
		[DataMember(IsRequired = true)]
		[XmlElement]
		public FolderResponseShape FolderShape { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ShapeName", IsRequired = false)]
		public string ShapeName { get; set; }

		[XmlElement("FractionalPageFolderView", typeof(FractionalPageView))]
		[XmlElement("IndexedPageFolderView", typeof(IndexedPageView))]
		[DataMember(IsRequired = true)]
		public Microsoft.Exchange.Services.Core.Search.BasePagingType Paging { get; set; }

		[XmlElement("Restriction", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public RestrictionType Restriction { get; set; }

		[DataMember(IsRequired = true)]
		[XmlArrayItem("FolderId", typeof(FolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderId[] ParentFolderIds { get; set; }

		[IgnoreDataMember]
		[XmlElement("MailboxGuid")]
		public Guid MailboxGuid { get; set; }

		[DataMember(Name = "MailboxGuid", IsRequired = false)]
		[XmlIgnore]
		public string MailboxGuidString
		{
			get
			{
				return this.MailboxGuid.ToString();
			}
			set
			{
				this.MailboxGuid = (string.IsNullOrEmpty(value) ? Guid.Empty : Guid.Parse(value));
			}
		}

		[IgnoreDataMember]
		[XmlAttribute]
		public FolderQueryTraversal Traversal { get; set; }

		[DataMember(Name = "Traversal", IsRequired = true)]
		[XmlIgnore]
		public string TraversalString
		{
			get
			{
				return EnumUtilities.ToString<FolderQueryTraversal>(this.Traversal);
			}
			set
			{
				this.Traversal = EnumUtilities.Parse<FolderQueryTraversal>(value);
			}
		}

		[XmlIgnore]
		[DataMember(IsRequired = false)]
		public bool ReturnParentFolder { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public DistinguishedFolderIdName[] FoldersToMoveToTop { get; set; }

		[DataMember(Name = "FoldersToMoveToTop", IsRequired = false)]
		[XmlIgnore]
		public string[] FoldersToMoveToTopString
		{
			get
			{
				return EnumUtilities.ToStringArray<DistinguishedFolderIdName>(this.FoldersToMoveToTop);
			}
			set
			{
				this.FoldersToMoveToTop = EnumUtilities.ParseStringArray<DistinguishedFolderIdName>(value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public DistinguishedFolderIdName[] RequiredFolders { get; set; }

		[DataMember(Name = "RequiredFolders", IsRequired = false)]
		[XmlIgnore]
		public string[] RequiredFoldersString
		{
			get
			{
				return EnumUtilities.ToStringArray<DistinguishedFolderIdName>(this.RequiredFolders);
			}
			set
			{
				this.RequiredFolders = EnumUtilities.ParseStringArray<DistinguishedFolderIdName>(value);
			}
		}

		protected override List<ServiceObjectId> GetAllIds()
		{
			if (this.ParentFolderIds == null)
			{
				return null;
			}
			return new List<ServiceObjectId>(this.ParentFolderIds);
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			if (this != null && ((IRemoteArchiveRequest)this).IsRemoteArchiveRequest(callContext))
			{
				return ((IRemoteArchiveRequest)this).GetRemoteArchiveServiceCommand(callContext);
			}
			return new FindFolder(callContext, this);
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
			if (this.ParentFolderIds == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForFolderIdListHierarchyOperations(callContext, this.ParentFolderIds);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.ParentFolderIds == null || this.ParentFolderIds.Length < taskStep)
			{
				return null;
			}
			if (this != null && ((IRemoteArchiveRequest)this).IsRemoteArchiveRequest(callContext))
			{
				return null;
			}
			return base.GetResourceKeysForFolderIdHierarchyOperations(false, callContext, this.ParentFolderIds[taskStep]);
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.MailboxGuid == Guid.Empty && (this.ParentFolderIds == null || this.ParentFolderIds.Length == 0))
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidIdMalformed), FaultParty.Sender);
			}
		}

		ExchangeServiceBinding IRemoteArchiveRequest.ArchiveService { get; set; }

		bool IRemoteArchiveRequest.IsRemoteArchiveRequest(CallContext callContext)
		{
			return ComplianceUtil.TryCreateArchiveService(callContext, this, this.ParentFolderIds != null, delegate
			{
				((IRemoteArchiveRequest)this).ArchiveService = ComplianceUtil.GetArchiveServiceForFolder(callContext, this.ParentFolderIds);
			});
		}

		ServiceCommandBase IRemoteArchiveRequest.GetRemoteArchiveServiceCommand(CallContext callContext)
		{
			return new FindRemoteArchiveFolder(callContext, this);
		}

		internal const string ParentFolderIdsElementName = "ParentFolderIds";
	}
}
