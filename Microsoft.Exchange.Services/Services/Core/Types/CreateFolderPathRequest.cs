using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("CreateFolderPathType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateFolderPathRequest : BaseRequest
	{
		[XmlElement("ParentFolderId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "ParentFolderId", IsRequired = true, Order = 1)]
		public TargetFolderId ParentFolderId { get; set; }

		[DataMember(Name = "RelativeFolderPath", IsRequired = true, Order = 2)]
		[XmlArrayItem("CalendarFolder", typeof(CalendarFolderType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray("RelativeFolderPath")]
		[XmlArrayItem("ContactsFolder", typeof(ContactsFolderType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("Folder", typeof(FolderType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("TasksFolder", typeof(TasksFolderType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderType[] RelativeFolderPath { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new CreateFolderPath(callContext, this);
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
			return BaseRequest.GetServerInfoForFolderIdHierarchyOperations(callContext, this.ParentFolderId.BaseFolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysForFolderIdHierarchyOperations(true, callContext, this.ParentFolderId.BaseFolderId);
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.RelativeFolderPath == null || this.RelativeFolderPath.Length == 0)
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidIdMalformed), FaultParty.Sender);
			}
			foreach (BaseFolderType baseFolderType in this.RelativeFolderPath)
			{
				if (baseFolderType.StoreObjectType == StoreObjectType.SearchFolder || baseFolderType.StoreObjectType == StoreObjectType.OutlookSearchFolder)
				{
					throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidFolderTypeForOperation), FaultParty.Sender);
				}
			}
		}

		internal const string ParentFolderIdElementName = "ParentFolderId";

		internal const string RelativePathElementName = "RelativeFolderPath";
	}
}
